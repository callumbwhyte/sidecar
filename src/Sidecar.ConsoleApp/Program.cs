using System;
using Our.Umbraco.Sidecar;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;

namespace Sidecar.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initialzing...");

            var application = new SidecarApplication();

            application.Start();

            Console.WriteLine("Umbraco loaded successfully");

            var contentService = Current.Services.ContentService;

            var contentItems = contentService.GetRootContent();

            var mediaService = Current.Services.MediaService;

            var mediaItems = mediaService.GetRootMedia();

            Console.ReadLine();
        }
    }
}