# 資料庫存取改用 Dapper — 階段一

## 範圍

只重寫三個 `Table<T>` 抽象基底的內部實作，改用 Dapper 執行參數化 SQL。
**對外方法簽章完全不變**，83 個 table wrapper 類別與 6 個專案的 73 處呼叫點一行都沒有改。

| 檔案 | 變更 |
| --- | --- |
| `DMD_DB/Tables/Table.cs` | 全檔改寫 |
| `CMFT_DB/Tables/Table.cs` | 全檔改寫（保留 `IsUseDatabase` 閘門、Ping 檢查、`UpdateWhere`） |
| `DCU_DB/Tables/Table.cs` | 全檔改寫 |
| `DMD_DB/CMFT_DB/DCU_DB` 的 `.csproj` | 加入 `..\common\Dapper.dll` 引用 |
| `DownloadDependencies.ps1` | 加入 Dapper 下載段落（預設 2.1.35） |

## 導入步驟

1. 在 repo 根目錄執行 `.\DownloadDependencies.ps1`，確認 `common\Dapper.dll` 產生成功。
2. 重新建置整個方案（`msbuild DMDService.sln /p:Configuration=Debug`）。
3. 用 UITest 逐一驗證 TaskCMFT / TaskDCU / TaskOCS 的資料庫操作。

## 修正的問題

| 問題 | 舊版 | 新版 |
| --- | --- | --- |
| SQL Injection / 單引號炸裂 | 值一律 `string.Format` 內嵌 | 一律以參數送出 |
| 參數化功能失效 | `SqlParameter[]`（SqlClient 型別）塞進 `NpgsqlCommand`，實際上不可用 | 第二個參數改為 `object`，可傳匿名型別或 `DynamicParameters` |
| 欄位對映靜默失敗 | `_modelColType == _tableValueType` 嚴格比對，型別不完全相等就跳過，資料悄悄變成預設值 | 交由 Dapper 對映（大小寫不敏感），對不上會明確拋例外 |
| `SelectAll(Desc)` 產生 `order by ins_time dasc` | DMD_DB / DCU_DB 的 Desc 路徑必然語法錯誤 | 修正為 `desc` |
| DateTime 毫秒被截掉 | 格式化成 `yyyy/MM/dd HH:mm:ss` 字串 | 直接以參數送出，保留完整精度 |
| `Update` 傳入 null 會 NRE | `paramObject.GetType()` 直接炸 | 產生 `欄位 = null` |
| 主鍵條件組不出來時產生亂碼 SQL | `where` 被 `Substring` 切成 `wh` | 拋出明確的 `InvalidOperationException` |

## 刻意維持不變的部分

這些屬於階段二的範圍，本次沒有動：

- **識別字不加雙引號**。Model 之中存在 `OCS_Data`、`countdown_display_Interval` 這類與實際欄位大小寫不一致的名稱，實測 `select * from dbo.OCS_Data` 可以命中 `ocs_data`，但 `dbo."OCS_Data"` 會找不到物件。因此沿用 PostgreSQL 自動折成小寫的行為。
- **`Insert` 不產生欄位清單**，仍以 `params object[]` 的位置對應實體資料表的欄位順序。
- **`Insert` / `Update` 固定附加** `ins_user` / `ins_time` / `upd_user` / `upd_time`。
- **CMFT 每次查詢前都會 Ping 一次資料庫主機**（1 秒 timeout）。這對高頻查詢是明顯的效能負擔，但屬於既有行為，沒有一併移除。

## 新增的參數化用法

舊的字串拼接呼叫全部照舊可用，新程式碼建議改走這三個多載：

```csharp
// 查詢
SelectWhere("where station_id = @station_id", new { station_id = stationID });

// 刪除
DeleteWhere("where group_id = @group_id", new { group_id = groupID });

// 更新指定欄位（CMFT_DB）
UpdateWhere(columnVals, "where alarm_id = @alarm_id", new { alarm_id = alarmID });
```

