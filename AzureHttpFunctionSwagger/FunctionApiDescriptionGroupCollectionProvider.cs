using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
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
        private readonly IOptions<MvcOptions> _mvcOptions;
        private MediaTypeCollection _contentTypes;

        public FunctionApiDescriptionGroupCollectionProvider(
            IFunctionHttpMethodProvider functionDataProvider,
            ITokenParser tokenParser,
            IEnumerable<IMapApiResponses> responseMappings,
            IEnumerable<IMapApiParameterDescriptions> parameterMappings,
            IParameterDescriptionFactory parameterDescriptionFactory,
            ILogger<FunctionApiDescriptionGroupCollectionProvider> logger,
            IApiRequestFormatMetadataProvider formatMetadataProvider,
            IOptions<MvcOptions> mvcOptions,
            IEnumerable<IApiDescriptionProvider> apiDescriptionProviders,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _functionDataProvider = functionDataProvider;
            _tokenParser = tokenParser;
            _responseMappings = responseMappings;
            _parameterMappings = parameterMappings;
            _parameterDescriptionFactory = parameterDescriptionFactory;
            _logger = logger;
            _mvcOptions = mvcOptions;
            _contentTypes = new MediaTypeCollection();
            _contentTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
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

                // map body if available
                var bodyAttributes = method
                    .MethodInfo
                    .GetCustomAttributes(typeof(SwaggerRequestAttribute), false)
                    .Where(x => (x as SwaggerRequestAttribute)?.In == RequestSource.Body)
                    .Select(x => x as SwaggerRequestAttribute);

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
                    var description = new FunctionApiDescription(
                        methodInfo: method.MethodInfo,
                        name: method.Name,
                        parameters: apiParameterDescriptions,
                        verb: httpVerb,
                        route: route,
                        responseTypes: apiResponseTypes);

                    foreach (var bodyAttribute in bodyAttributes)
                    {
                        var requestFormats = GetSupportedFormats(bodyAttribute.Type);
                        foreach (var format in requestFormats)
                        {
                            description.SupportedRequestFormats.Add(format);
                        }
                    }

                    descriptions.Add(description);
                }
            }
            var groups = new ApiDescriptionGroup("default", descriptions);
            return new ApiDescriptionGroupCollection(new[] { groups }, version: 1);
        }

        public ApiDescriptionGroupCollection ApiDescriptionGroups => Generate();

        private IReadOnlyList<ApiRequestFormat> GetSupportedFormats(Type type)
        {
            var results = new List<ApiRequestFormat>();
            foreach (var contentType in _contentTypes)
            {
                foreach (var formatter in _mvcOptions?.Value.InputFormatters)
                {
                    if (formatter is IApiRequestFormatMetadataProvider requestFormatMetadataProvider)
                    {
                        var supportedTypes = requestFormatMetadataProvider.GetSupportedContentTypes(contentType, type);

                        if (supportedTypes != null)
                        {
                            foreach (var supportedType in supportedTypes)
                            {
                                results.Add(new ApiRequestFormat()
                                {
                                    Formatter = formatter,
                                    MediaType = supportedType,
                                });
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}