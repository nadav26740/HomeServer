<div align="center">
  
# 🏠 HomeServer
      
**A powerful, extensible backend for Windows process management,<br/> automation, and monitoring — accessible via a modern TCP API.**

</div>

---

## 🚀 About the Project

**HomeServer** is a robust backend platform enabling process management, monitoring, and automation on Windows. With an extensible TCP API, rich logging, and flexible configuration, it empowers you to remotely control and automate server workflows.

---

## ✨ Features

- 🧑‍💻 **Process Management**  
  Start, stop, and interact with Windows processes using friendly tags.

- 🔌 **TCP API**  
  Communicate securely via TCP using simple JSON messages.

- 📚 **Logging**  
  All server and process activity logged to file & console for easy auditing.

- ⚙️ **Configuration**  
  Effortless setup using `config.json`.

- 🔄 **Auto-Restart**  
  Critical processes are automatically restarted if needed.

- 🛠️ **Extensible API**  
  Easily add new endpoints and features via the core API handler.

---

## 🧑‍💻 Technologies Used <span style="color: #0078D7;">[Windows Version]</span>

- <span style="color: #512BD4;">.NET 8.0</span>  
- <span style="color: #D17A45;">Newtonsoft.Json</span> (JSON serialization)  
- <span style="color: #2F855A;">System.Management</span> (Windows process management)  
- <span style="color: #0078D7;">Windows Processes</span> (required for process APIs)  

---

## ⚡ How It Works

1. **Startup:**  
   - Loads configuration from `config.json`
   - Initializes logging and process manager
   - Starts TCP server (default port: `3391`)

2. **Process Management:**  
   - Processes defined and managed by tag  
   - Supports auto-start, stop, and sending input

3. **API Communication:**  
   - Clients send JSON requests via TCP  
   - Requests routed by [`ServerCore`](Core/ServerCore.cs) and [`ServerCoreAPI`](Core/ServerCoreAPI.cs)  
   - Responses include status codes and data

4. **Core Components:**  
   - [`ServerCore`](Core/ServerCore.cs): Main orchestrator  
   - [`ProcessesManager`](ProcessesManager.cs): Tracks & supervises all managed processes  
   - [`SimpleTcpServer`](Communication/SimpleTcpServer.cs): Handles TCP connections & API requests  
   - [`Logger`](Logger.cs): Centralized logging system  

---

## 🛠️ How to Compile & Run <span style="color:#0078D7;">[Windows]</span>

1. **Install .NET 8.0 SDK**  
   👉 [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)

2. **Clone the Repository**

   ```sh
   git clone <your-repo-url>
   cd backend/HomeServer-Backend-win
   ```

3. **Configure**  
   ✍️ Edit `bin/Debug/net8.0/config.json` for paths, logging, and port.

4. **Build**

   ```sh
   dotnet build
   ```
   Or open [`HomeServer-Backend.sln`](HomeServer-Backend.sln) in **Visual Studio** and build.

5. **Run**

   ```sh
   dotnet run --project HomeServer-Backend-win/HomeServer-Backend.csproj
   ```

---

## 📡 Example API Usage

See [`API-Gateway/Tester.py`](../API-Gateway/Tester.py) for Python test client.

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

## 📖 Core Documentation

See [`Docs/Core.md`](Docs/Core.md) for a detailed explanation of the core system and API routing.

---

## 🌱 Future Plans

- 🚀 More built-in process management features
- 🛡️ Improved error handling & logging
- 📈 Advanced process monitoring
- 🐧 Linux support (if feasible)

---

<div align="center">

### 💡 **HomeServer — Automate Anything, Run Everywhere!**

</div>
