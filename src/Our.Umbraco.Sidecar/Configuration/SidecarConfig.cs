using System.Configuration;
using System.IO;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.HealthChecks;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.IO;

namespace Our.Umbraco.Sidecar.Configuration
{
    internal static class SidecarConfig
    {
        public static void Create(Configs configs)
        {
            var configDir = new DirectoryInfo(IOHelper.MapPath(SystemDirectories.Config));

            using (var context = new SidecarAppDomain(configDir + "/sidecar.config"))
            {
                var umbracoSettings = GetConfigSection<IUmbracoSettingsSection>("umbracoConfiguration/settings");

                configs.Add(() => umbracoSettings);

                var healthChecks = GetConfigSection<IHealthChecks>("umbracoConfiguration/HealthChecks");

                configs.Add(() => healthChecks);
            }
        }

        private static TConfig GetConfigSection<TConfig>(string sectionName)
            where TConfig : class
        {
            var section = ConfigurationManager.GetSection(sectionName);

            if (section is TConfig)
            {
                return section as TConfig;
            }

            throw new ConfigurationErrorsException($"Could not get configuration section \"{sectionName}\" from config files.");
        }
    }
}