# HomeServer-Backend Core Documentation

## Overview

The `ServerCore` class is the central component of the HomeServer-Backend. It coordinates configuration loading, process management, TCP server communication, and API request handling. All main server operations are initiated and managed through this class.

## Initialization

When a `ServerCore` instance is created, it:

1. Loads configuration from a specified path or the default location.
2. Sets up the logging system using the configured log path.
3. Initializes the TCP server (`SimpleTcpServer`) with the configured port and host.
4. Initializes the process manager (`ProcessesManager`).
5. Registers the API message handler for TCP requests.

## Main Responsibilities

- **Configuration Management:**  
  Loads and saves server configuration using the `Config` class.

- **Process Management:**  
  Uses `ProcessesManager` to track, start, stop, and interact with managed processes.

- **TCP Communication:**  
  Handles incoming client requests via the `SimpleTcpServer`, responding to API calls.

- **API Request Routing:**  
  Routes requests to appropriate handler methods based on request type and path.

## Key Methods

### Constructor

```csharp
public ServerCore(string ConfigPath = "")
```
- Loads configuration and initializes core components.

### LoadData

```csharp
public void LoadData()
```
- Loads process slave data from configuration files.
- Adds loaded processes to the manager.

### Start

```csharp
public void Start()
```
- Starts the TCP server and all managed processes.

### Shutdown

```csharp
public void Shutdown()
```
- Stops the TCP server and all managed processes.

### ClientHandler

```csharp
ServerMessageFormat ClientHandler(ClientMessageFormat message)
```
- Routes incoming API requests to the correct handler based on request type (`GET`, `POST`, `UPDATE`, `DELETE`).

## API Request Handling

The core uses several internal methods to handle API requests:

- `HandleGETRequests(ClientMessageFormat message)`
- `HandlePOSTRequests(ClientMessageFormat message)`
- `HandleUPDATERequests(ClientMessageFormat message)`
- `HandleDELETERequests(ClientMessageFormat message)`

Each handler method matches the request path and delegates to specific API methods, such as:

- `ApiProcesses` – Returns all managed processes.
- `ApiProcessesStatus` – Returns status of all processes.
- `ApiProcessLastLogs` – Returns last logs for a process.
- `ApiProcessLastErrors` – Returns last errors for a process.
- `ApiProcessesStart` – Starts a process by tag.
- `ApiProcessesStop` – Stops a process by tag.
- `ApiProcessInput` – Sends input to a running process.

## Example Flow

1. **Startup:**  
   - `ServerCore` loads config and process data, starts TCP server.
2. **Client Request:**  
   - A client sends a JSON API request via TCP.
3. **Routing:**  
   - `ClientHandler` routes the request to the correct handler.
4. **Processing:**  
   - The handler interacts with the process manager or logger as needed.
5. **Response:**  
   - A `ServerMessageFormat` response is sent back to the client.

## Extending Core Functionality

To add new API endpoints or features:
- Implement new handler methods in `ServerCoreAPI.cs`.
- Update the corresponding request handler (`HandleGETRequests`, etc.) to route new paths.

## Related Files

- [`Core/ServerCore.cs`](../Core/ServerCore.cs)
- [`Core/ServerCoreAPI.cs`](../Core/ServerCoreAPI.cs)
- [`Loader/Config.cs`](../Loader/Config.cs)
- [`ProcessesManager.cs`](../ProcessesManager.cs)
- [`Communication/SimpleTcpServer.cs`](../Communication/SimpleTcpServer.cs)

## Summary

The `ServerCore` class is the main entry point and orchestrator for the backend server, handling configuration, process management, and API communication in a modular and extensible way.