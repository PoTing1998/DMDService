using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ASI.Lib.Comm.Socket.Admin
{
    #region DTO

    /// <summary>
    /// 一個已連線的 Socket Client 資訊
    /// </summary>
    public class SocketClientInfo
    {
        /// <summary>所屬 Server 名稱，例如 CMFT、DCU</summary>
        public string Server { get; set; }

        /// <summary>遠端 IP:Port</summary>
        public string Endpoint { get; set; }

        /// <summary>備註，例如對應車站</summary>
        public string Note { get; set; }

        public DateTime ConnectedAt { get; set; }

        public DateTime? LastReceivedAt { get; set; }

        public long RxBytes { get; set; }

        public int TxCount { get; set; }
    }

    /// <summary>
    /// 連線/斷線/管理動作事件
    /// </summary>
    public class SocketAdminEvent
    {
        public long Seq { get; set; }

        public DateTime Time { get; set; }

        public string Server { get; set; }

        /// <summary>Connected / Disconnected / Kick / Send / Info / Error</summary>
        public string Type { get; set; }

        public string Endpoint { get; set; }

        public string Detail { get; set; }
    }

    /// <summary>
    /// list 指令的回應內容
    /// </summary>
    public class SocketServerStatus
    {
        public string Server { get; set; }

        public string ProcessName { get; set; }

        public string ListenConnStr { get; set; }

        public bool IsOpen { get; set; }

        public DateTime ServiceStartedAt { get; set; }

        public long LatestSeq { get; set; }

        public List<SocketClientInfo> Clients { get; set; } = new List<SocketClientInfo>();
    }

    /// <summary>
    /// 預期會連入的站點定義（例如 station_conf 的 DCU）
    /// </summary>
    public class SocketStationDef
    {
        public string StationId { get; set; }

        public string Name { get; set; }

        /// <summary>站點連入時的來源 IP</summary>
        public string Ip { get; set; }
    }

    /// <summary>
    /// 單一站點目前的連線狀態
    /// </summary>
    public class SocketStationStatus
    {
        public string StationId { get; set; }

        public string Name { get; set; }

        public string Ip { get; set; }

        public bool Online { get; set; }

        /// <summary>同一 IP 目前的連線數，大於 1 通常代表有殘留的舊連線</summary>
        public int ConnectionCount { get; set; }

        /// <summary>目前的連線，最新連入的排第一個</summary>
        public List<SocketClientInfo> Connections { get; set; } = new List<SocketClientInfo>();

        /// <summary>本次服務啟動後，最後一次連入時間</summary>
        public DateTime? LastConnectedAt { get; set; }

        /// <summary>本次服務啟動後，最後一次斷線時間</summary>
        public DateTime? LastDisconnectedAt { get; set; }
    }

    /// <summary>
    /// stations 指令的回應內容
    /// </summary>
    public class SocketStationReport
    {
        public string Server { get; set; }

        public bool IsOpen { get; set; }

        public DateTime ServiceStartedAt { get; set; }

        /// <summary>站點定義讀取失敗時的訊息（仍會回傳上次成功的清單）</summary>
        public string DefinitionError { get; set; }

        public List<SocketStationStatus> Stations { get; set; } = new List<SocketStationStatus>();

        /// <summary>IP 不屬於任何站點的連線</summary>
        public List<SocketClientInfo> UnknownClients { get; set; } = new List<SocketClientInfo>();
    }

    #endregion

    #region Tracker

    /// <summary>
    /// 記錄 Socket Server 目前的 Client 與事件歷史（執行緒安全）。
    /// 由 Task 在 API 的 Connected / Disconnected / 收到資料事件中呼叫。
    /// </summary>
    public class SocketClientTracker
    {
        private readonly object mLock = new object();
        private readonly Dictionary<string, SocketClientInfo> mClients =
            new Dictionary<string, SocketClientInfo>(StringComparer.OrdinalIgnoreCase);
        private readonly LinkedList<SocketAdminEvent> mEvents = new LinkedList<SocketAdminEvent>();
        private readonly int mMaxEvents;
        private long mSeq = 0;
        private readonly Dictionary<string, DateTime> mLastConnectByIp = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, DateTime> mLastDisconnectByIp = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public string ServerName { get; private set; }

        public DateTime StartedAt { get; private set; }

        public SocketClientTracker(string serverName, int maxEvents = 1000)
        {
            ServerName = serverName;
            mMaxEvents = maxEvents > 0 ? maxEvents : 1000;
            StartedAt = DateTime.Now;
        }

        public void OnConnected(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) return;
            lock (mLock)
            {
                mClients[endpoint] = new SocketClientInfo
                {
                    Server = ServerName,
                    Endpoint = endpoint,
                    ConnectedAt = DateTime.Now
                };
                mLastConnectByIp[GetIp(endpoint)] = DateTime.Now;
            }
            AddEvent("Connected", endpoint, null);
        }

        public void OnDisconnected(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) return;
            SocketClientInfo info;
            lock (mLock)
            {
                if (mClients.TryGetValue(endpoint, out info))
                    mClients.Remove(endpoint);
                mLastDisconnectByIp[GetIp(endpoint)] = DateTime.Now;
            }

            string detail = null;
            if (info != null)
            {
                var duration = DateTime.Now - info.ConnectedAt;
                detail = $"連線時長 {FormatDuration(duration)}，收到 {info.RxBytes} bytes";
                if (!string.IsNullOrEmpty(info.Note)) detail = $"[{info.Note}] " + detail;
            }
            AddEvent("Disconnected", endpoint, detail);
        }

        public void OnReceived(string endpoint, int length)
        {
            if (string.IsNullOrEmpty(endpoint)) return;
            lock (mLock)
            {
                SocketClientInfo info;
                if (mClients.TryGetValue(endpoint, out info))
                {
                    info.LastReceivedAt = DateTime.Now;
                    info.RxBytes += length;
                }
            }
        }

        public void OnSent(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) return;
            lock (mLock)
            {
                SocketClientInfo info;
                if (mClients.TryGetValue(endpoint, out info))
                    info.TxCount++;
            }
        }

        public void SetNote(string endpoint, string note)
        {
            if (string.IsNullOrEmpty(endpoint)) return;
            lock (mLock)
            {
                SocketClientInfo info;
                if (mClients.TryGetValue(endpoint, out info))
                    info.Note = note;
            }
        }

        /// <summary>
        /// Socket Server 重新開啟時，舊的 Client 都已被關閉，清除清單
        /// </summary>
        public void ClearClients(string reason)
        {
            int count;
            lock (mLock)
            {
                count = mClients.Count;
                mClients.Clear();
            }
            if (count > 0)
                AddEvent("Info", null, $"清除 {count} 筆連線記錄：{reason}");
        }

        public void AddEvent(string type, string endpoint, string detail)
        {
            lock (mLock)
            {
                mSeq++;
                mEvents.AddLast(new SocketAdminEvent
                {
                    Seq = mSeq,
                    Time = DateTime.Now,
                    Server = ServerName,
                    Type = type,
                    Endpoint = endpoint,
                    Detail = detail
                });
                while (mEvents.Count > mMaxEvents)
                    mEvents.RemoveFirst();
            }
        }

        public long LatestSeq
        {
            get { lock (mLock) { return mSeq; } }
        }

        public List<SocketClientInfo> GetClients()
        {
            lock (mLock)
            {
                return mClients.Values
                    .OrderBy(c => c.ConnectedAt)
                    .Select(c => new SocketClientInfo
                    {
                        Server = c.Server,
                        Endpoint = c.Endpoint,
                        Note = c.Note,
                        ConnectedAt = c.ConnectedAt,
                        LastReceivedAt = c.LastReceivedAt,
                        RxBytes = c.RxBytes,
                        TxCount = c.TxCount
                    })
                    .ToList();
            }
        }

        public List<SocketAdminEvent> GetEvents(long sinceSeq)
        {
            lock (mLock)
            {
                return mEvents.Where(e => e.Seq > sinceSeq).ToList();
            }
        }

        /// <summary>
        /// 取得某 IP 在本次服務啟動後的最後連入 / 斷線時間
        /// </summary>
        public void GetIpHistory(string ip, out DateTime? lastConnected, out DateTime? lastDisconnected)
        {
            lock (mLock)
            {
                DateTime t;
                lastConnected = mLastConnectByIp.TryGetValue(ip ?? "", out t) ? t : (DateTime?)null;
                lastDisconnected = mLastDisconnectByIp.TryGetValue(ip ?? "", out t) ? t : (DateTime?)null;
            }
        }

        /// <summary>
        /// "10.104.17.20:52311" -> "10.104.17.20"
        /// </summary>
        public static string GetIp(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) return "";
            int idx = endpoint.LastIndexOf(':');
            return (idx > 0 ? endpoint.Substring(0, idx) : endpoint).Trim();
        }

        public static string FormatDuration(TimeSpan ts)
        {
            if (ts.TotalDays >= 1) return $"{(int)ts.TotalDays}天{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }

    #endregion

    #region Server

    /// <summary>
    /// 由 Task 實作，提供管理通道實際操作 Socket 的能力
    /// </summary>
    public interface ISocketAdminHandler
    {
        /// <summary>Socket Server 是否開啟中</summary>
        bool IsServerOpen { get; }

        /// <summary>Socket Server 的連線字串</summary>
        string ListenConnStr { get; }

        /// <summary>
        /// 目前 Socket 層實際存在的 Client (IP:Port)。用來與 Tracker 對帳，可回傳 null。
        /// </summary>
        IList<string> GetSocketEndpoints();

        /// <summary>強制斷開指定 Client。0：成功；其他：失敗</summary>
        int Kick(string endpoint);

        /// <summary>
        /// 對指定 Client 送出一筆協定訊息。0：成功；其他：API 回傳碼
        /// </summary>
        int SendTo(string endpoint, int messageType, int messageId, string jsonContent);

        /// <summary>
        /// 預期會連入的站點清單；不需要站點管理時回傳 null。
        /// 讀取失敗請丟出例外，管理通道會沿用上次成功的清單。
        /// </summary>
        IList<SocketStationDef> GetStationDefinitions();
    }

    /// <summary>
    /// 本機管理通道 (TCP，一行一個 JSON 請求/回應)。
    /// 預設只綁 127.0.0.1，UITest 透過 <see cref="SocketAdminClient"/> 查詢/管理。
    /// <para>請求：{"cmd":"list"} / {"cmd":"events","since":0} / {"cmd":"kick","endpoint":"1.2.3.4:5678"} /
    /// {"cmd":"send","endpoint":"1.2.3.4:5678","messageType":2,"messageId":1,"json":"{...}"}</para>
    /// </summary>
    public class SocketAdminServer
    {
        public const int DefaultCmftPort = 18000;
        public const int DefaultDcuPort = 18001;

        private readonly string mProcName;
        private readonly SocketClientTracker mTracker;
        private readonly ISocketAdminHandler mHandler;
        private TcpListener mListener;
        private Thread mAcceptThread;
        private volatile bool mRunning;
        private IList<SocketStationDef> mLastStationDefs;

        public string BindIP { get; private set; }
        public int Port { get; private set; }

        public SocketAdminServer(string procName, SocketClientTracker tracker, ISocketAdminHandler handler)
        {
            mProcName = procName;
            mTracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            mHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// 開啟管理通道
        /// </summary>
        /// <param name="bindIP">空白時為 127.0.0.1</param>
        /// <param name="port">通訊埠</param>
        /// <returns>0：成功；-1：失敗</returns>
        public int Start(string bindIP, int port)
        {
            try
            {
                Stop();

                BindIP = string.IsNullOrWhiteSpace(bindIP) ? "127.0.0.1" : bindIP.Trim();
                Port = port;

                IPAddress address = BindIP == "0.0.0.0" ? IPAddress.Any : IPAddress.Parse(BindIP);
                mListener = new TcpListener(address, port);
                mListener.Start();
                mRunning = true;

                mAcceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = mProcName + "_SocketAdmin" };
                mAcceptThread.Start();

                ASI.Lib.Log.DebugLog.Log(mProcName, $"Socket 管理通道已開啟 {BindIP}:{port}");
                return 0;
            }
            catch (Exception ex)
            {
                mRunning = false;
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"Socket 管理通道開啟失敗 {bindIP}:{port}，{ex.Message}");
                return -1;
            }
        }

        public void Stop()
        {
            mRunning = false;
            try { mListener?.Stop(); } catch { }
            mListener = null;
            mAcceptThread = null;
        }

        private void AcceptLoop()
        {
            while (mRunning)
            {
                try
                {
                    TcpClient client = mListener.AcceptTcpClient();
                    var t = new Thread(() => HandleClient(client)) { IsBackground = true };
                    t.Start();
                }
                catch (Exception ex)
                {
                    if (mRunning)
                    {
                        ASI.Lib.Log.ErrorLog.Log(mProcName, $"Socket 管理通道 Accept 錯誤：{ex.Message}");
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            using (client)
            {
                try
                {
                    client.ReceiveTimeout = 60000;
                    var stream = client.GetStream();
                    var reader = new StreamReader(stream, new UTF8Encoding(false));
                    var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true, NewLine = "\n" };

                    string line;
                    while (mRunning && (line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string response;
                        try
                        {
                            response = Process(JObject.Parse(line)).ToString(Formatting.None);
                        }
                        catch (Exception ex)
                        {
                            response = Error(ex.Message).ToString(Formatting.None);
                        }
                        writer.WriteLine(response);
                    }
                }
                catch (IOException)
                {
                    // client 關閉或逾時
                }
                catch (Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, $"Socket 管理通道處理錯誤：{ex.Message}");
                }
            }
        }

        private JObject Process(JObject request)
        {
            string cmd = (string)request["cmd"] ?? "";
            switch (cmd.ToLowerInvariant())
            {
                case "list":
                    {
                        var status = new SocketServerStatus
                        {
                            Server = mTracker.ServerName,
                            ProcessName = mProcName,
                            ListenConnStr = mHandler.ListenConnStr,
                            IsOpen = mHandler.IsServerOpen,
                            ServiceStartedAt = mTracker.StartedAt,
                            LatestSeq = mTracker.LatestSeq,
                            Clients = Reconcile(mTracker.GetClients())
                        };
                        var result = Ok();
                        result["status"] = JObject.FromObject(status);
                        return result;
                    }
                case "events":
                    {
                        long since = (long?)request["since"] ?? 0;
                        var result = Ok();
                        result["latestSeq"] = mTracker.LatestSeq;
                        result["events"] = JArray.FromObject(mTracker.GetEvents(since));
                        return result;
                    }
                case "kick":
                    {
                        string endpoint = (string)request["endpoint"];
                        if (string.IsNullOrEmpty(endpoint)) return Error("缺少 endpoint");

                        mTracker.AddEvent("Kick", endpoint, "由管理介面要求強制斷線");
                        int rtn = mHandler.Kick(endpoint);
                        if (rtn != 0)
                        {
                            mTracker.AddEvent("Error", endpoint, $"強制斷線失敗，回傳碼 {rtn}");
                            return Error($"強制斷線失敗，回傳碼 {rtn}（Client 可能已離線）");
                        }
                        ASI.Lib.Log.DebugLog.Log(mProcName, $"管理介面強制斷開 {endpoint}");
                        return Ok();
                    }
                case "send":
                    {
                        string endpoint = (string)request["endpoint"];
                        if (string.IsNullOrEmpty(endpoint)) return Error("缺少 endpoint");
                        int messageType = (int?)request["messageType"] ?? 2;
                        int messageId = (int?)request["messageId"] ?? 1;
                        string json = (string)request["json"];

                        int rtn = mHandler.SendTo(endpoint, messageType, messageId, json);
                        mTracker.AddEvent(rtn == 0 ? "Send" : "Error", endpoint,
                            $"測試訊息 Type={messageType} ID={messageId} 回傳碼={rtn} 內容={Truncate(json, 200)}");
                        if (rtn == 0) mTracker.OnSent(endpoint);
                        ASI.Lib.Log.DebugLog.Log(mProcName, $"管理介面送出測試訊息至 {endpoint}，Type={messageType}，ID={messageId}，結果={rtn}，內容={json}");

                        var result = rtn == 0 ? Ok() : Error($"送出失敗，回傳碼 {rtn}");
                        result["code"] = rtn;
                        return result;
                    }
                case "stations":
                    {
                        var result = Ok();
                        result["report"] = JObject.FromObject(BuildStationReport());
                        return result;
                    }
                case "ping":
                    return Ok();
                default:
                    return Error($"未知指令: {cmd}");
            }
        }

        private SocketStationReport BuildStationReport()
        {
            var report = new SocketStationReport
            {
                Server = mTracker.ServerName,
                IsOpen = mHandler.IsServerOpen,
                ServiceStartedAt = mTracker.StartedAt
            };

            IList<SocketStationDef> defs;
            try
            {
                defs = mHandler.GetStationDefinitions();
                if (defs != null) mLastStationDefs = defs;
            }
            catch (Exception ex)
            {
                defs = mLastStationDefs;
                report.DefinitionError = ex.Message;
            }
            if (defs == null)
            {
                if (report.DefinitionError == null) report.DefinitionError = "此 Server 未提供站點清單";
                defs = new List<SocketStationDef>();
            }

            var clients = Reconcile(mTracker.GetClients());
            var matched = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var def in defs)
            {
                string ip = (def.Ip ?? "").Trim();
                var conns = clients
                    .Where(c => ip.Length > 0 && string.Equals(SocketClientTracker.GetIp(c.Endpoint), ip, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(c => c.ConnectedAt)
                    .ToList();
                foreach (var c in conns)
                {
                    matched.Add(c.Endpoint);
                    if (string.IsNullOrEmpty(c.Note) || c.Note.StartsWith("(")) c.Note = def.StationId;
                }

                DateTime? lastConn, lastDisc;
                mTracker.GetIpHistory(ip, out lastConn, out lastDisc);

                report.Stations.Add(new SocketStationStatus
                {
                    StationId = def.StationId,
                    Name = def.Name,
                    Ip = ip,
                    Online = conns.Count > 0,
                    ConnectionCount = conns.Count,
                    Connections = conns,
                    LastConnectedAt = lastConn,
                    LastDisconnectedAt = lastDisc
                });
            }

            report.UnknownClients = clients.Where(c => !matched.Contains(c.Endpoint)).ToList();
            return report;
        }

        /// <summary>
        /// 以 Socket 層實際的 Client 清單為準，補上 Tracker 漏記的連線
        /// </summary>
        private List<SocketClientInfo> Reconcile(List<SocketClientInfo> tracked)
        {
            IList<string> actual = null;
            try { actual = mHandler.GetSocketEndpoints(); } catch { }
            if (actual == null) return tracked;

            var set = new HashSet<string>(actual, StringComparer.OrdinalIgnoreCase);
            var result = tracked.Where(c => set.Contains(c.Endpoint)).ToList();
            foreach (var ep in actual)
            {
                if (!tracked.Any(c => string.Equals(c.Endpoint, ep, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(new SocketClientInfo
                    {
                        Server = mTracker.ServerName,
                        Endpoint = ep,
                        Note = "(未記錄連線時間)",
                        ConnectedAt = DateTime.MinValue
                    });
                }
            }
            return result;
        }

        private static string Truncate(string s, int max)
        {
            if (s == null) return "";
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }

        private static JObject Ok()
        {
            return new JObject { ["ok"] = true };
        }

        private static JObject Error(string message)
        {
            return new JObject { ["ok"] = false, ["error"] = message };
        }
    }

    #endregion

    #region Client

    /// <summary>
    /// 管理通道的 Client，供 UITest 使用。每次呼叫建立一條短連線。
    /// </summary>
    public class SocketAdminClient
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int TimeoutMs { get; set; } = 1500;

        public SocketAdminClient(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public SocketServerStatus List()
        {
            var res = Request(new JObject { ["cmd"] = "list" });
            return res["status"].ToObject<SocketServerStatus>();
        }

        public List<SocketAdminEvent> Events(long since, out long latestSeq)
        {
            var res = Request(new JObject { ["cmd"] = "events", ["since"] = since });
            latestSeq = (long?)res["latestSeq"] ?? 0;
            return res["events"].ToObject<List<SocketAdminEvent>>();
        }

        public SocketStationReport Stations()
        {
            var res = Request(new JObject { ["cmd"] = "stations" });
            return res["report"].ToObject<SocketStationReport>();
        }

        public void Kick(string endpoint)
        {
            Request(new JObject { ["cmd"] = "kick", ["endpoint"] = endpoint });
        }

        public void Send(string endpoint, int messageType, int messageId, string json)
        {
            Request(new JObject
            {
                ["cmd"] = "send",
                ["endpoint"] = endpoint,
                ["messageType"] = messageType,
                ["messageId"] = messageId,
                ["json"] = json
            });
        }

        /// <summary>
        /// 送出請求；連線失敗丟出 SocketException/IOException，服務端回報錯誤丟出 InvalidOperationException
        /// </summary>
        public JObject Request(JObject request)
        {
            using (var client = new TcpClient())
            {
                var ar = client.BeginConnect(Host, Port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(TimeoutMs))
                    throw new TimeoutException($"連線 {Host}:{Port} 逾時");
                client.EndConnect(ar);

                client.SendTimeout = TimeoutMs;
                client.ReceiveTimeout = TimeoutMs * 2;

                var stream = client.GetStream();
                var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true, NewLine = "\n" };
                var reader = new StreamReader(stream, new UTF8Encoding(false));

                writer.WriteLine(request.ToString(Formatting.None));
                string line = reader.ReadLine();
                if (line == null) throw new IOException("管理通道未回應");

                var response = JObject.Parse(line);
                if (!((bool?)response["ok"] ?? false))
                    throw new InvalidOperationException((string)response["error"] ?? "未知錯誤");
                return response;
            }
        }
    }

    #endregion
}
