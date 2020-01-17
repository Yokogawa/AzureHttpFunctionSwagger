using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public class SwaggerRequestParameterMapper : IMapApiParameterDescriptions
    {
        private readonly IParameterDescriptionFactory _parameterDescriptionFactory;

        public SwaggerRequestParameterMapper(IParameterDescriptionFactory parameterDescriptionFactory)
        {
            _parameterDescriptionFactory = parameterDescriptionFactory;
        }

        public IEnumerable<ApiParameterDescription> Map(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            var attributes = methodInfo.GetCustomAttributes(typeof(SwaggerRequestAttribute), false);
            var parameters = methodInfo.GetParameters().ToDictionary(x => x.Name, x => x);
            foreach (SwaggerRequestAttribute attribute in attributes)
            {
                if (attribute == null)
                    continue;
                if (attribute.In == RequestSource.Body
                    && attribute.Type != null)
                {
                    yield return _parameterDescriptionFactory.Create(
                            bindingSource: BindingSource.Body,
                            name: attribute.Type.Name,
                            type: attribute.Type,
                            isOptional: false);
                    continue;
                }

                var isOptional = !attribute.Required;
                var bindingSource = attribute.In == RequestSource.Header ? BindingSource.Header
                    : attribute.In == RequestSource.Path ? BindingSource.Path
                    : attribute.In == RequestSource.Query ? BindingSource.Query
                    : null;
                if (bindingSource != null)
                    yield return _parameterDescriptionFactory.Create(
                            bindingSource: bindingSource,
                            name: attribute.Name,
                            type: attribute.Type,
                            isOptional: !attribute.Required);
            }
        }
    }
}
