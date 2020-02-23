using System;

namespace Our.Umbraco.Sidecar.Attributes
{
    public class SidecarApplicationAttribute : Attribute, IDisposable
    {
        private SidecarApplication _application = new SidecarApplication();

        public SidecarApplicationAttribute()
        {
            _application.Start();
        }

        public void Dispose()
        {
            _application.Stop();
        }
    }
}