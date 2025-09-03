using Microsoft.Extensions.Logging;
using System;

namespace OrderProcessingSystem.Helpers
{
    public static class LoggingHelper
    {
        public static void LogInfo(ILogger logger, string message)
        {
            logger.LogInformation($"[INFO] {DateTime.UtcNow:u} - {message}");
        }

        public static void LogWarning(ILogger logger, string message)
        {
            logger.LogWarning($"[WARNING] {DateTime.UtcNow:u} - {message}");
        }

        public static void LogError(ILogger logger, string message, Exception ex)
        {
            logger.LogError(ex, $"[ERROR] {DateTime.UtcNow:u} - {message} | Exception: {ex.Message}");
        }
    }
}
