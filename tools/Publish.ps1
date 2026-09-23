<#
.SYNOPSIS
    建置 DMDService 並整理成可以直接交給別人執行的發佈資料夾（不含程式碼）。

.DESCRIPTION
    產出結構：
      Publish\
        DMDService\   Windows 服務 + 所有 Task 執行檔、DLL、Config\、安裝/移除服務的 bat
        UITest\       測試工具 UITest.exe 與所需 DLL
        README.txt    使用說明
        BuildInfo.txt 建置時間、組態、git commit

.PARAMETER Configuration
    Release (預設) 或 Debug

.PARAMETER Output
    輸出資料夾，預設為專案根目錄下的 Publish。每次執行會先清空。

.PARAMETER NoBuild
    不重新建置，直接收集目前 bin\<Configuration> 的檔案

.PARAMETER IncludePdb
    一併複製 .pdb (方便在現場看到例外的行號)

.PARAMETER Zip
    另外壓成 Publish_yyyyMMdd_HHmm.zip

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\Publish.ps1
    powershell -ExecutionPolicy Bypass -File tools\Publish.ps1 -Configuration Debug -IncludePdb -Zip
#>
[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',
    [string]$Output = '',
    [switch]$NoBuild,
    [switch]$IncludePdb,
    [switch]$Zip
)

$ErrorActionPreference = 'Stop'

$RepoRoot = Split-Path -Parent $PSScriptRoot
$Solution = Join-Path $RepoRoot 'DMDService.sln'
$Templates = Join-Path $PSScriptRoot 'publish-templates'
if ([string]::IsNullOrWhiteSpace($Output)) { $Output = Join-Path $RepoRoot 'Publish' }

# 服務需要的專案 (全部放在同一層，TaskKernel/TaskMain 以相對路徑啟動其他 Task)
$ServiceProjects = @('KernelService', 'TaskKernel', 'TaskMain', 'TaskCMFT', 'TaskDCU', 'TaskOCS')
$UITestProjects = @('UITest')

# 建置輸出中不需要帶走的檔案
$ExcludePatterns = @('*.vshost.*', '*.lastcodeanalysissucceeded', '*.CodeAnalysisLog.xml', '*.InstallLog', '*.InstallState')

