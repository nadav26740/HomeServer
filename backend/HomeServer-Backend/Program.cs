using HomeServer_Backend.Loader;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HomeServer_Backend
{
    internal class Program
    {
        // GEN 1 DEBUG
        //static void Main(string[] args)
        //{
        //    Logger.LogInfo("Home Server Starting...");
        //    Config.ReadConfigFile();
        //    Logger.LogInfo(Config.data.ToString());
        //    Config.WriteConfigFile();
        //    ProcessHandler.ProcessInfo testinfo = new ProcessHandler.ProcessInfo(
        //        "TestProcess",
        //        "java.exe",
        //        "-Xmx4096M -Xms1024M -jar minecraft_server.1.21.4.jar nogui",
        //        "D:\\dumb minecraft servers\\",
        //        "stop"
        //    );
        //    ProcessHandler handler = new ProcessHandler(testinfo);
        //    handler.StartProcess();
        //    Logger.LogInfo(handler.ToString());


        //    Console.ReadKey();
        //    Logger.LogInfo("Test Process Memory: " + handler.GetMemoryUsageString() + " bytes");
        //    Logger.LogInfo("Test Childrens Memory: " + handler.GetChildrensMemoryUsageString());

        //    // Testing Get Last Logs
        //    var LastLogs = handler.GetLastLogs();
        //    Console.WriteLine("============ Reading Last Logs ============");
        //    foreach (var log in LastLogs)
        //    {
        //        Console.WriteLine(log.Item2);
        //    }
        //    Console.WriteLine("===========================================");

        //    Console.ReadKey();

        //    Logger.LogInfo("Home Server Stopping...");
        //    handler.StopProcess();
        //}


        // GEN 2 debug
        static void Main(string[] args)
        {
            ProcessSlaveArgs SlaveInfoContainer = new ProcessSlaveArgs();

            SlaveInfoContainer.AutoStart = true;
            SlaveInfoContainer.ProcessInfo  = new ProcessHandler.ProcessInfo(
                "Minecraft Server",
                "java.exe",
                "-Xmx4096M -Xms1024M -jar minecraft_server.1.21.4.jar nogui",
                "D:\\dumb minecraft servers\\",
                "stop"
            );

            SlaveInfoContainer.Priority = ProcessesManager.ProcessPriority.Core;

            Console.WriteLine($"Slave to test: {SlaveInfoContainer.ToString()}");
            Console.WriteLine($"Slave To test json: {SlaveInfoContainer.DeserilizeToJson()}");


            Console.WriteLine("Press Any Key to start test");
            Console.ReadKey();

            ProcessesManager manager = new ProcessesManager();
            manager.AddProcess(SlaveInfoContainer.CreateProcessSlave());

            Console.ReadKey();

            ProcessesManager.ProcessSlave? slave = manager.FindProcess(SlaveInfoContainer.ProcessInfo.Tag);
            Logger.LogInfo($"Minecraft server Ram usage: {slave?.ProcessHandler.GetTotalMemoryUsageString()}");
            slave.ProcessHandler.StopProcess();

            Console.ReadKey();

            Logger.LogInfo("Testprocess stop");
            manager.RemoveProcess("Minecraft Server", true);
            Logger.LogInfo("Home Server Stopping...");
            manager.Shutdown();
        }
    }
}