## 驗證紀錄

1. **編譯驗證**：以 Dapper / Npgsql 的 stub（簽章比照官方 API）搭配三個 DB 專案的全部 Model 與 Table wrapper 原始碼編譯，三個專案皆 0 error。
2. **多載解析驗證**：確認既有的 `SelectWhere(where, eSortWay.Desc)` 呼叫仍然繫結到 `(string, eSortWay)` 多載、排序方向正確，不會被新的 `(string, object, eSortWay)` 多載攔截。
3. **SQL 語法驗證**：用 `DMDDB.sql` 還原出真實 schema，將新版產生的 Insert / Update / Select / SelectAll 全部送進 PostgreSQL `PREPARE`，70 個案例中 52 個通過。
4. **實際資料驗證**：對 `dmd_instant_message` 跑完整的 Insert → Update → Select → Delete，內容使用 `O'Brien 的 100% 測試`。舊版的字串內嵌寫法在同一筆資料上直接 `syntax error at or near "Brien"`，新版正常寫入與讀回。
5. **保留字驗證**：`interval`、`index` 這兩個欄位在不加引號的 `set` 子句中可正常運作。

## 既有問題（本次未修，但已確認）

以下是拿真實 schema 比對後發現的，**改用 Dapper 之前就存在**，新舊版行為相同（已實測舊版產生同樣的錯誤）：

### 1. 七張資料表沒有完整的稽核欄位，`Insert()` / `SelectAll()` 必定失敗

`ocs_data`、`line_conf`、`line_operation`、`train_location`、`train_movement_info`、`platform_conf`、`station_conf`。

- `Insert()` 固定附加四個稽核值 → `INSERT has more expressions than target columns`
- `SelectAll()` / `SelectWhere()` 固定 `order by ins_time` → `column "ins_time" does not exist`

實務上這些表都是走 `Tables/Train.cs` 裡自己拼 SQL 的 `InsertOCSData()` / `UpdateOCSDataByPlatformID()` 之類的方法，所以沒有被踩到。要透過 `Table<T>` 操作這些表，得等階段二把欄位清單做出來。

### 2. 三個 Model 的屬性順序與實際欄位順序不符

`Insert()` 靠位置對應欄位，順序不符就會寫進錯的欄位：

| Model | 情況 |
| --- | --- |
| `sys_equip_status` | Model 有 14 個屬性、資料表有 15 個欄位，從位置 1 開始就整排錯開（Model 少了 `equip_type` 之後的對齊，且缺 `is_overview`） |
| `platform_conf` | Model 的 `upd_user` / `upd_time` 對到資料表的 `ins_user` / `ins_time` |
| `station_conf` | 同上 |

建議在階段二一併校正，或至少先確認這幾張表沒有走 `Table<T>.Insert()`。

## 已知風險

- **無法在此環境編譯真正的 .NET Framework 專案**，上述編譯驗證是以 stub 進行的。實際建置請在 Visual Studio 執行一次完整 rebuild。
- 舊版「型別不完全相等就靜默略過欄位」的行為被移除後，原本被藏起來的 Model 與 schema 型別不一致會變成明確的例外浮出來。這是預期中的，導入時請預留除錯時間。
- Dapper 版本固定在 2.1.35。若要換版本，改 `DownloadDependencies.ps1` 裡的 `$dapperVersion` 即可，腳本會自動挑選可用的目標框架資料夾。

## 下一步（階段二）

1. 由 Model metadata 產生 `Insert` 的欄位清單，解掉位置對應的風險。
2. 校正 `sys_equip_status` / `platform_conf` / `station_conf` 的 Model 屬性順序。
3. 讓稽核欄位變成可選（依資料表實際有無決定是否附加）。
4. 把三份幾乎相同的 `Table<T>` 收斂成一份共用實作。
5. 為多表寫入（CMFT → DMD 同步）加上 transaction。