function Write-Step([string]$msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Write-Warn([string]$msg) { Write-Host "  [警告] $msg" -ForegroundColor Yellow }

function Find-MSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $path = & $vswhere -latest -prerelease -products * -requires Microsoft.Component.MSBuild `
                    -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
        if ($path -and (Test-Path $path)) { return $path }
    }
    $cmd = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    throw '找不到 MSBuild。請安裝 Visual Studio 或 Build Tools，或在「Developer Command Prompt」中執行。'
}

function Get-FileHashText([string]$path) {
    return (Get-FileHash -Algorithm MD5 -LiteralPath $path).Hash
}

# 把一個專案的 bin\<Configuration> 複製到目的資料夾；同名但內容不同的檔案保留較新的並提出警告
function Copy-ProjectOutput([string]$project, [string]$dest) {
    $binDir = Join-Path $RepoRoot "$project\bin\$Configuration"
    if (-not (Test-Path $binDir)) {
        throw "找不到 $binDir，請先建置 ($Configuration)。"
    }

    $files = Get-ChildItem -LiteralPath $binDir -File
    foreach ($f in $files) {
        $skip = $false
        foreach ($p in $ExcludePatterns) { if ($f.Name -like $p) { $skip = $true; break } }
        if ($f.Extension -eq '.pdb' -and -not $IncludePdb) { $skip = $true }
        # DLL 的 XML 文件註解檔 (與 DLL/EXE 同名的 .xml) 不需要
        if ($f.Extension -eq '.xml') {
            $base = [IO.Path]::GetFileNameWithoutExtension($f.Name)
            if ((Test-Path (Join-Path $binDir "$base.dll")) -or (Test-Path (Join-Path $binDir "$base.exe"))) { $skip = $true }
        }
        if ($skip) { continue }

        $target = Join-Path $dest $f.Name
        if (Test-Path -LiteralPath $target) {
            if ((Get-FileHashText $target) -ne (Get-FileHashText $f.FullName)) {
                $existing = Get-Item -LiteralPath $target
                if ($f.LastWriteTime -gt $existing.LastWriteTime) {
                    Write-Warn "$($f.Name) 在多個專案輸出中版本不同，採用較新的 $project\bin\$Configuration\$($f.Name)"
                    Copy-Item -LiteralPath $f.FullName -Destination $target -Force
                } else {
                    Write-Warn "$($f.Name) 在多個專案輸出中版本不同，保留較新的版本 (略過 $project)"
                }
            }
        } else {
            Copy-Item -LiteralPath $f.FullName -Destination $target
        }
    }
}

# ---------------------------------------------------------------------------
Write-Host "DMDService 發佈" -ForegroundColor Green
Write-Host "  專案     : $RepoRoot"
Write-Host "  組態     : $Configuration"
Write-Host "  輸出     : $Output"

# 1. 建置 ---------------------------------------------------------------------
if (-not $NoBuild) {
    Write-Step '建置方案'
    $msbuild = Find-MSBuild
    Write-Host "  MSBuild  : $msbuild"

    # PostBuildEvent= 清空各專案 xcopy 到 D:\ASI.Wanda.DMD.DMDService 的動作，避免沒有 D 槽時建置失敗
    $msbuildArgs = @(
        $Solution,
        '/t:Rebuild',
        '/restore',
        "/p:Configuration=$Configuration",
        '/p:RestorePackagesConfig=true',
        '/p:PostBuildEvent=',
        '/m',
        '/nologo',
        '/v:minimal'
    )
    & $msbuild @msbuildArgs
    if ($LASTEXITCODE -ne 0) { throw "建置失敗 (exit code $LASTEXITCODE)，未產生發佈資料夾。" }
}

# 2. 準備輸出資料夾 -------------------------------------------------------------
Write-Step '整理發佈資料夾'
if (Test-Path $Output) {
    try {
        Remove-Item -LiteralPath $Output -Recurse -Force
    } catch {
        throw "無法清除 $Output，請確認服務 / UITest 沒有從這個資料夾執行中。$($_.Exception.Message)"
    }
}
$ServiceDir = Join-Path $Output 'DMDService'
$UITestDir = Join-Path $Output 'UITest'
New-Item -ItemType Directory -Path $ServiceDir, $UITestDir | Out-Null

# 3. 服務 ----------------------------------------------------------------------
foreach ($p in $ServiceProjects) {
    Write-Host "  DMDService <- $p"
    Copy-ProjectOutput $p $ServiceDir
}

# Config 以專案內的 TaskMain\Config 為準
$configSrc = Join-Path $RepoRoot 'TaskMain\Config'
$configDst = Join-Path $ServiceDir 'Config'
New-Item -ItemType Directory -Path $configDst | Out-Null
Copy-Item -Path (Join-Path $configSrc '*.xml') -Destination $configDst
Write-Host "  DMDService <- TaskMain\Config"

Copy-Item -Path (Join-Path $Templates 'DMDService\*') -Destination $ServiceDir

# 必要檔案檢查
$required = @('ASI.Wanda.DMD.DMDService.exe', 'TaskKernel.exe', 'TaskMain.exe', 'TaskCMFT.exe', 'TaskDCU.exe', 'TaskOCS.exe', 'ASI.Lib.dll', 'Npgsql.dll', 'Config\Config.xml')
foreach ($r in $required) {
    if (-not (Test-Path (Join-Path $ServiceDir $r))) { throw "發佈結果缺少 DMDService\$r" }
}

# 4. UITest --------------------------------------------------------------------
foreach ($p in $UITestProjects) {
    Write-Host "  UITest     <- $p"
    Copy-ProjectOutput $p $UITestDir
}
if (-not (Test-Path (Join-Path $UITestDir 'UITest.exe'))) { throw '發佈結果缺少 UITest\UITest.exe' }

# 5. 說明與建置資訊 ---------------------------------------------------------------
Copy-Item -LiteralPath (Join-Path $Templates 'README.txt') -Destination $Output

$commit = ''
$dirty = ''
try {
    $commit = (& git -C $RepoRoot rev-parse --short HEAD 2>$null)
    if ((& git -C $RepoRoot status --porcelain 2>$null)) { $dirty = ' (含未 commit 的修改)' }
} catch { }

$info = @(
    "建置時間 : $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
    "建置電腦 : $env:COMPUTERNAME",
    "組態     : $Configuration",
    "Git      : $commit$dirty"
)
$utf8Bom = New-Object System.Text.UTF8Encoding($true)
[IO.File]::WriteAllLines((Join-Path $Output 'BuildInfo.txt'), $info, $utf8Bom)

# 6. 壓縮 ----------------------------------------------------------------------
if ($Zip) {
    Write-Step '壓縮'
    $zipPath = Join-Path (Split-Path -Parent $Output) ("Publish_{0}.zip" -f (Get-Date -Format 'yyyyMMdd_HHmm'))
    if (Test-Path $zipPath) { Remove-Item -LiteralPath $zipPath -Force }
    Compress-Archive -Path (Join-Path $Output '*') -DestinationPath $zipPath
    Write-Host "  $zipPath"
}

# 7. 結果 ----------------------------------------------------------------------
$svcCount = (Get-ChildItem $ServiceDir -Recurse -File).Count
$uiCount = (Get-ChildItem $UITestDir -Recurse -File).Count
$leaked = Get-ChildItem $Output -Recurse -File -Include *.cs, *.csproj, *.sln, *.resx
if ($leaked) { Write-Warn "發佈資料夾中出現程式碼檔案：$($leaked.FullName -join ', ')" }

Write-Host ""
Write-Host "完成！" -ForegroundColor Green
Write-Host "  $ServiceDir  ($svcCount 個檔案)"
Write-Host "  $UITestDir  ($uiCount 個檔案)"
Write-Host "  交付前請先確認 DMDService\Config\Config.xml 是目標環境的設定。"
