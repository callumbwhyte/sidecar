using System;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Sidecar
{
    public abstract class BaseHttpApplication : HttpApplication
    {
        private IRuntime _runtime;

        protected virtual IRegister GetRegister() => RegisterFactory.Create();

        protected abstract IRuntime GetRuntime();

        // called by ASP.NET (auto event wireup) once per app domain
        // do NOT set instance data here - only static (see docs)
        // sender is System.Web.HttpApplicationFactory, evargs is EventArgs.Empty
        protected void Application_Start(object sender, EventArgs evargs)
        {
            HandleApplicationStart(sender, evargs);
        }

        // called by ASP.NET (auto event wireup) once per app domain
        // sender is System.Web.HttpApplicationFactory, evargs is EventArgs.Empty
        protected void Application_End(object sender, EventArgs evargs)
        {
            HandleApplicationEnd();
        }

        internal void HandleApplicationStart(object sender, EventArgs evargs)
        {
            var register = GetRegister();

            _runtime = GetRuntime();

            _runtime.Boot(register);
        }

        internal void HandleApplicationEnd()
        {
            if (_runtime != null)
            {
                _runtime.Terminate();
                _runtime.DisposeIfDisposable();

                _runtime = null;
            }

            // try to log the detailed shutdown message (typical asp.net hack: http://weblogs.asp.net/scottgu/433194)
            try
            {
                var runtime = (HttpRuntime)typeof(HttpRuntime).InvokeMember("_theRuntime",
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                    null, null, null);

                if (runtime == null)
                {
                    return;
                }

                var shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                    null, runtime, null);

                var shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                    null, runtime, null);

                Current.Logger.Info<BaseHttpApplication>("Application shutdown. Details: {ShutdownReason}\r\n\r\n_shutDownMessage={ShutdownMessage}\r\n\r\n_shutDownStack={ShutdownStack}",
                    HostingEnvironment.ShutdownReason,
                    shutDownMessage,
                    shutDownStack);
            }
            catch (Exception)
            {
                // if for some reason that fails, then log the normal output
                Current.Logger.Info<BaseHttpApplication>("Application shutdown. Reason: {ShutdownReason}", HostingEnvironment.ShutdownReason);
            }

            Current.Logger.DisposeIfDisposable();

            // dispose the container and everything
            Current.Reset();
        }
    }
}
