# 資料庫存取改用 Dapper — 階段二

階段一把三個 `Table<T>` 各自改成 Dapper 參數化。階段二把實作收斂成一份、讓 `Insert` 產生欄位清單、校正 Model，並加上交易支援。

## 變更清單

共用實作放在**新的 `DB_Core` 專案**，`ASILib` 完全沒有改動。

| 檔案 | 變更 |
| --- | --- |
| `DB_Core/DB_Core.csproj` | **新專案**。net472 類別庫，只相依 Dapper 與 BCL，不參考 ASILib |
| `DB_Core/TableBase.cs` | **新增**。三個資料庫專案共用的 `TableBase<TModel, TOptions>`，原本三份幾乎相同的實作收斂到這裡。命名空間 `ASI.Wanda.DB` |
| `DB_Core/ITableOptions.cs` | **新增**。各專案提供連線字串、稽核使用者、是否啟用資料庫、連線前置檢查 |
| `DB_Core/DbTransactionScope.cs` | **新增**。以執行緒為界的交易範圍 |
| `DMDService.sln` | 加入 `DB_Core`，放在 `DB` 方案資料夾下 |
| `DMD_DB` / `CMFT_DB` / `DCU_DB` 的 `Tables/Table.cs` | 各自只剩一個 Options 類別 + `abstract class Table<T> : ASI.Wanda.DB.TableBase<T, XxxTableOptions>`，從 360～430 行縮到 30～50 行 |
| `DMD_DB` / `CMFT_DB` / `DCU_DB` 的 `.csproj` | 加上 `DB_Core` 的 `ProjectReference` |
| `TaskOCS` / `TaskCMFT` / `TaskDCU` / `DMDService.Services` / `UITest` 的 `.csproj` | 加上 `DB_Core` 的 `ProjectReference`（見下方「使用 Table wrapper 的專案都要參考 DB_Core」）|
| 三個 `Manager.cs` | 加上 `BeginTransaction()` |
| `DMD_DB/Models/Train.cs`、`Models/System.cs` | 校正四個 Model（見下） |
| `DMD_DB/Tables/System.cs` | `UpdateEquipStatus` 的引數順序配合 Model 重排 |
| `TaskCMFT/Helper.cs` | 9 個「先刪再寫」的同步區塊包進交易範圍 |
| `ASILib` | **未改動**。有 13 個專案參考它、又有自己的 `ASILib.sln`，所以共用實作另立專案而不放進去 |

三份 `Table.cs` 的淨變化：`-1240 / +387` 行。

## Insert 改為產生欄位清單

舊版：

```sql
insert into dbo.dmd_group
values ('...', '...', '...', 'admin', clock_timestamp(), 'admin', clock_timestamp());
```

完全靠 `params object[]` 的位置對應實體資料表的欄位順序，而且無條件附加四個稽核值。

新版：

```sql
insert into dbo.dmd_group (group_id, group_name, group_description, ins_user, ins_time, upd_user, upd_time)
values (@p0, @p1, @p2, @audit_user, clock_timestamp(), @audit_user, clock_timestamp());
```

欄位名取自 Model 的屬性宣告順序（扣掉稽核欄位），**稽核欄位則依 Model 實際宣告了哪幾個自動決定**。這一點解掉了階段一列出的兩類問題：

- `ocs_data`、`train_location`、`line_operation` 的 Model 沒有稽核屬性 → 不再附加四個值，`Insert()` 從「必定失敗」變成可用。
- `platform_conf`、`station_conf` 只有 `ins_*`，`line_conf` 只有 `upd_*` → 各自只補上自己有的欄位。
- `SelectAll()` / `SelectWhere()` 的 `order by ins_time` 改成「Model 有 `ins_time` 才加」，這三張表不再因為欄位不存在而失敗。

時間欄位若在 Model 中是 `string` 型別（`platform_conf.ins_time`、`station_conf.ins_time`、`line_conf.upd_time` 的資料表欄位都是 `character varying`），產生的是 `cast(clock_timestamp() as text)`，寫出來的格式與現有資料一致（`2024-01-02 14:14:09.203071+08`）。

## 交易支援

```csharp
using (var scope = ASI.Wanda.DMD.DB.Manager.BeginTransaction())
{
    dmdPlayList.DeletePlayingItem(stationID, areaID, deviceID);
    dmdPlayList.InsertPlayingItem(...);
    scope.Complete();          // 沒呼叫就離開 using 會自動 Rollback
}
```

範圍一旦開啟，同一執行緒上針對**同一個資料庫**的所有 Table 操作都會自動沿用該連線與交易，所以 83 個 wrapper 完全不用改。

限制：

- 以執行緒為界（`ThreadStatic`），跨執行緒或 `await` 之後不會延續。
- 同一個資料庫不支援巢狀交易，巢狀開啟會直接拋例外。
- DMD / CMFT / DCU 各自獨立，可以同時開，但不是同一筆交易。

