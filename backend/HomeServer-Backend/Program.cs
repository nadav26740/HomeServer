using HomeServer_Backend.Loader;
using System.Runtime.CompilerServices;

namespace HomeServer_Backend
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.LogInfo("Home Server Starting...");
            Config.ReadConfigFile();
            Logger.LogInfo(Config.data.ToString());
            Config.WriteConfigFile();
        }
    }
}
