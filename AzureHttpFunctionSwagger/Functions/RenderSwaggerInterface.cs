using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Functions
{
    public class RenderSwaggerInterface
    {
        private const string embeddedFileNamespace = "Swashbuckle.AspNetCore.SwaggerUI.node_modules.swagger_ui_dist";
        private const string SwaggerUiRoute = "swagger/ui";
        private const string SwaggerIndexHtmlFile = "Swashbuckle.AspNetCore.SwaggerUI.index.html";
        private static Assembly SwashbuckleAssembly = typeof(SwaggerUIOptions).GetTypeInfo().Assembly;

        [FunctionName(nameof(SwaggerUiJs))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SwaggerUiJs(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/{fileName}")] HttpRequest req,
        string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return new NotFoundResult();

            var fileProvider = new EmbeddedFileProvider(SwashbuckleAssembly, embeddedFileNamespace);
            var fileInfo = fileProvider.GetFileInfo(fileName);
            if (!fileInfo.Exists)
                return new NotFoundResult();
            var fileExtension = fileName?.Substring(fileName.LastIndexOf('.') + 1);
            var contentType = "";
            switch (fileExtension.ToUpperInvariant())
            {
                case "JS":
                    contentType = MediaTypeConstants.Text.Javascript;
                    break;
                case "CSS":
                    contentType = MediaTypeConstants.Text.Css;
                    break;
                case "MAP":
                    contentType = MediaTypeNames.Application.Json;
                    break;
                case "PNG":
                    contentType = MediaTypeConstants.Image.Png;
                    break;
                default:
                    break;
            }
            using (var readStream = fileInfo.CreateReadStream())
            using (var reader = new StreamReader(readStream))
            {
                return new ContentResult()
                {
                    Content = reader.ReadToEnd(),
                    ContentType = contentType,
                    StatusCode = 200
                };
            }
        }

        [FunctionName(nameof(RenderSwaggerUI))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ContentResult RenderSwaggerUI(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SwaggerUiRoute)] HttpRequest req)
        {
            using (var stream = SwashbuckleAssembly.GetManifestResourceStream(SwaggerIndexHtmlFile))
            using (var streamReader = new StreamReader(stream))
            {
                var htmlBuilder = new StringBuilder(streamReader.ReadToEnd());
                var options = new SwaggerUIOptions();
                var configObject = options.ConfigObject;
                var urls = new List<UrlDescriptor>(configObject.Urls ?? Enumerable.Empty<UrlDescriptor>());
                var swaggerDocumentUrl = req.GetEncodedUrl().Replace(SwaggerUiRoute, RenderSwaggerDocument.SwaggerJsonRoute, System.StringComparison.OrdinalIgnoreCase);
                urls.Add(new UrlDescriptor
                {
                    Url = swaggerDocumentUrl
                });
                configObject.Urls = urls;
                foreach (var entry in GetIndexArguments(options))
                {
                    htmlBuilder.Replace(entry.Key, entry.Value);
                }
                return new ContentResult()
                {
                    Content = htmlBuilder.ToString(),
                    ContentType = MediaTypeConstants.Text.Html,
                    StatusCode = 200
                };
            }
        }

        private static IDictionary<string, string> GetIndexArguments(SwaggerUIOptions options)
        {
            return new Dictionary<string, string>()
            {
                { "%(DocumentTitle)", "DocumentTitle" },
                { "%(HeadContent)", options.HeadContent },
                { "%(ConfigObject)", SerializeToJson(options.ConfigObject) },
                { "%(OAuthConfigObject)", SerializeToJson(options.OAuthConfigObject) }
            };
        }

        private static string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new[] { new StringEnumConverter(true) },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });
        }
    }
}
