# 🌟 HomeServer-Backend Core Documentation

---

## 🚀 Overview

The `ServerCore` class is the **central hub** of the HomeServer-Backend, orchestrating:

- ✅ **Configuration Loading**
- ✅ **Process Management**
- ✅ **TCP Server Communication**
- ✅ **API Request Routing**

It ensures seamless operation by coordinating all main server functions in a modular and extensible manner.

---

## 🛠️ Initialization Workflow

When a `ServerCore` instance is created, it:

1. 📁 **Loads configuration** from a custom or default path.
2. 📝 **Sets up logging** using the configured log path.
3. 🌐 **Initializes TCP server** (`SimpleTcpServer`) with specified host and port.
4. 🔄 **Starts the process manager** (`ProcessesManager`).
5. 📨 **Registers API message handler** for TCP requests.

---

## 🎯 Main Responsibilities

- **Configuration Management:**  
  - Handles loading and saving of server configuration using the `Config` class.

- **Process Management:**  
  - Tracks, starts, stops, and interacts with managed processes via `ProcessesManager`.

- **TCP Communication:**  
  - Listens for client requests through `SimpleTcpServer` and responds to API calls.

- **API Request Routing:**  
  - Directs requests to appropriate handler methods by request type and path.

---

## 🔑 Key Methods

| Method | Description |
|--------|-------------|
| `ServerCore(string ConfigPath = "")` | **Constructor:** Loads configuration and initializes all core components. |
| `void LoadData()` | Loads process slave data from configuration files and adds them to the manager. |
| `void Start()` | Starts the TCP server and all managed processes. |
| `void Shutdown()` | Stops the TCP server and all managed processes. |
| `ServerMessageFormat ClientHandler(ClientMessageFormat message)` | Routes incoming API requests to the correct handler based on request type (`GET`, `POST`, `UPDATE`, `DELETE`). |

---

## 🔄 API Request Handling

API requests are routed to internal handler methods:

- `HandleGETRequests(ClientMessageFormat message)`
- `HandlePOSTRequests(ClientMessageFormat message)`
- `HandleUPDATERequests(ClientMessageFormat message)`
- `HandleDELETERequests(ClientMessageFormat message)`

These handlers match the request path and delegate to specific API methods, including:

### 📩 Get
- `ApiProcesses` – Returns all managed processes.
- `ApiProcessesStatus` – Returns status of all processes.
- `ApiProcessLastLogs` – Returns last logs for a process.
- `ApiProcessLastErrors` – Returns last errors for a process.

### ⚙ Update
- `ApiProcessesStart` – Starts a process by tag.
- `ApiProcessesStop` – Stops a process by tag.

### 📮 Post
- `ApiProcessInput` – Sends input to a running process.

---

## 📝 Example Operation Flow

1. **Startup**
    - `ServerCore` loads configuration and process data, then starts the TCP server.
2. **Client Request**
    - A client sends a JSON API request via TCP.
3. **Routing**
    - `ClientHandler` directs the request to the appropriate handler.
4. **Processing**
    - The handler interacts with the process manager and logger as needed.
5. **Response**
    - A `ServerMessageFormat` response is sent back to the client.

---

## 🧩 Extending Core Functionality

To add new API endpoints or features:

- Implement new handler methods in `ServerCoreAPI.cs`.
- Update the corresponding request handler (`HandleGETRequests`, etc.) to route new paths.

---

## 📂 Related Files

- [`Core/ServerCore.cs`](../Core/ServerCore.cs)
- [`Core/ServerCoreAPI.cs`](../Core/ServerCoreAPI.cs)
- [`Loader/Config.cs`](../Loader/Config.cs)
- ['Config.md'](./Config.md)
- [`ProcessesManager.cs`](../ProcessesManager.cs)
- [`Communication/SimpleTcpServer.cs`](../Communication/SimpleTcpServer.cs)

---

## 🏁 Summary

The `ServerCore` class serves as the **main entry point and orchestrator** for the backend server, ensuring robust configuration management, efficient process control, and flexible API communication. Its modular architecture makes it easy to extend and maintain.

---
