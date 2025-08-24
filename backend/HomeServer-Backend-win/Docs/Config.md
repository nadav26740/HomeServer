# ⚙️ Configuration System Documentation

This document explains the configuration system for **HomeServer-Backend-win**.  
The configuration is managed via the `Config` class and the `ConfigData` struct, which handle loading, saving, and accessing all server settings.

---

## 📄 Config File Structure

The config file is a JSON file (default: `config.json`) containing all the settings needed for the backend server.  
Example:

```json
{
  "ConfigPath": "config.json",
  "DataPath": "Processes.json",
  "LogPath": "Logs/main",
  "BackupPath": "backup.json",
  "OperatorPassword": "Admin129716",
  "EnableDebugLogging": true,
  "EnableDiscovery": true,
  "EnableLogging": true,
  "ServerPort": 3391,
  "DiscoveryPort": 50130
}
```

---

## 🏷️ Config Fields

| Field                | Type    | Default Value         | Description                                 |
|----------------------|---------|----------------------|---------------------------------------------|
| `ConfigPath`         | string  | `config.json`        | Path to the config file                     |
| `DataPath`           | string  | `Processes.json`     | Path to process data file                   |
| `LogPath`            | string  | `Logs/main`          | Directory for logs                          |
| `BackupPath`         | string  | `backup.json`        | Path for backup file                        |
| `OperatorPassword`   | string  | `Admin129716`        | Operator/admin password                     |
| `EnableDebugLogging` | bool    | `true`               | Enable debug logging                        |
| `EnableDiscovery`    | bool    | `true`               | Enable network discovery                    |
| `EnableLogging`      | bool    | `true`               | Enable general logging                      |
| `ServerPort`         | int     | `3391`               | TCP port for API server                     |
| `DiscoveryPort`      | int     | `50130`              | Port for discovery service                  |

---

## 🛠️ How It Works

- **Singleton Pattern:**  
  The `Config` class is a singleton. Access the current config via `Config.Instance` or `Config.data`.

- **Loading Config:**  
  Use `Config.LoadConfig(path)` to load settings from a file.  
  If the file is missing or invalid, an exception is thrown.

- **Reading Config:**  
  Use `Config.ReadConfigFile(path)` to read and update the config from a file.

- **Writing Config:**  
  Use `Config.WriteConfigFile(path)` to save the current config to a file.

- **Accessing Values:**  
  All config values are available via `Config.data`, e.g. `Config.data.ServerPort`.

---

## 🔄 API Reference

### Load Config

```csharp
Config.LoadConfig("path/to/config.json");
```

### Read Config File

```csharp
Config.ReadConfigFile(); // Uses current path
Config.ReadConfigFile("custom/path.json");
```

### Write Config File

```csharp
Config.WriteConfigFile(); // Uses current path
Config.WriteConfigFile("custom/path.json");
```

### Access Config Values

```csharp
int port = Config.data.ServerPort;
string logPath = Config.data.LogPath;
```

---

## 📝 Notes

- If the config file is missing, the system uses default values.
- All changes to config values should be saved using `WriteConfigFile` to persist them.
- Sensitive fields (like `OperatorPassword`) should be protected and not exposed publicly.

---

## 📚 See Also

- [Core.md](./Core.md) — Main system documentation
- [Config.cs](../Loader/Config.cs) — Source code for the configuration system