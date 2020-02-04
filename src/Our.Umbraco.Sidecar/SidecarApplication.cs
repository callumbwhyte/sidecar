using System;
using Umbraco.Core;
using Umbraco.Web;

namespace Our.Umbraco.Sidecar
{
    public class SidecarApplication : UmbracoApplicationBase
    {
        public void Start()
        {
            base.Application_Start(this, new EventArgs());
        }

        protected override IRuntime GetRuntime() => new SidecarRuntime();
    }
}