using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public interface IParameterDescriptionFactory
    {
        ApiParameterDescription Create(BindingSource bindingSource, string name, Type type, bool isOptional = true);
    }
}