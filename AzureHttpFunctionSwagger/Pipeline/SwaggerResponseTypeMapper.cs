using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public class SwaggerResponseTypeMapper : IMapApiResponses
    {
        private readonly IApiResponseFactory _responseFactory;

        public SwaggerResponseTypeMapper(IApiResponseFactory responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public IEnumerable<ApiResponseType> Map(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            var attributes = methodInfo.GetCustomAttributes(typeof(SwaggerResponseAttribute), false);
            foreach (SwaggerResponseAttribute attribute in attributes)
            {
                yield return _responseFactory.CreateResponse(attribute.StatusCode, attribute.Type);
            }
        }
    }
}
