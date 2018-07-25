using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MathSite.Api.Server.Infrastructure.Configuration
{
    public class LoggingSettings
    {
        public bool IncludeScopes { get; set; } = false;

        public IDictionary<string, LogLevel> LogLevel { get; protected set; } = new Dictionary<string, LogLevel>();
    }

    public class Settings
    {
        public IDictionary<string, string> ConnectionStrings { get; protected set; } = new Dictionary<string, string>();

        public LoggingSettings Logging { get; set; } = new LoggingSettings();
    }
}