using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace GItClient.Core.Controllers
{
    internal static class LoggerProvider
    {
        private static ILoggerFactory? _loggerFactory;

        private static void CreateFactory()
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddNLog();
                NLog.LogManager.LoadConfiguration("NLog.config");
            });
        }

        public static ILogger GetLogger(string type)
        {
            if (_loggerFactory == null) CreateFactory();
            return _loggerFactory.CreateLogger(type);
        }
    }
}
