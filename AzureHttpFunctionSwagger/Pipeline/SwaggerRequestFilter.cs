using System.Collections.Concurrent;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public class SwaggerRequestFilter : IOperationFilter
    {
        public SwaggerRequestFilter()
        {
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null)
                return;
            var attributes = context.MethodInfo.GetCustomAttributes(typeof(SwaggerRequestAttribute), false);
            foreach (SwaggerRequestAttribute attribute in attributes)
            {
                if (attribute == null)
                    continue;
                OpenApiParameter parameter = null;
                if (string.IsNullOrWhiteSpace(attribute.Name))
                {
                    if (attribute.In != RequestSource.Body)
                    {
                        continue;
                    }

                    if (operation?.RequestBody?.Content != null
                        && operation.RequestBody.Content.Any())
                    {
                        operation.RequestBody.Required = attribute.Required;
                        operation.RequestBody.Description = attribute.Description;
                        return;
                    }
                    operation.RequestBody = new OpenApiRequestBody();
                    operation.RequestBody.Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                            }
                        }
                    };
                }
                else
                {
                    parameter = operation.Parameters.SingleOrDefault(x => x.Name == attribute.Name);
                }
                if (parameter == null)
                    continue;

                parameter.Description = attribute.Description;
                parameter.Required = attribute.Required;
            }
        }
    }
}