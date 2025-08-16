﻿using System;
using System.Collections.Generic;
using System.Management;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HomeServer_Backend.ProcessHandler;
using System.Runtime.Versioning;

namespace HomeServer_Backend.ExtensionsLibs
{
    internal static class ProcessExtensions
    {
        public static ProcessHandler.ProcessRunningData GetRunningData(this ProcessHandler handler)
            => new ProcessHandler.ProcessRunningData() { 
                ChildrensMemoryUsage = handler.GetChildrensMemoryUsage(),
                MemoryUsage = handler.GetMemoryUsage(),
                ProcessID = handler.GetProcessID().GetValueOrDefault(-1),
                ProcessName = handler.GetProcessName(),
                Running = handler.IsRunning,
                Childrens = { }
            };


        [SupportedOSPlatform("windows")]
        public static IList<Process> GetChildProcesses(this Process process)
            => new ManagementObjectSearcher(
                    $"Select * From Win32_Process Where ParentProcessID={process.Id}")
                .Get()
                .Cast<ManagementObject>()
                .Select(mo =>
                    Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])))
                .ToList();

        public static string GetMemoryUsageFormated(this Process process)
        {

            try
            {
                double ssMemory = process.WorkingSet64;
                return BytesToFormatedString(ssMemory);
            }
            catch (Exception ex)
            {
                return $"Error retrieving memory usage: {ex.Message}";
            }
        }

        public static string BytesToFormatedString(double ssMemory)
        {
            string[] MemorySizes = {"B", "KB", "MB", "GB", "TB" };

            foreach (var size in MemorySizes)
            {
                if (ssMemory < 1024)
                {
                    return $"{ssMemory:0.00} {size}";
                }
                ssMemory /= 1024.0;
            }
            return $"{ssMemory:0.00} PB"; // Return the largest size if it exceeds TB

        }
    }
}
