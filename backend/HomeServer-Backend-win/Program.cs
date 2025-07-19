using HomeServer_Backend.Communication;
using HomeServer_Backend.Core;
using HomeServer_Backend.Loader;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HomeServer_Backend
{
    internal class Program
    {
        private static string GetSplashMessage() => "  _   _                        ____                           \r\n | | | | ___  _ __ ___   ___  / ___|  ___ _ ____   _____ _ __ \r\n | |_| |/ _ \\| '_ ` _ \\ / _ \\ \\___ \\ / _ | '__\\ \\ / / _ | '__|\r\n |  _  | (_) | | | | | |  __/  ___) |  __| |   \\ V |  __| |   \r\n |_| |_|\\___/|_| |_| |_|\\___| |____/ \\___|_|    \\_/ \\___|_|   \r\n                                                              ";


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
        //static void Main(string[] args)
        //{
        //    ProcessSlaveArgs SlaveInfoContainer;
        //    SimpleTcpServer SimpleServer = new SimpleTcpServer(9939);

        //    Console.WriteLine(GetSplashMessage());

        //    //SlaveInfoContainer.AutoStart = true;
        //    //SlaveInfoContainer.ProcessInfo  = new ProcessHandler.ProcessInfo(
        //    //    "Minecraft Server",
        //    //    "java.exe",
        //    //    "-Xmx4096M -Xms1024M -jar minecraft_server.1.21.4.jar nogui",
        //    //    "D:\\dumb minecraft servers\\",
        //    //    "stop"
        //    //);

        //    //SlaveInfoContainer.Priority = ProcessesManager.ProcessPriority.Core;

        //    //Console.WriteLine($"Slave to test: {SlaveInfoContainer.ToString()}");
        //    //Console.WriteLine($"Slave To test json: {SlaveInfoContainer.DeserilizeToJson()}");


        //    Console.WriteLine("Press Any Key to start test");

        //    // Starting Test
        //    Console.ReadKey();
        //    Logger.LogInfo("Reading Process Config File...");

        //    Logger.LogInfo("Starting Logger..");
        //    Logger.LogInfo("Starting HomeServer Test!");

        //    ProcessesManager manager = new ProcessesManager();

        //    foreach (var procs in ProcessConfigSave.ReadSlaveProcessData(Config.data.DataPath))
        //    {
        //        manager.AddProcess(procs.CreateProcessSlave());
        //    }

        //    Task.Run(SimpleServer.StartAsync);

        //    // Testing Save System 
        //    Console.ReadKey();

        //    Logger.LogInfo("Saving Process Data...");
        //    manager.SaveSlaveProcessData(Config.data.DataPath);

        //    // Testing Failure Recovery
        //    Console.ReadKey();


        //    ProcessesManager.ProcessSlave? slave = manager.FindProcess("Minecraft Server");
        //    Logger.LogInfo($"Minecraft server Ram usage: {slave?.ProcessHandler.GetTotalMemoryUsageString()}");
        //    slave.ProcessHandler.StopProcess();

        //    Console.ReadKey();

        //    // Testing Shutdown
        //    Logger.LogInfo("Testprocess stop");
        //    manager.RemoveProcess("Minecraft Server", true);
        //    Logger.LogInfo("Home Server Stopping...");
        //    SimpleServer.Stop();
        //    manager.Shutdown(true);
        //}

        // Gen 3 debug
        private static void Main(string[] args)
        {
            Console.WriteLine(GetSplashMessage());
            Console.WriteLine("Press any key to start test");
            Console.ReadKey();

            Console.WriteLine("Home Server test Starting...");
            ServerCore Core = new ServerCore();

            Logger.LogInfo("Loading data");
            Core.LoadData();

            Logger.LogInfo("Starting server...");
            Core.Start();
            Logger.LogInfo("Server started successfully!");

            Console.WriteLine("Press any key to stop");
            Console.ReadKey();
            
            Logger.LogInfo("Stopping server...");
            Core.Shutdown();
            Logger.LogInfo("Server stopped successfully!");

            Console.WriteLine("Home Server test completed. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
