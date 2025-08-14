# HomeServer

> **Summary:**  
> TODO

---

## About the Project

HomeServer provides a robust backend for process management, monitoring, and automation. It exposes a TCP API for remote control, supports logging, configuration, and is extensible for custom workflows.

---

## Features

- **Process Management:**  
  Start, stop, and interact with Windows processes by tag.

- **TCP API:**  
  Communicate with the server using JSON messages over TCP.

- **Logging:**  
  Logs process activity and server events to files and console.

- **Configuration:**  
  Easily configurable via `config.json`.

- **Auto-Restart:**  
  Supports auto-restarting critical processes.

- **Extensible API:**  
  Add new endpoints and features via the core API handler.

---

## Technologies Used [windows version]

- [.NET 8.0](https://dotnet.microsoft.com/)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) (JSON serialization)
- [System.Management](https://learn.microsoft.com/en-us/dotnet/api/system.management) (Windows process management)
- Windows Processes (required for process APIs)

---

## How It Works

1. **Startup:**  
   - Loads configuration from `config.json`.
   - Initializes logging and process manager.
   - Starts a TCP server (default port: 3391).

2. **Process Management:**  
   - Processes are defined and managed by tag.
   - Supports auto-start, stop, and sending input to running processes.

3. **API Communication:**  
   - Clients send JSON requests via TCP.
   - Requests are routed by [`ServerCore`](Core/ServerCore.cs) and [`ServerCoreAPI`](Core/ServerCoreAPI.cs).
   - Responses include status codes and data.

4. **Core Components:**  
   - [`ServerCore`](Core/ServerCore.cs): Main orchestrator, handles config, process manager, and TCP server.
   - [`ProcessesManager`](ProcessesManager.cs): Tracks and supervises all managed processes.
   - [`SimpleTcpServer`](Communication/SimpleTcpServer.cs): Handles TCP connections and API requests.
   - [`Logger`](Logger.cs): Centralized logging to file and console.

---

## How to Compile & Run [Windows]

1. **Install .NET 8.0 SDK**  
   [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)

2. **Clone the Repository**

   ```sh
   git clone <your-repo-url>
   cd backend/HomeServer-Backend-win
   ```

3. **Configure**  
   Edit `bin/Debug/net8.0/config.json` for paths, logging, and port.

4. **Build**

   ```sh
   dotnet build
   ```

   Or open [HomeServer-Backend.sln](HomeServer-Backend.sln) in Visual Studio and build.

5. **Run**

   ```sh
   dotnet run --project HomeServer-Backend-win/HomeServer-Backend.csproj
   ```

---

## Example API Usage

See [API-Gateway/Tester.py](../API-Gateway/Tester.py) for Python test client.

- **Start a process**
  ```json
  {"Path":"/api/process/start", "Type":"UPDATE", "Data":"Minecraft Server"}
  ```
- **Send input to a process**
  ```json
  {"Path":"api/process/input", "Type":"POST", "Data":"{\"ProcessTag\": \"Minecraft Server\", \"Input\": \"say hello API\"}"}
  ```
- **Stop a process**
  ```json
  {"Path":"/api/process/stop", "Type":"UPDATE", "Data":"Minecraft Server"}
  ```

---

## Core Documentation

See [Docs/Core.md](Docs/Core.md) for detailed explanation of the core system and API routing.

## Future Plans
- Add more built-in process management features.
- Improve error handling and logging.
- Implement more advanced process monitoring.
- Implement Linux support (if feasible).
---



##