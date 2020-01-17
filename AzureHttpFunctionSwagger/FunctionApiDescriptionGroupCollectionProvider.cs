using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class FunctionApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        private readonly IFunctionHttpMethodProvider _functionDataProvider;
        private readonly ITokenParser _tokenParser;
        private readonly IEnumerable<IMapApiResponses> _responseMappings;
        private readonly IEnumerable<IMapApiParameterDescriptions> _parameterMappings;
        private readonly IParameterDescriptionFactory _parameterDescriptionFactory;
        private readonly ILogger<FunctionApiDescriptionGroupCollectionProvider> _logger;

        public FunctionApiDescriptionGroupCollectionProvider(
            IFunctionHttpMethodProvider functionDataProvider,
            ITokenParser tokenParser,
            IEnumerable<IMapApiResponses> responseMappings,
            IEnumerable<IMapApiParameterDescriptions> parameterMappings,
            IParameterDescriptionFactory parameterDescriptionFactory,
            ILogger<FunctionApiDescriptionGroupCollectionProvider> logger)
        {
            _functionDataProvider = functionDataProvider;
            _tokenParser = tokenParser;
            _responseMappings = responseMappings;
            _parameterMappings = parameterMappings;
            _parameterDescriptionFactory = parameterDescriptionFactory;
            _logger = logger;
        }

        public ApiDescriptionGroupCollection Generate()
        {
            var descriptions = new List<ApiDescription>();
            foreach (var method in _functionDataProvider.GetMethods())
            {
                var route = method.TriggerAttribute.Route;
                var routeParameters = _tokenParser.Parse(route);
                var apiParameterDescriptions = new List<ApiParameterDescription>();
                var apiResponseTypes = new List<ApiResponseType>();

                foreach (var parameterMapping in _parameterMappings)
                {
                    try
                    {
                        apiParameterDescriptions.AddRange(parameterMapping.Map(method.MethodInfo) ?? Enumerable.Empty<ApiParameterDescription>());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error mapping parameters in api explorer");
                    }
                }

                foreach (var responseTypeMapping in _responseMappings)
                {
                    try
                    {
                        apiResponseTypes.AddRange(responseTypeMapping.Map(method.MethodInfo) ?? Enumerable.Empty<ApiResponseType>());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error mapping parameters in api explorer");
                    }
                }

                foreach (var methodParameter in method.MethodInfo.GetParameters())
                {
                    var matchingRouteParameter = routeParameters.SingleOrDefault(param => string.Equals(param, methodParameter.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (matchingRouteParameter != default
                        && !apiParameterDescriptions.Any(x => string.Equals(x.Name, matchingRouteParameter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var isOptional = matchingRouteParameter.EndsWith("?", StringComparison.InvariantCultureIgnoreCase);
                        apiParameterDescriptions.Add(_parameterDescriptionFactory.Create(
                            bindingSource: BindingSource.Path,
                            name: matchingRouteParameter,
                            type: methodParameter.ParameterType,
                            isOptional));
                        continue;
                    }
                }
                foreach (var httpVerb in method.TriggerAttribute.Methods)
                {
                    descriptions.Add(new FunctionApiDescription(
                        methodInfo: method.MethodInfo,
                        name: method.Name,
                        parameters: apiParameterDescriptions,
                        verb: httpVerb,
                        route: route,
                        responseTypes: apiResponseTypes));
                }
            }
            var groups = new ApiDescriptionGroup("default", descriptions);
            return new ApiDescriptionGroupCollection(new[] { groups }, version: 1);
        }

        public ApiDescriptionGroupCollection ApiDescriptionGroups => Generate();
    }
}