# **Logger Methods**
*Defined in [`Logger.cs`](../Logger.cs)*

## SetLoggingPath(path)
Sets the logging path for the logger.

- **Parameters:**  
  `path` (string): The directory path where logs will be saved.

---

## LogInfo(message)
Logs an informational message to the log file and console.

- **Parameters:**  
  `message` (string): The message to log.

---

## LogWarn(message)
Logs a warning message to the log file and console.

- **Parameters:**  
  `message` (string): The warning message to log.

---

## LogError(message)
Logs an error message to the log file and console.

- **Parameters:**  
  `message` (string): The error message to log.

---

# **ProcessHandler Methods**
*Defined in [`ProcessHandler.cs`](../ProcessHandler.cs)*

## Start()
Starts the managed process.

---

## Stop()
Stops the managed process.

---

## SendInput(input)
Sends input to the running process.

- **Parameters:**  
  `input` (string): The input string to send.

---

## GetStatus()
Gets the current status of the process.

- **Returns:**  
  Status string (e.g., Running, Stopped).

---

# **ProcessesManager Methods**
*Defined in [`ProcessesManager.cs`](../ProcessesManager.cs)*

## StartProcess(tag)
Starts a process by its tag.

- **Parameters:**  
  `tag` (string): The process tag.

---

## StopProcess(tag)
Stops a process by its tag.

- **Parameters:**  
  `tag` (string): The process tag.

---

## ProcessesToSlavesArgs()
Converts all processes to their argument representations for saving.

- **Returns:**  
  List of process arguments.

---

# **ProcessLogger Methods**
*Defined in [`ProcessLogger.cs`](../ProcessLogger.cs)*

## LogInfo(message)
Logs an informational message for the process.

- **Parameters:**  
  `message` (string): The message to log.

---

## LogError(message)
Logs an error message for the process.

- **Parameters:**  
  `message` (string): The error message to log.

---

## GetLastLogs()
Retrieves the last N logs for the process.

- **Returns:**  
  List of log messages.

---

# **SimpleTcpServer Methods**
*Defined in [`SimpleTcpServer.cs`](../Communication/SimpleTcpServer.cs)*

## Start()
Starts the TCP server and begins listening for connections.

---

## Stop()
Stops the TCP server.

---

## SendResponse(response)
Sends a response to the connected client.

- **Parameters:**  
  `response` (string): The response message to send.

---

You can copy this structure for each class and method in your project, expanding descriptions as