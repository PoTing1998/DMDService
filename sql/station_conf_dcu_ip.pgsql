-- =====================================================================
-- 【注意】本檔為 PostgreSQL 語法 (ON CONFLICT / SET client_encoding)，
--         非 T-SQL。副檔名使用 .pgsql 以避免 Visual Studio 套用
--         T-SQL 剖析器而誤報 SQL80001。請以 psql / pgAdmin 執行。
-- =====================================================================
-- =====================================================================
-- station_conf：補齊萬大線各車站 DCU 設定 (LG01 ~ LG08A)
--
-- 用途：TaskDCU 以 dbo.station_conf.dcu_ip 判斷「車站 <-> DCU 連線」對照
--       (DMD_DB/Tables/Train.cs：GetDcuIP / GetStationIdByDcuIp / GetAllDcuIP)
--       Config.xml 內的 Station 設定僅供人工對照，程式實際讀的是這張表。
--
-- 主鍵：(station_id, line_id)
-- 本腳本可重複執行 (idempotent)：
--   * 該站不存在 -> 新增整筆
--   * 該站已存在 -> 只更新 dcu_ip / in_use，不覆蓋既有中英文站名與備註
--
-- 執行方式：
--   psql -h 10.104.26.11 -p 5432 -U postgres -d DMDDB -f station_conf_dcu_ip.pgsql
-- =====================================================================

SET client_encoding = 'UTF8';

BEGIN;

INSERT INTO dbo.station_conf
    (station_id, line_id, platform_address_up, platform_address_dn,
     english, chinese, in_use, dcu_ip, remark, ins_user, ins_time)
VALUES
    ('LG01' , 'WANDA', 30001, 30101, 'Chiang Kai-Shek Memorial Hall', '中正紀念堂'      , 'Y', '10.104.17.20', 'LG01' , 'DMDSERVER', now()),
    ('LG02' , 'WANDA', 30201, 30301, 'Taipei Botanical Garden'      , '植物園(國語實小)', 'Y', '10.104.18.20', 'LG02' , 'DMDSERVER', now()),
    ('LG03' , 'WANDA', 30401, 30501, 'Xiaan'                        , '廈安'            , 'Y', '10.104.19.20', 'LG03' , 'DMDSERVER', now()),
    ('LG04' , 'WANDA', 30601, 30701, 'Kalah'                        , '加蚋'            , 'Y', '10.104.20.20', 'LG04' , 'DMDSERVER', now()),
    ('LG05' , 'WANDA', 30801, 30901, 'Yonghe'                       , '永和永平國小'    , 'Y', '10.104.21.20', 'LG05' , 'DMDSERVER', now()),
    ('LG06' , 'WANDA', 31001, 31101, 'Zhonghe'                      , '中和'            , 'Y', '10.104.22.20', 'LG06' , 'DMDSERVER', now()),
    ('LG07' , 'WANDA', 31201, 31301, 'Shuang Ho Hospital'           , '雙和醫院'        , 'Y', '10.104.23.20', 'LG07' , 'DMDSERVER', now()),
    ('LG08' , 'WANDA', 31401, 31501, 'Zhonghe Senior High School'   , '中和高中'        , 'Y', '10.104.24.20', 'LG08' , 'DMDSERVER', now()),
    ('LG08A', 'WANDA', 31601, 31701, 'Juguang'                      , '莒光'            , 'Y', '10.104.25.20', 'LG08A', 'DMDSERVER', now())
ON CONFLICT (station_id, line_id) DO UPDATE
    SET dcu_ip = EXCLUDED.dcu_ip,
        in_use = 'Y';

COMMIT;

-- 驗證：應為 9 筆，dcu_ip 依序 10.104.17.20 ~ 10.104.25.20
SELECT station_id, line_id, chinese, dcu_ip, in_use
FROM   dbo.station_conf
WHERE  line_id = 'WANDA'
ORDER  BY station_id;
