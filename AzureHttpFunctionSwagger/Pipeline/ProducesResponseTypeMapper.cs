using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public class ProducesResponseTypeMapper : IMapApiResponses
    {
        private readonly IApiResponseFactory _responseFactory;

        public ProducesResponseTypeMapper(IApiResponseFactory responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public IEnumerable<ApiResponseType> Map(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            var attributes = methodInfo.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false);
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() != typeof(ProducesResponseTypeAttribute))
                    continue;
                var typedAttribute = attribute as ProducesResponseTypeAttribute;
                yield return _responseFactory.CreateResponse(typedAttribute.StatusCode, typedAttribute.Type);
            }
        }
    }
}
