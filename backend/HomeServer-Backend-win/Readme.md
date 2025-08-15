# 🏠 HomeServer-Backend-win

> **Summary:**  
> _HomeServer-Backend-win is a modern .NET 8.0 server for managing and monitoring Windows processes via a TCP API. It’s designed for automation, remote control, and robust logging._

---

## ✨ Overview

HomeServer-Backend-win empowers you to control, monitor, and automate Windows processes remotely. With a simple TCP API, you can start, stop, and interact with processes, while enjoying configurable logging and easy setup.

---

## 🚀 Features

- ⚡ **TCP API** for process management
- 🏷️ Start/stop processes by tag
- 💬 Send input to running processes
- 📜 Retrieve last logs and errors for processes
- 🛠️ Configurable via `config.json`
- 📝 Logging to file and console

---

## 🧰 Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows OS (uses System.Management)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)

---

## 🏁 Getting Started

1. **Clone the repository**
2. **Configure**  
   Edit `bin/Debug/net8.0/config.json` to set paths, logging, and server port.
3. **Build**  
   Open [HomeServer-Backend.sln](HomeServer-Backend.sln) in Visual Studio and build, or run:
   ```sh
   dotnet build
   ```
4. **Run**  
   Execute the server:
   ```sh
   dotnet run --project HomeServer-Backend-win/HomeServer-Backend.csproj
   ```

---

## 🔗 API Usage

Connect to the server via TCP (default port: `3391`) and send JSON messages.  
See [API-Gateway/Tester.py](API-Gateway/Tester.py) for example usage.

### 📦 Example API Requests

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

## ⚙️ Configuration

Edit `config.json` for:
- `ConfigPath`: Path to config file
- `DataPath`: Path to process data
- `LogPath`: Log file directory
- `EnableLogging`: Enable/disable logging
- `ServerPort`: TCP port for API

---

## 📚 Core Components

- **ServerCore:** Main orchestrator, handles config, process manager, and TCP server.
- **ProcessesManager:** Tracks and supervises all managed processes.
- **SimpleTcpServer:** Handles TCP connections and API requests.
- **Logger:** Centralized logging to file and console.

See [Docs/Core.md](Docs/Core.md) for more details.

---

## 📝 Logging

Logs are written to the directory specified in `LogPath` and also printed to the console.

---

## 📄 License

MIT