`TaskCMFT/Helper.cs` 裡 9 個「先 `DeleteXxx` 全部再 `InsertXxx` 全部」的同步方法已經套用。原本如果在中途失敗，資料表會停在「刪掉了但沒寫回去」的狀態。

## Model 校正（僅 DMD_DB）

依 `DMDDB.sql` 還原出的實際 schema 比對：

| Model | 問題 | 處理 |
| --- | --- | --- |
| `sys_equip_status` | Model 14 個屬性、資料表 15 個欄位；`equip_type` 位置錯、缺 `is_overview` | 依資料表順序重排並補上 `is_overview` |
| `platform_conf` | Model 寫 `upd_user` / `upd_time`，資料表其實是 `ins_user` / `ins_time` | 改名，型別改為 `string`（欄位是 varchar） |
| `station_conf` | 同上 | 同上 |
| `line_conf` | `upd_time` 宣告為 `DateTime`，資料表是 `character varying(50)` | 改為 `string` |

`line_conf` / `platform_conf` / `station_conf` 這三個型別不符，在階段一之前是被舊版「型別不完全相等就靜默略過」的邏輯吃掉的；改用 Dapper 之後讀取這些表會直接拋例外，所以這次一併修掉。目前沒有任何呼叫端使用這幾張表，屬於預先排雷。

`sys_equip_status` 重排會影響 `Update` 的位置對應，`DMD_DB/Tables/System.cs` 的 `UpdateEquipStatus` 引數順序已同步調整。CMFT_DB 的同名 Model 本來就是正確的 15 欄版本，DCU_DB 的則與舊版 DMD_DB 相同（未在本次範圍內）。

## 使用 Table wrapper 的專案都要參考 DB_Core

.NET Framework 的傳統 csproj，**專案參考不會遞移到編譯階段**。`dmdPlayList` 之類的 wrapper 繼承自 `ASI.Wanda.DB.TableBase<>`，任何碰到這些型別的程式碼，編譯器都必須看得到 `DB_Core`——只參考 `DMD_DB` 是不夠的，會得到：

```
error CS0012: 類型 'ASI.Wanda.DB.TableBase<T, DmdTableOptions>' 定義於未被參考的組件中，
              請考慮加入對組件 'DB_Core' 的參考
```

而在 Visual Studio 的錯誤清單裡，這個錯誤往往被下游專案的「**找不到中繼資料檔 'TaskOCS.exe'**」蓋過去——後者只是因為 TaskOCS 沒編出來而產生的連鎖症狀，不是真正的原因。

共用實作原本放在 ASILib 時沒有這個問題，因為每個專案本來就都參考 ASILib。搬到 `DB_Core` 之後，下列五個專案都補上了 `ProjectReference`：`TaskOCS`、`TaskCMFT`、`TaskDCU`、`DMDService.Services`、`UITest`。**日後新增會用到 Table wrapper 的專案，也要記得加。**

## OCS 存取改走共用實作

`DMD_DB/Tables/Train.cs` 的 `ocsData` 原本有一段手寫的 34 欄 `update` 語句（`string.Format` 內嵌），以及一個把 where 子句當欄位值傳的 `updatePlatform_ID(int)`。現在四個方法都走共用實作：

```csharp
static public OCS_Data SelectByPlatformID(int platform_id)
{
    return SelectWhere("where platform_id = @platform_id", new { platform_id }).FirstOrDefault();
}

static public int InsertOCSData(OCS_Data data)
{
    return Insert(data);
}

static public int UpdateOCSDataByPlatformID(int platform_id, OCS_Data data)
{
    return UpdateWhere(data, "where platform_id = @platform_id", new { platform_id });
}
```

為此在 `TableBase` 增加兩個以 Model 為輸入的多載，欄位清單一律由 Model 產生，呼叫端不必手動維護：

- `Insert(TModel model)`
- `UpdateWhere(TModel model, string where, object whereParam = null)`

`where` 條件刻意保留成 `platform_id` 單一欄位（沒有改用主鍵版的 `Update()`），因為 `OCS_Data` 這個 Model 根本沒有標 `[Key]`，而且 upsert 本來就是以 `platform_id` 認人。壞掉又沒有呼叫端的 `updatePlatform_ID(int)` 直接移除。

`Tables/Train.cs` 從 247 行縮到 126 行；`TaskOCS` 對外呼叫的 `InsertOrUpdateOCSData(OCS_Data)` 簽章不變。

## Dapper 引用方式

你在階段一之後用 NuGet 幫三個 DB 專案裝了 Dapper 2.1.79（`PackageReference`），所以這次把引用方式統一過去：

- 移除 `DMD_DB`、`CMFT_DB` 裡指向 `..\common\Dapper.dll` 的舊 `<Reference>`（那個檔案並不存在，會讓建置失敗）。
- `DB_Core` 用同樣的 `PackageReference Include="Dapper" Version="2.1.79"`。
- `DownloadDependencies.ps1` 已還原成原始內容（階段一加的 Dapper 下載段落不再需要）。

