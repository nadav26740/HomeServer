using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HomeServer_Backend.Loader
{
    public struct ConfigData
    {
        // Default paths
        public const string default_config_path = "config.json";
        private const string default_operator_password = "Admin129716";
        private const string default_Process_data_path = "Processes.json";
        private const string default_log_path = "Logs/main";
        private const string default_backup_path = "backup.json";
        private const int default_Discover_port = 50130;
        private const int default_server_port = 3391;
        public ConfigData() 
        {
        }

        public string ConfigPath = default_config_path;
        public string DataPath  = default_Process_data_path;
        public string LogPath  = default_log_path;
        public string BackupPath  = default_backup_path;
        public string OperatorPassword  = default_operator_password;
        public bool EnableLogging  = true;
        public int ServerPort = default_server_port;
        public int DiscoveryPort = default_Discover_port;

        public override readonly string ToString()
        {
            return $"======Config Data======\n" +
                   $"  Config Path: {ConfigPath}\n" +
                   $"  Data Path: {DataPath}\n" +
                   $"  Log Path: {LogPath}\n" +
                   $"  Backup Path: {BackupPath}\n" +
                   $"  Operator Password: {OperatorPassword}\n" +
                   $"  Enable Logging: {EnableLogging}\n" +
                   $"  Server Port: {ServerPort}";
        }
    }

    /// <summary>
    /// This config is a singleton
    /// </summary>
    public class Config
    {
        private ConfigData _configData = new ConfigData();
        public static ConfigData data 
        {
            get { return Instance._configData; }
        }

        private static Config? _instance = null;
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config();
                }
                return _instance;
            }
        }

        private Config()
        {
            // TODO
        }

        private Config(ConfigData configData)
        {
            _configData = configData;
        }

        public static void LoadConfig(string path = ConfigData.default_config_path)
        {
            if (!Path.Exists(path))
            {
                throw new ArgumentException($"Config file not found at {path}. Please provide a valid path.");
            }

            try
            {
                ConfigData data = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(path));
                _instance = new Config(data);
            }
            catch
            {
                throw new Exception($"Error loading config file from {path}. Please check the file format and content.");
            }
        }

        // ============= Read Config File ===============
        /// <summary>
        /// Reads the configuration file from the specified path.
        /// </summary>
        /// <param name="path">Config file path</param>
        public static void ReadConfigFile(string? path = null)
        {
            Instance._ReadConfigFile(path);
        }

        private void _ReadConfigFile(string? path)
        {
            if (path == null)
            {
                path = _configData.ConfigPath;
            }
            else
            {
                _configData.ConfigPath = path;
            }

            string configContent = string.Empty;
            Console.WriteLine("Reading config file");

            try
            {
                using (StreamReader sr = new StreamReader(_configData.ConfigPath))
                {
                    configContent = sr.ReadToEnd();
                    _configData = JsonConvert.DeserializeObject<ConfigData>(configContent);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"Error reading config file: {err.Message}"); 
            }
            Console.WriteLine("Config File Read!");
        }
        // ===============================================

        // ============= Write Config File ===============
        private void _WriteConfigFile(string? path)
        {
            string configContent = JsonConvert.SerializeObject(_configData, Formatting.Indented);
            try
            {
                using (StreamWriter sw = new StreamWriter(path ?? _configData.ConfigPath))
                {
                    sw.Write(JsonConvert.SerializeObject(_configData, Formatting.Indented));
                }
            }
            catch (Exception err)
            {
                Logger.LogError($"Error writing config file: {err.Message}");
            }
            Logger.LogInfo($"Config File Written to {_configData.ConfigPath}!");
        }

        /// <summary>
        /// Write the current configuration to the config file.
        /// </summary>
        public static void WriteConfigFile(string? path = null)
        {
            Instance._WriteConfigFile(path);
        }
        // ===============================================
    }
}
