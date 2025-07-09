﻿using HomeServer_Backend.Loader;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            ProcessHandler.ProcessInfo testinfo = new ProcessHandler.ProcessInfo(
                "TestProcess",
                "java.exe",
                "-Xmx4096M -Xms1024M -jar minecraft_server.1.21.4.jar nogui",
                "D:\\dumb minecraft servers\\",
                "stop"
            );
            ProcessHandler handler = new ProcessHandler(testinfo);
            handler.StartProcess();
            Logger.LogInfo(handler.ToString());


            Console.ReadKey();
            Logger.LogInfo("Test Process Memory: " + handler.GetMemoryUsageString() + " bytes");
            Logger.LogInfo("Test Childrens Memory: " + handler.GetChildrensMemoryUsageString());
            
            // Testing Get Last Logs
            var LastLogs = handler.GetLastLogs();
            Console.WriteLine("============ Reading Last Logs ============");
            foreach (var log in LastLogs)
            {
                Console.WriteLine(log.Item2);
            }
            Console.WriteLine("===========================================");

            Console.ReadKey();

            Logger.LogInfo("Home Server Stopping...");
            handler.StopProcess();
        }
    }
}
