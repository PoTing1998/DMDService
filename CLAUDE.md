# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DMDService is a Windows service-based system built with C# and .NET Framework 4.7.2. It implements a distributed task processing architecture where multiple task processes communicate via MSMQ (Microsoft Message Queuing).

## Building and Running

### Build the Solution
```bash
# Build entire solution in Visual Studio or via MSBuild
msbuild DMDService.sln /p:Configuration=Debug
msbuild DMDService.sln /p:Configuration=Release
```

### Build Individual Projects
```bash
# Build specific project
msbuild KernelService\KernelService.csproj /p:Configuration=Debug
msbuild TaskMain\TaskMain.csproj /p:Configuration=Debug
```

### Post-Build Deployment
All projects have a post-build event that copies binaries to `D:\ASI.Wanda.DMD.DMDService`. Be aware that this hardcoded path may need adjustment for different development environments.

## Architecture

### Layer Structure

The solution is organized into logical folders:

- **Server**: Contains service hosts
  - `KernelService`: Windows service that launches and manages TaskKernel
  - `TaskMain`: Process manager/orchestrator
  - `TaskKernel`: Kernel task process that monitors TaskMain
  - `ASILib`: Shared utility library

- **Task**: Task execution processes
  - `TaskCMFT`: CMFT-specific task processor
  - `TaskDCU`: DCU-specific task processor
  - `TaskOCS`: OCS task processor with Modbus polling

- **Frame**: Communication and API frameworks
  - `DMD_Frame`: DMD API and messaging framework (uses Socket communication with 0xAC/0xA9 delimiters)
  - `CMFT_Frame`: CMFT API and messaging framework

- **DB**: Database access layers
  - `DMD_DB`: DMD database models and tables
  - `CMFT_DB`: CMFT database models and tables
  - `DCU_DB`: DCU database models and tables

### Process Communication Pattern

All task processes inherit from `ASI.Lib.Process.ProcBase` and implement the `IProcess` interface:

1. **Process Lifecycle**:
   - `StartTask(string computerName, string processName)`: Initialize and connect to MSMQ
   - `Run()`: Main execution loop that processes messages
   - `StopTask()`: Cleanup and shutdown

2. **Message Handling**:
   - `ProcEvent(string label, string body)`: Handle incoming messages
   - `ProcTimerEvent(string message)`: Handle timer-based events
   - All messages use `MSGFrameBase` format: `[source]:[destination];[message body]`

3. **Inter-Process Communication**:
   - Uses MSMQ (`ASI.Lib.Comm.MSMQ.MsgQueLib`) for reliable message passing
   - Messages are sent via `ProcMsg.SendMessage()` or `MSQueue.SendMessage()`
   - Message types include: `MSGSimple`, `MSGHealth`, `MSGTimer`, `MSGStopProc`

4. **Process Monitoring**:
   - TaskKernel monitors TaskMain via health check messages
   - Processes track state: Initial, Healthy, Failed
   - Health timeouts configured via `ProcHealthTimeOut` and `ProcInitialTimeOut`

### Service Startup Flow

1. KernelService (Windows Service) starts
2. KernelService creates AppDomain and executes TaskKernel.exe
3. TaskKernel monitors and manages TaskMain process
4. Task processes (TaskCMFT, TaskDCU, TaskOCS) run independently and communicate via MSMQ

### Communication APIs

- **DMD_API**: Socket-based communication API (Server/Client mode)
  - Uses ByteMessage protocol with header/tail delimiters (0xAC, 0xA9)
  - Supports events: ReceivedEvent, OpenedEvent, ClosedEvent, ConnectedEvent
  - Connection string format: `"IP=192.168.0.1;Port=8000;Type=Server|Client"`

- **CMFT_API**: Similar pattern for CMFT communication

## Configuration

- Each process uses `ASI.Lib.Config.ConfigApp` singleton for configuration
- Config loaded from XML files (e.g., `TaskMain\Config\Config.xml`)
- Key config values:
  - `HOST_NAME`: Computer/hostname identifier
  - `SERVER_DB`: Server database connection
  - `LOCAL_DB`: Local database connection
  - `PROCESS`: Process-specific settings

## Key Libraries (ASILib)

The `ASILib` project provides shared utilities:

- **Process**: Base classes for task processes (`ProcBase`, `IProcess`, message types)
- **Comm**: Communication abstractions (Socket, MSMQ, SerialPort, SNMP, NTP)
- **DB**: Database utilities (`DBLib`)
- **Log**: Logging infrastructure (`LogFile`, `ErrorLog`, `DebugLog`)
- **Queue**: MSMQ wrappers (`QueueLib`, `MSMQ`)
- **Config**: Configuration management (`ConfigApp`)
- **Text**: String parsing and compression utilities
- **Msg**: Byte message parsing with CRC16

## Database Access Pattern

Each DB project (DMD_DB, CMFT_DB, DCU_DB) follows this pattern:
- `Manager.cs`: Database connection and operation manager
- `Models/`: Entity models
- `Tables/`: Table-specific data access classes

## TaskOCS Specific Implementation

### Modbus TCP Communication
- Uses custom `ModbusTcpClient` implementation (not external library)
- Connects to OCS (Operations Control System) via Modbus TCP
- Default connection: `10.107.26.99:502`
- Reads 38 ushort registers (76 bytes) per platform
- Configuration requires: `TcpClientIP`, `TcpClientPort` in Config.xml

