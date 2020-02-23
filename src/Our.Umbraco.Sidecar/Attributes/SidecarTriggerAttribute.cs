using System;

namespace Our.Umbraco.Sidecar.Attributes
{
    public class SidecarTriggerAttribute : Attribute
    {
        public string Cron { get; set; }
    }
}