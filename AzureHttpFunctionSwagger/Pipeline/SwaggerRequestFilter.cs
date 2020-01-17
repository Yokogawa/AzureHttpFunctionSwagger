using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public class SwaggerRequestFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation == null || context == null)
                return;
            var attributes = context.MethodInfo.GetCustomAttributes(typeof(SwaggerRequestAttribute), false);
            foreach (SwaggerRequestAttribute attribute in attributes)
            {
                if (attribute == null)
                    continue;
                IParameter parameter = null;
                if (string.IsNullOrWhiteSpace(attribute.Name))
                {
                    if (attribute.In != RequestSource.Body)
                    {
                        continue;
                    }
                    parameter = operation.Parameters.SingleOrDefault(x => x.In.ToUpperInvariant() == "BODY");
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
