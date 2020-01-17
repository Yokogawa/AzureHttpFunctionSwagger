using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class FunctionHttpMethodProvider : IFunctionHttpMethodProvider
    {
        private readonly ITypeLocator _typeLocator;

        public FunctionHttpMethodProvider(ITypeLocator typeLocator)
        {
            _typeLocator = typeLocator;
        }

        public IEnumerable<HttpFunctionData> GetMethods()
        {
            foreach (var method in _typeLocator.GetTypes().SelectMany(x => x.GetMethods()))
            {
                var explorerSettings = method.GetCustomAttribute<ApiExplorerSettingsAttribute>(false);
                if (explorerSettings != null && explorerSettings.IgnoreApi)
                    continue;
                var functionNameAttribute = method.GetCustomAttribute<FunctionNameAttribute>(false);
                if (functionNameAttribute == null)
                    continue;
                if (!TryGetParameterWithAttribute<HttpTriggerAttribute>(method, out var httpTriggerParameter, out var httpTriggerAttribute))
                    continue;
                yield return new HttpFunctionData
                {
                    MethodInfo = method,
                    Name = functionNameAttribute.Name,
                    TriggerAttribute = httpTriggerAttribute,
                    TriggerParameter = httpTriggerParameter
                };
            }
        }

        private bool TryGetParameterWithAttribute<TAttribute>(
            MethodInfo method,
            out ParameterInfo parameterToReturn,
            out TAttribute attributeToReturn) where TAttribute : Attribute
        {
            foreach (var parameter in method.GetParameters())
            {
                foreach (var attribute in parameter.GetCustomAttributes(false))
                {
                    if (attribute.GetType() == typeof(TAttribute))
                    {
                        parameterToReturn = parameter;
                        attributeToReturn = attribute as TAttribute;
                        return true;
                    }
                }
            }

            parameterToReturn = null;
            attributeToReturn = null;
            return false;
        }
    }

}
