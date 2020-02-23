using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Our.Umbraco.Sidecar;
using Umbraco.Core.Composing;

namespace Sidecar.Function
{
    public static class GetContent
    {
        [FunctionName("GetContent")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            req.SetConfiguration(new HttpConfiguration());

            var application = new SidecarApplication();

            application.Start();

            var contentService = Current.Services.ContentService;

            var contentItems = contentService.GetRootContent();

            var mediaService = Current.Services.MediaService;

            var mediaItems = mediaService.GetRootMedia();

            return req.CreateResponse(HttpStatusCode.OK, "Hello world");
        }
    }
}