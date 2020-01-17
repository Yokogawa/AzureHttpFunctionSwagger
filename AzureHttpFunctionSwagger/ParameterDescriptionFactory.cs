using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class ParameterDescriptionFactory : IParameterDescriptionFactory
    {
        public ApiParameterDescription Create(
            BindingSource bindingSource,
            string name,
            Type type,
            bool isOptional = true)
        {
            return new ApiParameterDescription
            {
                Name = name,
                Type = type,
                Source = bindingSource,
                ParameterDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor
                {
                    Name = name,
                    ParameterType = type
                },
                RouteInfo = new ApiParameterRouteInfo
                {
                    IsOptional = isOptional
                }
            };
        }
    }
}
