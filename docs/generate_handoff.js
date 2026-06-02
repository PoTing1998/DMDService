const fs = require("fs");
const {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  Header, Footer, AlignmentType, HeadingLevel, BorderStyle, WidthType,
  ShadingType, PageNumber, PageBreak, LevelFormat, TabStopType, TabStopPosition,
} = require("docx");

const border = { style: BorderStyle.SINGLE, size: 1, color: "999999" };
const borders = { top: border, bottom: border, left: border, right: border };
const cellMargins = { top: 60, bottom: 60, left: 100, right: 100 };
const PAGE_WIDTH = 12240;
const MARGIN = 1440;
const CONTENT_WIDTH = PAGE_WIDTH - 2 * MARGIN; // 9360

function headerCell(text, width) {
  return new TableCell({
    borders,
    width: { size: width, type: WidthType.DXA },
    shading: { fill: "2B579A", type: ShadingType.CLEAR },
    margins: cellMargins,
    children: [new Paragraph({ children: [new TextRun({ text, bold: true, color: "FFFFFF", font: "Microsoft JhengHei", size: 20 })] })],
  });
}

function cell(text, width) {
  return new TableCell({
    borders,
    width: { size: width, type: WidthType.DXA },
    margins: cellMargins,
    children: [new Paragraph({ children: [new TextRun({ text, font: "Microsoft JhengHei", size: 20 })] })],
  });
}

function h1(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_1,
    spacing: { before: 360, after: 200 },
    children: [new TextRun({ text, bold: true, font: "Microsoft JhengHei", size: 32, color: "2B579A" })],
  });
}

function h2(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_2,
    spacing: { before: 240, after: 120 },
    children: [new TextRun({ text, bold: true, font: "Microsoft JhengHei", size: 26, color: "2B579A" })],
  });
}

function h3(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_3,
    spacing: { before: 200, after: 100 },
    children: [new TextRun({ text, bold: true, font: "Microsoft JhengHei", size: 22, color: "404040" })],
  });
}

function p(text) {
  return new Paragraph({
    spacing: { after: 120 },
    children: [new TextRun({ text, font: "Microsoft JhengHei", size: 20 })],
  });
}

function bullet(text, ref, level) {
  return new Paragraph({
    numbering: { reference: ref, level: level || 0 },
    spacing: { after: 60 },
    children: [new TextRun({ text, font: "Microsoft JhengHei", size: 20 })],
  });
}

function boldP(boldText, normalText) {
  return new Paragraph({
    spacing: { after: 80 },
    children: [
      new TextRun({ text: boldText, bold: true, font: "Microsoft JhengHei", size: 20 }),
      new TextRun({ text: normalText, font: "Microsoft JhengHei", size: 20 }),
    ],
  });
}

function codeBlock(text) {
  return new Paragraph({
    spacing: { before: 60, after: 60 },
    indent: { left: 360 },
    children: [new TextRun({ text, font: "Consolas", size: 18, color: "333333" })],
  });
}

