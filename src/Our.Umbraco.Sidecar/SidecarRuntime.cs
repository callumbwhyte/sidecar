using System;
using Our.Umbraco.Sidecar.Configuration;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Runtime;
using RuntimeLevel = Umbraco.Core.RuntimeLevel;

namespace Our.Umbraco.Sidecar
{
    internal class SidecarRuntime : CoreRuntime
    {
        /// <summary>
        /// Override boot for better error handling
        /// </summary>
        public override IFactory Boot(IRegister register)
        {
            IFactory factory = base.Boot(register);

            if (State.Level != RuntimeLevel.Run)
            {
                if (State.Level == RuntimeLevel.Install)
                {
                    throw new Exception("Umbraco is not installed");
                }
                else if (State.Level == RuntimeLevel.Upgrade)
                {
                    throw new Exception("Umbraco needs to upgrade");
                }
                else
                {
                    throw State.BootFailedException;
                }
            }

            return factory;
        }

        /// <summary>
        /// Override logger to handle non-HTTP contexts
        /// <see cref="LoggerConfigExtensions.MinimalConfiguration"/> relies on <see cref="HttpRuntime.AppDomainId"/>
        /// </summary>
        protected override ILogger GetLogger() => SidecarLogger.Create();

        /// <summary>
        /// Override config to handle environments without XML based config
        /// <see cref="ConfigsExtensions.AddCoreConfigs"/> relies on <see cref="ConfigurationManager.GetSection()"/>
        /// </summary>
        protected override Configs GetConfigs()
        {
            var configs = base.GetConfigs();

            SidecarConfig.Create(configs);

            return configs;
        }
    }
}