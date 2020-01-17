using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class FunctionApiDescription : ApiDescription
    {
        public FunctionApiDescription(
            MethodInfo methodInfo,
            string name,
            IList<ApiParameterDescription> parameters,
            string verb,
            string route,
            IList<ApiResponseType> responseTypes)
        {
            ActionDescriptor = new ControllerActionDescriptor
            {
                MethodInfo = methodInfo,
                ControllerName = name,
                DisplayName = name,
                ControllerTypeInfo = methodInfo?.DeclaringType?.GetTypeInfo() ?? throw new ArgumentNullException(nameof(methodInfo)),
                Parameters = parameters
                                .Where(x => x.ParameterDescriptor != null)
                                .Select(x => x.ParameterDescriptor).ToList(),
                RouteValues = new Dictionary<string, string>
                {
                    { "controller", name },
                    { "action", name}
                },
                ActionConstraints = new List<IActionConstraintMetadata>
                {
                    new HttpMethodActionConstraint(new[] { verb })
                }
            };
            RelativePath = route;
            HttpMethod = verb?.ToUpperInvariant();

            if (responseTypes != null)
                foreach (var responseType in responseTypes)
                {
                    SupportedResponseTypes.Add(responseType);
                }

            if (parameters != null)
                foreach (var parameter in parameters)
                {
                    ParameterDescriptions.Add(parameter);
                }
        }
    }
}
