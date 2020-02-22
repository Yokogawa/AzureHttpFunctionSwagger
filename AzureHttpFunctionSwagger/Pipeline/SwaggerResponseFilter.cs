using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public class SwaggerResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null)
                return;
            var responses = context.MethodInfo.GetCustomAttributes(typeof(SwaggerResponseAttribute), false);
            foreach (SwaggerResponseAttribute response in responses)
            {
                var statusCode = response?.StatusCode;
                if (statusCode == null)
                    continue;

                if (!operation.Responses.TryGetValue(statusCode.ToString(), out var swaggerResponse))
                    continue;

                swaggerResponse.Description = response.Description;
            }
        }
    }
}
