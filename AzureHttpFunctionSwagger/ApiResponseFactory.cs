using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class ApiResponseFactory : IApiResponseFactory
    {
        private readonly IOutputFormatter _outputFormatter;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public ApiResponseFactory(
            IOutputFormatter outputFormatter,
            IModelMetadataProvider modelMetadataProvider)
        {
            _outputFormatter = outputFormatter;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public ApiResponseType CreateResponse(int statusCode, Type type)
        {
            var responseType = new ApiResponseType
            {
                StatusCode = statusCode
            };
            if (type == null || type == typeof(void))
                return responseType;

            responseType.Type = type;
            responseType.ModelMetadata = _modelMetadataProvider.GetMetadataForType(type);
            responseType.ApiResponseFormats = new List<ApiResponseFormat>
            {
                new ApiResponseFormat
                {
                    Formatter = _outputFormatter,
                    MediaType = MediaTypeNames.Application.Json
                }
            };
            return responseType;
        }
    }
}