`_to_delete/` 資料夾放的是這次汰除的檔案（共用實作曾經短暫放在 `ASILib/DB/` 的那三份），確認建置沒問題後整個刪掉即可。

## 驗證紀錄

1. **編譯驗證**：以 Dapper / Npgsql 簽章 stub 編譯 `DB_Core` 的三個檔案，再用產出的組件編譯三個 DB 專案的全部 Model 與 Table wrapper — 皆 0 error。驗證的是實際寫回磁碟的檔案，不是容器內的副本。
2. **相依性驗證**：`DB_Core` 只給 Dapper 的 stub（不給任何 `ASI.Lib.*`）也能編譯通過，確認它對 ASILib 零相依。
3. **繼承驗證**：確認 wrapper 仍能呼叫到基底的 `static protected` 成員，以及 `Table<sys_alarm>.eSortWay.Desc` 這種透過衍生泛型類別存取巢狀列舉的寫法可以編譯。
4. **SQL 驗證**：用 `DMDDB.sql` 還原真實 schema，把新版產生的 Insert / Update / Select / Delete / SelectAll 全部送進 PostgreSQL `PREPARE` — **87 個案例全數通過**（階段一是 52 通過 / 18 失敗）。
5. **實際資料驗證**：對先前必定失敗的 `ocs_data`（無稽核欄位）、`platform_conf`（varchar 時間欄位）、`line_conf`（只有 `upd_*`）實際寫入成功，並確認時間字串格式與既有資料一致；另驗證交易 rollback 行為正確。
6. **語法驗證**：`TaskCMFT/Helper.cs` 包入 9 個交易範圍後大括號仍平衡（78 → 87 對），編譯器只回報缺少專案參考的 CS0234，沒有任何 parser 錯誤。
7. **OCS 改寫後的驗證**：`DB_Core`、`DMD_DB`、`TaskOCS`（實際呼叫 `InsertOrUpdateOCSData` 的地方）重新編譯 0 error；新的 update 語句對真實 `ocs_data` 做 `PREPARE` 通過；並實際跑了一輪 Insert → Select → Update → Select，確認 34 個欄位都寫到正確位置。

## 待處理

- **`dbo.platform_conf.ins_time` 是 `character varying(18)`**，放不下時間字串（其他同類欄位都是 50）。這張表目前沒有資料也沒有呼叫端，但要讓 `Insert()` 能用，需要：
  ```sql
  ALTER TABLE dbo.platform_conf ALTER COLUMN ins_time TYPE character varying(50);
  ```
- **CMFT_DB / DCU_DB 的 Model 尚未對照實際 schema**。若能提供這兩個資料庫的 `pg_dump`，可以用同一套方式比對。DCU_DB 的 `sys_equip_status` 目前與舊版 DMD_DB 相同，很可能有同樣的欄位順序問題。
- **CMFT 每次查詢前都會 Ping 一次資料庫主機**（1 秒 timeout），行為沿用舊版未動。若 TaskCMFT 有查詢量大的路徑，這是明顯的延遲來源，值得評估改成連線失敗時才觸發。
- **`DMD_DB/Tables/OCS.cs` 整支是死碼**。`ASI.Wanda.DMD.DB.Tables.OCS.ocsData` 對應的是 `Models/OCS.cs` 的 `ocs_data`，但實際在跑的是 `Tables.Train.ocsData`（對應 `Models/Train.cs` 的 `OCS_Data`）。兩個 Model 的 34 個屬性名稱完全一樣，只差在 `ocs_data` 有標 `[Key]`、`OCS_Data` 沒有。建議擇一保留。
- **`Tables/Train.cs` 的 `trainMessage`** 還有兩個壞掉的方法：`UpdateTrain_MSG(int)` 和 `selectAddressID(int)` 都把 where 子句字串當成欄位值傳進 `Update()` / `Select()`；`InsertTrain_MSG` 收了 `platform_id` 卻沒用到。目前都沒有呼叫端。

## 建置步驟

1. 開啟 `DMDService.sln`，確認方案的 `DB` 資料夾下出現 `DB_Core` 專案（方案檔已經加好，不需手動 Add Existing Project）。
2. 對方案執行「還原 NuGet 套件」（或 `nuget restore DMDService.sln`），確認 `DB_Core` 也取得 Dapper 2.1.79。
3. 重新建置整個方案。ASILib 沒有變動，不受影響的專案不需重編。
   若出現「找不到中繼資料檔 'XXX.exe'」，先把錯誤清單依專案排序、看**第一個**真正的編譯錯誤，那類訊息通常只是下游的連鎖症狀。
4. 用 UITest 驗證 TaskCMFT / TaskDCU / TaskOCS 的資料庫操作，特別是 TaskCMFT 的同步流程（交易範圍是這次唯一會改變執行時期行為的部分）。
