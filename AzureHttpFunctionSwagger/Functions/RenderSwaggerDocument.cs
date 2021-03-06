﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.OpenApi.Writers;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Functions
{
    public class RenderSwaggerDocument
    {
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly IApiDescriptionGroupCollectionProvider _collectionProvider;
        private readonly IEnumerable<IApiDescriptionProvider> _descriptionProviders;
        private readonly OpenApiInfo _info;
        public const string SwaggerJsonRoute = "swagger.json";

        public RenderSwaggerDocument(
            ISwaggerProvider swaggerProvider,
            IApiDescriptionGroupCollectionProvider collectionProvider,
            IEnumerable<IApiDescriptionProvider> descriptionProviders,
            OpenApiInfo info)
        {
            _swaggerProvider = swaggerProvider;
            _collectionProvider = collectionProvider;
            _descriptionProviders = descriptionProviders;
            _info = info;
        }

        [FunctionName(nameof(RenderSwaggerDocument))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SwaggerJsonRoute)] HttpRequest req)
        {
            try
            {
                var document = _swaggerProvider.GetSwagger(_info.Version);
                using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    var jsonWriter = new OpenApiJsonWriter(stringWriter);
                    document.SerializeAsV3(jsonWriter);
                    return new OkObjectResult(stringWriter.ToString());
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                return new ObjectResult(ex.Demystify())
                {
                    StatusCode = 500
                };
#endif
                return new ObjectResult("")
                {
                    StatusCode = 500
                };
            }
        }
    }
}
