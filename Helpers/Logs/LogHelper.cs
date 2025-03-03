using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Helpers.Logs
{
    public static class LogHelper
    {
        // Save log in the project's root directory
        private static readonly string LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ASM", "Logs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "app_logs.txt");

        static LogHelper()
        {
            // Ensure the directory exists
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }


        public static void AppendLog(string logType, string message, Dictionary<string, string>? additionalData = null)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Remove newlines from message to ensure logs are on a single line
                string sanitizedMessage = message.Replace("\r", " ").Replace("\n", " ");

                string logEntry = $"[{timestamp}] [{logType}] {sanitizedMessage}";

                if (additionalData != null && additionalData.Count > 0)
                {
                    logEntry += " | " + string.Join(", ", additionalData.Select(kv => $"{kv.Key}={kv.Value}"));
                }

                lock (typeof(LogHelper)) // Ensure thread safety
                {
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                    Console.WriteLine("Log file appended to :: " + LogFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing log: {ex.Message}");
            }
        }

        public static List<string> RetrieveLogs(Dictionary<string, string> searchCriteria)
        {
            List<string> matchingLogs = new List<string>();

            try
            {
                if (!File.Exists(LogFilePath)) return matchingLogs;

                var logs = File.ReadAllLines(LogFilePath);

                foreach (var log in logs)
                {
                    bool matchesAllCriteria = searchCriteria.All(kv => log.Contains($"{kv.Key}={kv.Value}"));

                    if (matchesAllCriteria)
                    {
                        matchingLogs.Add(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading log file: {ex.Message}");
            }

            return matchingLogs;
        }
    }
}
