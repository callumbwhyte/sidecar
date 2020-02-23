using System;
using Serilog;
using Umbraco.Core.Logging.Serilog;
using ILogger = Umbraco.Core.Logging.ILogger;

namespace Our.Umbraco.Sidecar.Configuration
{
    internal static class SidecarLogger
    {
        public static ILogger Create()
        {
            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("MACHINENAME", Environment.MachineName, EnvironmentVariableTarget.Process);

            var logConfig = new LoggerConfiguration();

            logConfig.MinimumLevel.Verbose()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .ReadFromConfigFile()
                .ReadFromUserConfigFile();

            return new SerilogLogger(logConfig);
        }
    }
}