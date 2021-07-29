﻿using System;

namespace MSSQL_to_MongoDB.Helpers
{
    public static class LogHelper
    {
        public static void Log(string message, string location = "")
        {
            var log = $"[{GetTimestamp()}]";
            if (!string.IsNullOrEmpty(location)) { log += $" {location} |"; }
            log += $" {message}";

            Console.WriteLine(log);
        }

        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff");
        }
    }
}
