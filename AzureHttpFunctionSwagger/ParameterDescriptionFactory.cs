using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class ParameterDescriptionFactory : IParameterDescriptionFactory
    {
        private readonly IModelMetadataProvider _metadataProvider;

        public ParameterDescriptionFactory(IModelMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public ApiParameterDescription Create(
            BindingSource bindingSource,
            string name,
            Type type,
            bool isOptional = true)
        {
            var modelType = _metadataProvider.GetMetadataForType(type);
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
                ModelMetadata = modelType,
                RouteInfo = new ApiParameterRouteInfo
                {
                    IsOptional = isOptional
                }
            };
        }
    }
}
