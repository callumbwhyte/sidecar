using System;
using Umbraco.Core;

namespace Our.Umbraco.Sidecar
{
    public class SidecarApplication : BaseHttpApplication
    {
        protected override IRuntime GetRuntime() => new SidecarRuntime();

        public void Start()
        {
            base.Application_Start(this, new EventArgs());
        }

        public void Stop()
        {
            base.Application_End(this, new EventArgs());
        }
    }
}