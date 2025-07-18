using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HomeServer_Backend.Loader
{
    public static class ProcessConfigSave
    {
        public static ProcessesManager.ProcessSlave[]? CreateSlaves(this ProcessSlaveArgs[] slaveArgs)
        {
            if (slaveArgs == null || slaveArgs.Length == 0)
            {
                return null;
            }
            ProcessesManager.ProcessSlave[] slaves = new ProcessesManager.ProcessSlave[slaveArgs.Length];

            for (int i = 0; i < slaveArgs.Length; i++)
            {
                slaves[i] = slaveArgs[i].CreateProcessSlave();
            }

            return slaves;
        }

        public static ProcessSlaveArgs[]? ReadSlaveProcessData(string path)
        {
            if (!(path.Contains(".json") || path.Contains(".data")))
            {
                throw new ArgumentException("unknown file type");
            }
            
            if (!Path.Exists(path))
            {
                throw new FileNotFoundException("file not found", path);
            }

            ProcessSlaveArgs[]? processesData = JsonConvert.DeserializeObject<ProcessSlaveArgs[]>(File.ReadAllText(path)) ?? null;
            Logger.LogInfo($"Loaded {processesData?.Length} Process Slave Data from file: " + path);

            return processesData;
        }


        public static void SaveSlaveProcessData(string path, ProcessSlaveArgs[] data)
        {
            if (!Directory.Exists (Path.GetDirectoryName(path)))
            {

            }

            if (!(path.Contains(".json") || path.Contains(".data")))
            {
                throw new ArgumentException("unknown file type");
            }

            Logger.LogInfo("Saving Process Slave Data to file: " + path);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void SaveSlaveProcessData(this ProcessSlaveArgs[] data, string path)
        {
            SaveSlaveProcessData(path, data);
        }

        public static void SaveSlaveProcessData(this ProcessesManager processesManager, string path)
        {
            (processesManager.ProcessesToSlavesArgs()?? new ProcessSlaveArgs[0]).SaveSlaveProcessData(path);
        }
    }
}
