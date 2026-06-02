# 下载 Npgsql 所需的依赖 DLL
$packageDir = "$PSScriptRoot\packages"
$commonDir = "$PSScriptRoot\common"

# 创建 packages 目录
if (-not (Test-Path $packageDir)) {
    New-Item -ItemType Directory -Path $packageDir | Out-Null
}

Write-Host "正在下载依赖包..." -ForegroundColor Green

# 下载 Microsoft.Bcl.AsyncInterfaces 7.0.0
$asyncInterfacesUrl = "https://www.nuget.org/api/v2/package/Microsoft.Bcl.AsyncInterfaces/7.0.0"
$asyncInterfacesZip = "$packageDir\Microsoft.Bcl.AsyncInterfaces.7.0.0.zip"
Invoke-WebRequest -Uri $asyncInterfacesUrl -OutFile $asyncInterfacesZip
Write-Host "下载 Microsoft.Bcl.AsyncInterfaces 完成" -ForegroundColor Yellow

# 下载 System.Runtime.CompilerServices.Unsafe 6.0.0
$unsafeUrl = "https://www.nuget.org/api/v2/package/System.Runtime.CompilerServices.Unsafe/6.0.0"
$unsafeZip = "$packageDir\System.Runtime.CompilerServices.Unsafe.6.0.0.zip"
Invoke-WebRequest -Uri $unsafeUrl -OutFile $unsafeZip
Write-Host "下载 System.Runtime.CompilerServices.Unsafe 完成" -ForegroundColor Yellow

# 解压并提取 DLL
Write-Host "`n正在提取 DLL 文件..." -ForegroundColor Green

# 提取 Microsoft.Bcl.AsyncInterfaces.dll
$asyncInterfacesExtractPath = "$packageDir\Microsoft.Bcl.AsyncInterfaces"
Expand-Archive -Path $asyncInterfacesZip -DestinationPath $asyncInterfacesExtractPath -Force
$asyncInterfacesDll = "$asyncInterfacesExtractPath\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll"

if (Test-Path $asyncInterfacesDll) {
    Copy-Item $asyncInterfacesDll -Destination $commonDir -Force
    Write-Host "已复制 Microsoft.Bcl.AsyncInterfaces.dll 到 common 文件夹" -ForegroundColor Cyan
} else {
    Write-Host "警告: 找不到 Microsoft.Bcl.AsyncInterfaces.dll" -ForegroundColor Red
}

# 提取 System.Runtime.CompilerServices.Unsafe.dll
$unsafeExtractPath = "$packageDir\System.Runtime.CompilerServices.Unsafe"
Expand-Archive -Path $unsafeZip -DestinationPath $unsafeExtractPath -Force
$unsafeDll = "$unsafeExtractPath\lib\netstandard2.0\System.Runtime.CompilerServices.Unsafe.dll"

if (Test-Path $unsafeDll) {
    Copy-Item $unsafeDll -Destination $commonDir -Force
    Write-Host "已复制 System.Runtime.CompilerServices.Unsafe.dll 到 common 文件夹" -ForegroundColor Cyan
} else {
    Write-Host "警告: 找不到 System.Runtime.CompilerServices.Unsafe.dll" -ForegroundColor Red
}

Write-Host "`n完成! 依赖项已安装到 common 文件夹" -ForegroundColor Green
Write-Host "`n请重新编译解决方案，所有依赖项将会被复制到输出目录。" -ForegroundColor Yellow

# 列出 common 文件夹中的 DLL
Write-Host "`ncommon 文件夹中的 DLL:" -ForegroundColor Green
Get-ChildItem -Path $commonDir -Filter *.dll | Select-Object Name | Format-Table