const doc = new Document({
  numbering: {
    config: [
      {
        reference: "bullets",
        levels: [{
          level: 0, format: LevelFormat.BULLET, text: "•", alignment: AlignmentType.LEFT,
          style: { paragraph: { indent: { left: 720, hanging: 360 } } },
        }, {
          level: 1, format: LevelFormat.BULLET, text: "◦", alignment: AlignmentType.LEFT,
          style: { paragraph: { indent: { left: 1080, hanging: 360 } } },
        }],
      },
      {
        reference: "numbers",
        levels: [{
          level: 0, format: LevelFormat.DECIMAL, text: "%1.", alignment: AlignmentType.LEFT,
          style: { paragraph: { indent: { left: 720, hanging: 360 } } },
        }],
      },
    ],
  },
  styles: {
    default: {
      document: { run: { font: "Microsoft JhengHei", size: 20 } },
    },
    paragraphStyles: [
      { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 32, bold: true, font: "Microsoft JhengHei", color: "2B579A" },
        paragraph: { spacing: { before: 360, after: 200 }, outlineLevel: 0 } },
      { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 26, bold: true, font: "Microsoft JhengHei", color: "2B579A" },
        paragraph: { spacing: { before: 240, after: 120 }, outlineLevel: 1 } },
      { id: "Heading3", name: "Heading 3", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 22, bold: true, font: "Microsoft JhengHei", color: "404040" },
        paragraph: { spacing: { before: 200, after: 100 }, outlineLevel: 2 } },
    ],
  },
  sections: [
    // ===== COVER PAGE =====
    {
      properties: {
        page: {
          size: { width: PAGE_WIDTH, height: 15840 },
          margin: { top: 1440, right: 1440, bottom: 1440, left: 1440 },
        },
      },
      children: [
        new Paragraph({ spacing: { before: 3000 } }),
        new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 200 },
          children: [new TextRun({ text: "DMDService", bold: true, font: "Microsoft JhengHei", size: 56, color: "2B579A" })],
        }),
        new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 600 },
          children: [new TextRun({ text: "專案交接文件", font: "Microsoft JhengHei", size: 36, color: "404040" })],
        }),
        new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 100 },
          children: [new TextRun({ text: "動態訊息顯示系統 (Dynamic Message Display)", font: "Microsoft JhengHei", size: 22, color: "666666" })],
        }),
        new Paragraph({ spacing: { before: 1200 } }),
        new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 80 },
          children: [new TextRun({ text: "文件版本：v1.0", font: "Microsoft JhengHei", size: 20, color: "666666" })],
        }),
        new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 80 },
          children: [new TextRun({ text: "建立日期：2026-06-02", font: "Microsoft JhengHei", size: 20, color: "666666" })],
        }),
        new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 80 },
          children: [new TextRun({ text: "技術栈：.NET Framework 4.7.2 / WinForms / PostgreSQL", font: "Microsoft JhengHei", size: 20, color: "666666" })],
        }),
        new Paragraph({ children: [new PageBreak()] }),
      ],
    },
    // ===== MAIN CONTENT =====
    {
      properties: {
        page: {
          size: { width: PAGE_WIDTH, height: 15840 },
          margin: { top: 1440, right: 1440, bottom: 1440, left: 1440 },
        },
      },
      headers: {
        default: new Header({
          children: [new Paragraph({
            border: { bottom: { style: BorderStyle.SINGLE, size: 6, color: "2B579A", space: 1 } },
            children: [
              new TextRun({ text: "DMDService 專案交接文件", font: "Microsoft JhengHei", size: 16, color: "999999" }),
            ],
            tabStops: [{ type: TabStopType.RIGHT, position: TabStopPosition.MAX }],
          })],
        }),
      },
      footers: {
        default: new Footer({
          children: [new Paragraph({
            alignment: AlignmentType.CENTER,
            children: [
              new TextRun({ text: "Page ", font: "Microsoft JhengHei", size: 16, color: "999999" }),
              new TextRun({ children: [PageNumber.CURRENT], font: "Microsoft JhengHei", size: 16, color: "999999" }),
            ],
          })],
        }),
      },
      children: [
        // ===== 1. PROJECT OVERVIEW =====
        h1("1. 專案概述"),
        h2("1.1 專案簡介"),
        p("DMDService 是動態訊息顯示系統（Dynamic Message Display）的服務端應用程式，負責管理與控制軌道運輸系統中各車站的電子顯示看板（DCU）。系統提供以下核心功能："),
        bullet("向 DCU 看板發送預錄訊息與即時訊息", "bullets"),
        bullet("即時訊息的編輯與儲存（中英文內容）", "bullets"),
        bullet("透過 Socket 通訊與 DCU 設備進行即時通訊", "bullets"),
        bullet("資料庫管理（PostgreSQL）—— 訊息、播放清單、設備清單", "bullets"),
        bullet("OCS、CMFT、DCU 等多種任務流程的測試介面", "bullets"),

        h2("1.2 技術栈"),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [2500, 6860],
          rows: [
            new TableRow({ children: [headerCell("項目", 2500), headerCell("說明", 6860)] }),
            new TableRow({ children: [cell("執行環境", 2500), cell(".NET Framework 4.7.2", 6860)] }),
            new TableRow({ children: [cell("UI 框架", 2500), cell("Windows Forms (WinForms)", 6860)] }),
            new TableRow({ children: [cell("資料庫", 2500), cell("PostgreSQL（透過 Npgsql 7.0.2 驅動）", 6860)] }),
            new TableRow({ children: [cell("通訊協定", 2500), cell("TCP Socket（自定 JSON 協定）", 6860)] }),
            new TableRow({ children: [cell("專案格式", 2500), cell("傳統 .csproj（非 SDK-style）", 6860)] }),
            new TableRow({ children: [cell("IDE", 2500), cell("Visual Studio 2022 (v17.9+)", 6860)] }),
            new TableRow({ children: [cell("DLL 管理", 2500), cell("手動管理，放於 common/ 資料夾（非 NuGet）", 6860)] }),
          ],
        }),

        // ===== 2. PROJECT STRUCTURE =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("2. 專案結構"),
        h2("2.1 Solution 結構總覽"),
        p("DMDService.sln 包含以下專案，依功能分為四個群組："),

        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [1800, 2200, 5360],
          rows: [
            new TableRow({ children: [headerCell("群組", 1800), headerCell("專案名稱", 2200), headerCell("說明", 5360)] }),
            new TableRow({ children: [cell("DB", 1800), cell("DMD_DB", 2200), cell("DMD 資料庫存取層（Table<T> 泛型 ORM）", 5360)] }),
            new TableRow({ children: [cell("DB", 1800), cell("CMFT_DB", 2200), cell("CMFT 資料庫存取層", 5360)] }),
            new TableRow({ children: [cell("DB", 1800), cell("DCU_DB", 2200), cell("DCU 資料庫存取層", 5360)] }),
            new TableRow({ children: [cell("Frame", 1800), cell("DMD_Frame", 2200), cell("DMD Socket 通訊層（DMD_API）", 5360)] }),
            new TableRow({ children: [cell("Frame", 1800), cell("CMFT_Frame", 2200), cell("CMFT 通訊層", 5360)] }),
            new TableRow({ children: [cell("Task", 1800), cell("TaskOCS", 2200), cell("OCS 任務流程 UI", 5360)] }),
            new TableRow({ children: [cell("Task", 1800), cell("TaskCMFT", 2200), cell("CMFT 任務流程 UI", 5360)] }),
            new TableRow({ children: [cell("Task", 1800), cell("TaskDCU", 2200), cell("DCU 任務流程 UI", 5360)] }),
            new TableRow({ children: [cell("Server", 1800), cell("ASILib", 2200), cell("共用工具庫（Log、Json 解析、其他工具）", 5360)] }),
            new TableRow({ children: [cell("Server", 1800), cell("TaskKernel", 2200), cell("任務核心邏輯", 5360)] }),
            new TableRow({ children: [cell("Server", 1800), cell("TaskMain", 2200), cell("主程式進入點", 5360)] }),
            new TableRow({ children: [cell("Server", 1800), cell("KernelService", 2200), cell("核心服務層", 5360)] }),
            new TableRow({ children: [cell("UITest", 1800), cell("UITest", 2200), cell("測試用 WinForms 介面（含 SendToDCU）", 5360)] }),
            new TableRow({ children: [cell("-", 1800), cell("DMDService.Services", 2200), cell("商業邏輯 Service 層（新增）", 5360)] }),
          ],
        }),

        h2("2.2 相依關係"),
        p("專案間的主要相依方向如下："),
        codeBlock("UITest"),
        codeBlock("  └── DMDService.Services  (商業邏輯層)"),
        codeBlock("        ├── ASILib              (共用工具)"),
        codeBlock("        ├── DMD_Frame           (Socket 通訊)"),
        codeBlock("        └── DMD_DB              (資料庫存取)"),
        codeBlock("              └── Npgsql 7.0.2   (PostgreSQL 驅動)"),
        p(""),
        codeBlock("UITest 也直接引用："),
        codeBlock("  ├── ASILib, DMD_Frame, DMD_DB"),
        codeBlock("  ├── CMFT_Frame, CMFT_DB, DCU_DB"),
        codeBlock("  └── TaskOCS (尚未重構)"),

        // ===== 3. ARCHITECTURE =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("3. 架構設計"),
        h2("3.1 分層架構"),
        p("系統採用三層式架構，將 UI、商業邏輯、資料存取分離："),

        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [2000, 2500, 4860],
          rows: [
            new TableRow({ children: [headerCell("層次", 2000), headerCell("專案", 2500), headerCell("職責", 4860)] }),
            new TableRow({ children: [cell("UI 層", 2000), cell("UITest", 2500), cell("WinForms 控制項、使用者互動、畫面更新", 4860)] }),
            new TableRow({ children: [cell("Service 層", 2000), cell("DMDService.Services", 2500), cell("DB 查詢、訊息建構、通訊管理、商業邏輯", 4860)] }),
            new TableRow({ children: [cell("Frame 層", 2000), cell("DMD_Frame", 2500), cell("Socket 通訊協定（DMD_API）", 4860)] }),
            new TableRow({ children: [cell("DB 層", 2000), cell("DMD_DB / CMFT_DB / DCU_DB", 2500), cell("資料庫存取（泛型 Table<T> ORM）", 4860)] }),
            new TableRow({ children: [cell("共用層", 2000), cell("ASILib", 2500), cell("日誌、JSON 解析、其他工具", 4860)] }),
          ],
        }),

        h2("3.2 重構說明（SendToDCU 商業邏輯分離）"),
        p("已完成 SendToDCU 的商業邏輯與 UI 分離重構，建立了 DMDService.Services 專案，採用介面抽象與手動建構子注入（Constructor Injection）模式。"),

        h3("重構前"),
        bullet("SendToDCU.cs 包含所有商業邏輯：DB 查詢、JSON 建構、Socket 通訊、Station Enum 轉換", "bullets"),
        bullet("直接在按鈕事件中建立 DMD_API、呼叫 DB、組裝 JSON", "bullets"),
        bullet("無法單元測試、難以複用", "bullets"),

        h3("重構後"),
        bullet("UI 層（SendToDCU.cs）：僅負責控制項操作、讀取用戶輸入、顯示結果", "bullets"),
        bullet("Service 層：JSON 建構、DB 查詢、通訊管理全部抽出到 DmdMessageService / DmdConnectionService", "bullets"),
        bullet("透過介面（IDmdMessageService、IDmdConnectionService）解耦", "bullets"),
        bullet("Composition Root 在 UITest.cs 建構子中建立 Service 並注入", "bullets"),

        h3("注入模式範例（UITest.cs）"),
        codeBlock("// Composition Root"),
        codeBlock("IDmdConnectionService connectionService = new DmdConnectionService();"),
        codeBlock("IDmdMessageService messageService = new DmdMessageService(connectionService);"),
        codeBlock("sendToDCUControl = new SendToDCU(messageService);"),

        // ===== 4. PROJECT DETAILS =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("4. 各專案詳細說明"),

        h2("4.1 ASILib（共用工具庫）"),
        p("提供跨專案共用的基礎功能："),
        bullet("ASI.Lib.Log.ErrorLog / DebugLog — 統一日誌輸出", "bullets"),
        bullet("ASI.Lib.Text.Parsing.Json — JSON 序列化與解析（GetValue / SerializeObject）", "bullets"),
        bullet("其他共用工具類", "bullets"),

        h2("4.2 DMD_Frame（Socket 通訊層）"),
        p("封裝 DMD 系統的 TCP Socket 通訊協定，核心類別為 DMD_API。"),
        h3("DMD_API 主要方法"),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [3500, 5860],
          rows: [
            new TableRow({ children: [headerCell("方法", 3500), headerCell("說明", 5860)] }),
            new TableRow({ children: [cell("Initial(connStr)", 3500), cell("初始化連線，參數格式：IP=x;Port=x;Type=Server|Client", 5860)] }),
            new TableRow({ children: [cell("Send(Message)", 3500), cell("發送訊息，回傳 0 表示成功", 5860)] }),
            new TableRow({ children: [cell("Dispose()", 3500), cell("釋放連線資源", 5860)] }),
          ],
        }),
        h3("DMD_API 主要事件"),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [3500, 5860],
          rows: [
            new TableRow({ children: [headerCell("事件", 3500), headerCell("說明", 5860)] }),
            new TableRow({ children: [cell("ReceivedEvent", 3500), cell("接收到訊息時觸發（ACK / Response / Command）", 5860)] }),
            new TableRow({ children: [cell("OpenedEvent", 3500), cell("連線開啟", 5860)] }),
            new TableRow({ children: [cell("ClosedEvent", 3500), cell("連線關閉", 5860)] }),
            new TableRow({ children: [cell("ErrorEvent", 3500), cell("錯誤發生", 5860)] }),
          ],
        }),

        h2("4.3 DMD_DB（資料庫存取層）"),
        p("基於泛型 Table<T> 抽象類別的反射式 ORM，透過 Npgsql 存取 PostgreSQL。"),
        h3("Table<T> 主要靜態方法"),
        bullet("SelectAll() — 查詢所有記錄", "bullets"),
        bullet("Select(key) — 依主鍵查詢", "bullets"),
        bullet("Insert(entity) / Update(entity) / Delete(key)", "bullets"),
        bullet("SelectWhere(condition) / DeleteWhere(condition)", "bullets"),

        h3("主要 Model 類別"),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [3500, 5860],
          rows: [
            new TableRow({ children: [headerCell("Model", 3500), headerCell("說明", 5860)] }),
            new TableRow({ children: [cell("dmd_pre_record_message", 3500), cell("預錄訊息（message_id, message_content, message_content_en, message_priority, move_speed, move_mode）", 5860)] }),
            new TableRow({ children: [cell("dmd_instant_message", 3500), cell("即時訊息（同上欄位 + font_type/size/color 中英文）", 5860)] }),
            new TableRow({ children: [cell("dmd_play_list", 3500), cell("播放清單（station_id, area_id, device_id）", 5860)] }),
          ],
        }),

        h2("4.4 CMFT_Frame / CMFT_DB / DCU_DB"),
        p("CMFT_Frame 與 DMD_Frame 結構類似，提供 CMFT 系統的 Socket 通訊封裝。"),
        p("CMFT_DB / DCU_DB 與 DMD_DB 採用相同的 Table<T> ORM 模式，分別對應 CMFT 與 DCU 資料庫。"),
        p("所有 DB 專案共用 Npgsql 7.0.2，DLL 放於 common/ 資料夾。"),

        h2("4.5 DMDService.Services（商業邏輯 Service 層）"),
        p("新增的專案，將 SendToDCU 的商業邏輯抽出為可測試、可複用的 Service。"),

        h3("檔案結構"),
        codeBlock("DMDService.Services/"),
        codeBlock("  Interfaces/"),
        codeBlock("    IDmdConnectionService.cs    // DMD_API 連線管理介面"),
        codeBlock("    IDmdMessageService.cs       // 訊息查詢、建構、發送介面"),
        codeBlock("  Services/"),
        codeBlock("    DmdConnectionService.cs     // 實作 DMD_API 生命週期"),
        codeBlock("    DmdMessageService.cs        // 實作 DB 查詢 + JSON 建構 + 發送"),
        codeBlock("  Models/"),
        codeBlock("    SendResult.cs               // 發送結果 DTO"),
        codeBlock("    TargetDevice.cs             // 目標看板 DTO"),

        h3("IDmdConnectionService 介面"),
        bullet("Connect(ip, port, type) — 建立 Socket 連線", "bullets"),
        bullet("Disconnect() — 斷開連線", "bullets"),
        bullet("Send(Message) — 發送訊息", "bullets"),
        bullet("IsConnected — 連線狀態", "bullets"),
        bullet("LogMessage / MessageReceived 事件", "bullets"),

        h3("IDmdMessageService 介面"),
        bullet("InitializeDatabase() — 初始化資料庫連線", "bullets"),
        bullet("GetPreRecordMessages() / GetInstantMessages() — 查詢訊息", "bullets"),
        bullet("GetTargetDevices(stationFilter) — 查詢目標看板（依車站過濾）", "bullets"),
        bullet("SendPreRecordMessage() / SendInstantMessage() — 建構 JSON 並發送", "bullets"),
        bullet("SaveInstantMessage() — 儲存即時訊息編輯", "bullets"),
        bullet("Connect() / Disconnect() — 委派給 ConnectionService", "bullets"),

        // ===== 5. COMMUNICATION PROTOCOL =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("5. 通訊協定"),
        h2("5.1 Message 結構"),
        p("DMD 系統的訊息基於 ASI.Wanda.DMD.Message.Message 類別，包含以下屬性："),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [2500, 2500, 4360],
          rows: [
            new TableRow({ children: [headerCell("屬性", 2500), headerCell("型別", 2500), headerCell("說明", 4360)] }),
            new TableRow({ children: [cell("MessageType", 2500), cell("eMessageType", 2500), cell("Command / Ack / Response", 4360)] }),
            new TableRow({ children: [cell("MessageID", 2500), cell("int", 2500), cell("訊息唯一識別碼（隨機產生）", 4360)] }),
            new TableRow({ children: [cell("JsonContent", 2500), cell("string", 2500), cell("JSON 格式的訊息內容", 4360)] }),
          ],
        }),

        h2("5.2 發送流程"),
        p("發送預錄訊息 / 即時訊息的流程如下："),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "UI 層讀取用戶輸入（車站、訊息 ID、目標看板、參數）", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "呼叫 Service 層的 SendPreRecordMessage / SendInstantMessage", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "Service 層建構 JSON 物件（SendPreRecordMessage / SendInstantMessage JsonObject）", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "序列化為 JSON 字串，建立 Message 物件", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "透過 ConnectionService.Send() 經 DMD_API 發送至 DCU", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "DCU 回傳 ACK，ConnectionService 通過 ReceivedEvent 處理", font: "Microsoft JhengHei", size: 20 })],
        }),

        h2("5.3 JSON 協定範例"),
        h3("SendPreRecordMessage"),
        codeBlock("{"),
        codeBlock('  "JsonObjectName": "SendPreRecordMessage",'),
        codeBlock('  "seatID": "OCC01",'),
        codeBlock('  "msg_id": ["1"],'),
        codeBlock('  "target_du": ["LG01_A01_DU001"],'),
        codeBlock('  "message_priority": 3,'),
        codeBlock('  "move_speed": 3,'),
        codeBlock('  "move_mode": 0'),
        codeBlock("}"),

        h3("SendInstantMessage"),
        codeBlock("{"),
        codeBlock('  "JsonObjectName": "SendInstantMessage",'),
        codeBlock('  "seatID": "OCC01",'),
        codeBlock('  "msg_id": "1",'),
        codeBlock('  "target_du": ["LG01_A01_DU001"],'),
        codeBlock('  "message_priority": 3,'),
        codeBlock('  "move_speed": 3,'),
        codeBlock('  "move_mode": 0'),
        codeBlock("}"),

        // ===== 6. DATABASE =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("6. 資料庫說明"),
        h2("6.1 連線方式"),
        p("透過 Npgsql 7.0.2 連接 PostgreSQL，使用 ASI.Wanda.DMD.DB.Manager.Initializer() 初始化："),
        codeBlock("Manager.Initializer(ip, port, dbName, userId, password, \"DMDServer\")"),

        h2("6.2 主要資料表"),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [3200, 6160],
          rows: [
            new TableRow({ children: [headerCell("資料表", 3200), headerCell("用途", 6160)] }),
            new TableRow({ children: [cell("dmd_pre_record_message", 3200), cell("預錄訊息清單（中英文內容、優先級、移動速度/模式）", 6160)] }),
            new TableRow({ children: [cell("dmd_instant_message", 3200), cell("即時訊息清單（可編輯的中英文內容 + 字型/字級/顏色）", 6160)] }),
            new TableRow({ children: [cell("dmd_play_list", 3200), cell("播放清單 / 目標看板清單（station_id, area_id, device_id）", 6160)] }),
          ],
        }),

        h2("6.3 Npgsql 相依 DLL"),
        p("Npgsql 7.0.2 需要以下 DLL，全部放於 common/ 資料夾："),
        bullet("Npgsql.dll (7.0.2)", "bullets"),
        bullet("Microsoft.Extensions.Logging.Abstractions.dll (6.0.0)", "bullets"),
        bullet("Microsoft.Bcl.HashCode.dll", "bullets"),
        bullet("System.Collections.Immutable.dll", "bullets"),
        bullet("System.Diagnostics.DiagnosticSource.dll", "bullets"),
        bullet("System.Text.Json.dll", "bullets"),
        bullet("System.Text.Encodings.Web.dll", "bullets"),
        bullet("System.Threading.Channels.dll", "bullets"),
        bullet("System.Numerics.Vectors.dll", "bullets"),
        p("注意：所有 DLL 使用 net461/net462 版本以相容 .NET Framework 4.7.2。"),

        // ===== 7. DEV ENVIRONMENT =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("7. 開發環境設定"),
        h2("7.1 前置需求"),
        bullet("Visual Studio 2022 (v17.9+)，安裝 .NET Desktop 開發工作負載", "bullets"),
        bullet(".NET Framework 4.7.2 開發套件（Targeting Pack）", "bullets"),
        bullet("PostgreSQL 資料庫服務器（開發/測試環境）", "bullets"),

        h2("7.2 建置步驟"),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "Clone 專案儲存庫", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "以 Visual Studio 開啟 DMDService.sln", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "確認 common/ 資料夾中所有 DLL 存在（特別是 Npgsql 相依）", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "建置方案（Build Solution，Ctrl+Shift+B）", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "將 UITest 設為啟動專案以運行測試介面", font: "Microsoft JhengHei", size: 20 })],
        }),

        h2("7.3 常見問題"),
        h3("FileNotFoundException: Npgsql 相依 DLL"),
        p("問題：執行時報 FileNotFoundException，缺少 Microsoft.Extensions.Logging.Abstractions 等 DLL。"),
        p("解決：確認 common/ 資料夾中包含所有 Npgsql 相依 DLL（見 6.3 節），並確認各 DB 專案的 .csproj 中有正確的 Reference 與 HintPath。"),

        h3("dotnet add package 無法使用"),
        p("問題：專案使用傳統 .csproj 格式，dotnet CLI 無法直接操作。"),
        p("解決：手動將 DLL 放入 common/ 資料夾，然後在 .csproj 中加入 <Reference Include=\"...\"> 與 <HintPath>。"),

        // ===== 8. UITest =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("8. UITest 測試介面操作指南"),
        h2("8.1 畫面切換"),
        p("UITest 主畫面包含四個按鈕，切換不同的 UserControl："),
        bullet("TaskOCS — OCS 任務測試", "bullets"),
        bullet("TaskCMFT — CMFT 任務測試", "bullets"),
        bullet("TaskDCU — DCU 任務測試", "bullets"),
        bullet("SendToDCU — 發送訊息至 DCU（已重構）", "bullets"),

        h2("8.2 SendToDCU 操作流程"),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "輸入資料庫連線資訊（IP、Port、資料庫名稱、帳號密碼），點擊「連線資料庫」", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "訊息清單與目標看板自動載入", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "設定 Socket 連線參數（IP、Port、Server/Client），點擊「連線」", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "選擇訊息類型（預錄/即時）、選擇訊息、勾選目標看板", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "設定參數（優先級、移動速度、移動模式），點擊「發送」", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "日誌區域顯示發送結果，可匯出為 TXT 或 CSV", font: "Microsoft JhengHei", size: 20 })],
        }),

        h2("8.3 即時訊息編輯"),
        p("在「即時訊息」模式下，可以："),
        bullet("點擊「載入」讀取選中訊息的中英文內容", "bullets"),
        bullet("編輯中英文內容後點擊「儲存」寫回資料庫", "bullets"),
        bullet("儲存後訊息清單自動重新載入", "bullets"),

        // ===== 9. PENDING ITEMS =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("9. 待辦事項與未來規劃"),
        h2("9.1 待重構項目"),
        p("以下 UserControl 尚未完成商業邏輯分離，建議依 SendToDCU 相同模式重構："),
        bullet("TaskOCS — OCS 任務流程", "bullets"),
        bullet("TaskDCU — DCU 任務流程", "bullets"),
        bullet("TaskCMFT — CMFT 任務流程", "bullets"),

        h3("重構模式（參考 SendToDCU）"),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "在 DMDService.Services 專案中建立對應的 Interface 與 Service", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "將 UserControl 中的商業邏輯（DB 查詢、JSON 建構、通訊）搬移至 Service", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "UserControl 保留無參數建構子（Designer 用）+ 參數建構子（注入 Service）", font: "Microsoft JhengHei", size: 20 })],
        }),
        new Paragraph({
          numbering: { reference: "numbers", level: 0 },
          spacing: { after: 60 },
          children: [new TextRun({ text: "在 UITest.cs 的 Composition Root 中建立 Service 並注入", font: "Microsoft JhengHei", size: 20 })],
        }),

        h2("9.2 建議改進方向"),
        bullet("導入 NuGet 套件管理（升級到 PackageReference 格式）取代手動 DLL 管理", "bullets"),
        bullet("考慮導入 DI 容器（如 Microsoft.Extensions.DependencyInjection）簡化 Composition Root", "bullets"),
        bullet("建立單元測試專案，針對 Service 層撰寫測試", "bullets"),
        bullet("統一 Error Handling 與日誌機制", "bullets"),

        // ===== 10. APPENDIX =====
        new Paragraph({ children: [new PageBreak()] }),
        h1("10. 附錄"),
        h2("10.1 車站代碼對應表"),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [2000, 3680, 3680],
          rows: [
            new TableRow({ children: [headerCell("Enum", 2000), headerCell("代碼", 3680), headerCell("說明", 3680)] }),
            new TableRow({ children: [cell("Station.OCC", 2000), cell("OCC", 3680), cell("運控中心", 3680)] }),
            new TableRow({ children: [cell("Station.LG01", 2000), cell("LG01", 3680), cell("車站 1", 3680)] }),
            new TableRow({ children: [cell("Station.LG02", 2000), cell("LG02", 3680), cell("車站 2", 3680)] }),
            new TableRow({ children: [cell("Station.LG03", 2000), cell("LG03", 3680), cell("車站 3", 3680)] }),
            new TableRow({ children: [cell("Station.LG04", 2000), cell("LG04", 3680), cell("車站 4", 3680)] }),
            new TableRow({ children: [cell("Station.LG05", 2000), cell("LG05", 3680), cell("車站 5", 3680)] }),
            new TableRow({ children: [cell("Station.LG06", 2000), cell("LG06", 3680), cell("車站 6", 3680)] }),
            new TableRow({ children: [cell("Station.LG07", 2000), cell("LG07", 3680), cell("車站 7", 3680)] }),
            new TableRow({ children: [cell("Station.LG08", 2000), cell("LG08", 3680), cell("車站 8", 3680)] }),
            new TableRow({ children: [cell("Station.LG08A", 2000), cell("LG08A", 3680), cell("車站 8A", 3680)] }),
          ],
        }),

        h2("10.2 移動模式對應表"),
        new Table({
          width: { size: CONTENT_WIDTH, type: WidthType.DXA },
          columnWidths: [2000, 7360],
          rows: [
            new TableRow({ children: [headerCell("代碼", 2000), headerCell("說明", 7360)] }),
            new TableRow({ children: [cell("0", 2000), cell("立即顯示", 7360)] }),
            new TableRow({ children: [cell("1", 2000), cell("靜態顯示", 7360)] }),
            new TableRow({ children: [cell("2", 2000), cell("上移", 7360)] }),
            new TableRow({ children: [cell("3", 2000), cell("下移", 7360)] }),
            new TableRow({ children: [cell("4", 2000), cell("左移", 7360)] }),
            new TableRow({ children: [cell("5", 2000), cell("右移", 7360)] }),
            new TableRow({ children: [cell("6", 2000), cell("閃爍", 7360)] }),
          ],
        }),

        h2("10.3 優先級與移動速度"),
        p("優先級（message_priority）：1-5，數字越小優先級越高。"),
        p("移動速度（move_speed）：1-5，數字越大速度越快。"),
      ],
    },
  ],
});

Packer.toBuffer(doc).then(buffer => {
  fs.writeFileSync("C:\\Users\\06006800\\source\\repos\\DMDService\\docs\\DMDService_交接文件.docx", buffer);
  console.log("Document created successfully!");
});
