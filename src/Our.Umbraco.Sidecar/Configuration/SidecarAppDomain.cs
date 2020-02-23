using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Our.Umbraco.Sidecar.Configuration
{
    internal class SidecarAppDomain : IDisposable
    {
        private readonly string oldConfig = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

        private bool isDisposed;

        public SidecarAppDomain(string path)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            ResetConfigMechanism();
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", oldConfig);

                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

                ResetConfigMechanism();

                isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',').FirstOrDefault();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies.FirstOrDefault(x => x.GetName().Name == assemblyName);
        }

        private static void ResetConfigMechanism()
        {
            typeof(ConfigurationManager)
                .GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, 0);

            typeof(ConfigurationManager)
                .GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, null);

            typeof(ConfigurationManager)
                .Assembly.GetTypes()
                .Where(x => x.FullName == "System.Configuration.ClientConfigPaths")
                .First()
                .GetField("s_current", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, null);
        }
    }
}