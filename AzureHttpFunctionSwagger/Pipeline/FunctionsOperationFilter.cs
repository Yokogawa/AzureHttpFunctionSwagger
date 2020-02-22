using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null)
                return;
            var attributes = context.MethodInfo.GetCustomAttributes(typeof(SwaggerOperationAttribute), false);
            if (attributes.Length != 1)
                return;

            var descriptionAttribute = attributes[0] as SwaggerOperationAttribute;

            if (!string.IsNullOrWhiteSpace(descriptionAttribute.Description))
                operation.Description = descriptionAttribute.Description;

            if (!string.IsNullOrWhiteSpace(descriptionAttribute.OperationId))
                operation.OperationId = descriptionAttribute.OperationId;

            if (!string.IsNullOrWhiteSpace(descriptionAttribute.Summary))
                operation.Summary = descriptionAttribute.Summary;

            if (descriptionAttribute.Tags != null && descriptionAttribute.Tags.Length > 0)
                operation.Tags = descriptionAttribute.Tags.Select(x => new OpenApiTag
                {
                    Name = x
                }).ToList();
        }
    }
}
