using System;
using Serilog;
using Umbraco.Core.Logging.Serilog;
using Umbraco.Core.Runtime;
using ILogger = Umbraco.Core.Logging.ILogger;

namespace Our.Umbraco.Sidecar
{
    internal class SidecarRuntime : CoreRuntime
    {
        /// <summary>
        /// Override logger because <see cref="LoggerConfigExtensions.MinimalConfiguration(LoggerConfiguration)"/> fails
        /// HttpRuntime.AppDomainId is null outside of a web context
        /// Http request log enrichers are not needed
        /// </summary>
        protected override ILogger GetLogger()
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