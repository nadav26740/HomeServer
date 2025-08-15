<h1 align="center">🌐📡 API Protocol for 🏠 HomeServer 🛠️</h1>

<p style="color:gray;" align="center">Efficient • Secure • Reliable</p>


**All API requests and responses are in JSON format.**

>   - Server and client using JSON messages
>
>  - Supports both request/response and event-driven messages
>
>  - Responses include status and data fields
>
>  - Handles TCP connections and message routing
>
>  - Server uses `ServerCore` and `ServerCoreAPI` for routing
>
>  - Server work in Message and response only protocol and does not use long socket connections
> 
 
<br>

**Request example:**
```json
{
    "Path": "/api/process/start",
    "Type": "UPDATE",
    "Data": "Minecraft Server"
}
```

**Response example:**
```json
    {
         "Status": "200", // ok
         "Data": "Process Minecraft Server started successfully."
    }
```
--------------------------

## 📜 API Protocol Overview
### 1. **Request Structure:**  
>   - All messages are JSON objects  
>   - Contains `Path`, `Type`, and `Data` fields  
>       - `Path` specifies the API endpoint  
>       - `Type` indicates the request type (GET, POST, UPDATE, DELETE)  
>       - `Data` contains the payload or parameters

### 2. **Request Types:**  
>    - **GET:** Retrieve data
>    - **POST:** Submit data or trigger actions
>    - **UPDATE:** Modify existing processes or data
>    - **DELETE:** Remove data or stop processes

### 3. **Response Structure:**
>    - Contains `Status` and `Data` fields  
>         - `Status` indicates the result of the request (e.g., 200 for success, 404 for not found)  
>         - `Data` contains any relevant information or error messages

### 4. **Response status codes:**
>    - `200`: OK  
>    - `201`: Created
>    - `204`: No Content
>    - `400`: Bad Request
>    - `401`: Unauthorized
>    - `404`: Not Found  
>    - `500`: Internal Server Error
>    - `503`: Service Unavailable

### 5. **Error Handling:**
>    - Errors are communicated via status codes and error messages in the `Data` field

### 6. **Connection Management:**
>    - Uses TCP for communication
>    - Each request is independent; no persistent connections

### 7. **Security:**
>    - *Authentication and authorization are handled at the application level [TODO]*

--------------------------