### Multi-Client Polling Architecture
- `OCSClientPoller` manages multiple concurrent Modbus connections
- Three clients poll 6 addresses each (18 total addresses: 30001-31701)
- Each client runs in independent `Task` with long-running thread
- Polling interval: 1000ms per cycle

### Change Detection Logic
- Stores previous data in `Dictionary<string, ushort[][]>` per client
- Compares current vs previous data byte-by-byte
- Only sends updates when data changes or on first read
- Prevents redundant messages and database writes

### Data Flow: OCS → Database
```
Modbus Server → ModbusTcpClient → OCSClientPoller (change detection)
    → OCSPlatform.UpdateFromUShortArray() → UpdateOCSData()
    → InsertOrUpdateOCSData() → PostgreSQL (OCS_Data table)
```

### Data Flow: OCS → TaskDCU
```
OCSClientPoller → TrainMSG JSON → SendToTaskDCU()
    → MSGFromTaskOCS → MSMQ → TaskDCU → DMD_API.Send()
    → DCU Server (Socket)
```

### Critical Implementation Details
- **Field Assignment**: `InitializeOCSData()` must assign return value to `ocsDataInstance` field
- **Poller Reference**: Use `this.poller =` not `var poller =` to maintain reference
- **Resource Cleanup**: Override `StopTask()` to close Modbus connections
- **Database Operations**: `OCS_Data` table uses `platform_id` as query key for upsert operations

## TaskCMFT ↔ TaskDCU Communication

### Message Flow
```
CMFT Server (Socket) → TaskCMFT → MSMQ → TaskDCU → DCU Server (Socket)
```

### Supported Message Types
TaskCMFT processes these from CMFT and forwards to TaskDCU:
- `SendPreRecordMsg`: Pre-recorded message playback
- `SendInstantMsg`: Instant message broadcast
- `SendScheduleSetting`: Schedule configuration
- `SendPreRecordMessageSetting`: Pre-recorded message settings
- `SendTrainMessageSetting`: Train information display settings
- `SendPowerTimeSetting`: Power schedule settings
- `SendGroupSetting`: Device group configuration
- `SendParameterSetting`: System parameters

### Database Synchronization
TaskCMFT uses `CMFTHelper` to:
1. Read from CMFT database (CMFT_DB)
2. Synchronize to DMD database (DMD_DB)
3. Convert message format (CMFT → DMD)
4. Send via MSMQ to TaskDCU

### Response Handling
```
DCU Response → TaskDCU.DMD_API_ReceivedEvent()
    → Parse failed_target list → Delete from dmd_playlist
    → (Future: send response back to TaskCMFT → CMFT Server)
```

## Database Operations

### Connection Pattern
All task processes connect to dual databases:
- **DMD Database**: `ASI.Wanda.DMD.DB.Manager.Initializer()`
- **CMFT Database**: `ASI.Wanda.CMFT.DB.Manager.Initializer()`

Required config: `DMD_DB_IP`, `DMD_DB_Port`, `DMD_DB_Name`, `CMFT_DB_IP`, `CMFT_DB_Port`, `CMFT_DB_Name`

### Table Access Pattern
Tables inherit from `Table<T>` base class:
- `Insert(params object[])`: Auto-adds `ins_user`, `ins_time`, `upd_user`, `upd_time`
- `Update(params object[])`: Updates by primary key, auto-sets `upd_user`, `upd_time`
- `Select(params object[])`: Query by primary key
- `SelectWhere(string where)`: Custom WHERE clause
- `Delete(params object[])`: Delete by primary key

### OCS_Data Table Operations
- `InsertOCSData()`: Insert new OCS platform data
- `UpdateOCSDataByPlatformID()`: Update existing record
- `InsertOrUpdateOCSData()`: Automatic upsert based on platform_id
- `SelectByPlatformID()`: Check if record exists

## Common Issues and Solutions

### Project Reference Issues
- **Problem**: External SVN path references in `.csproj` files
- **Solution**: Remove absolute path references, keep only relative paths within solution
- **Example**: TaskCMFT had duplicate CMFT_DB reference with `D:\Svn\...` path - removed

### Field vs Local Variable
- **Problem**: Creating local variable instead of assigning to class field
- **Symptom**: `NullReferenceException` when accessing field later
- **Solution**: Use `this.fieldName =` or just `fieldName =` (not `var fieldName =`)
- **Example**: TaskOCS poller initialization required `this.poller =` assignment

### Resource Cleanup
- **Problem**: Background threads/connections not stopped properly
- **Solution**: Override `StopTask()` to dispose resources
- **Pattern**: Check for null, try-catch individual cleanup operations, call `base.StopTask()`

## Important Notes

- All namespaces use `ASI.Wanda.DMD` or `ASI.Lib` prefix
- Task executables accept process name as command-line argument (defaults to assembly name if not provided)
- Messages between processes must follow MSGFrameBase format for proper routing
- Socket communication uses proprietary ByteMessage protocol with specific delimiters
- Process health monitoring is critical - processes must send health messages within timeout periods
- Post-build events copy to `D:\ASI.Wanda.DMD.DMDService` - adjust path for local environments
