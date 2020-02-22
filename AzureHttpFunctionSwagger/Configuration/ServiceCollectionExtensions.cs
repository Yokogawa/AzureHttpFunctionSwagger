using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using System.IO;
using Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Configuration
{
    public static class ServiceCollectionExtensions
    {
        private static OpenApiInfo _defaultOptions = new OpenApiInfo
        {
            Description = "Function App API",
            Title = "Function App",
            Version = "1"
        };

        public static void AddFunctionSwagger(
            this IServiceCollection services,
            OpenApiInfo apiOptions = null,
            params string[] commentsFiles)
        {
            if (services == null)
                return;
            var options = apiOptions ?? _defaultOptions;
            services.AddSingleton(options);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options?.Version, apiOptions);
                c.DescribeAllEnumsAsStrings();

                foreach (var commentsFile in commentsFiles)
                {
                    if (!File.Exists(commentsFile))
                        continue;
                    c.IncludeXmlComments(commentsFile);
                }
                c.OperationFilter<SwaggerOperationFilter>();
                c.OperationFilter<SwaggerResponseFilter>();
                c.OperationFilter<SwaggerRequestFilter>();
            });
            services.AddSingleton<IApiDescriptionGroupCollectionProvider, ApiDescriptionGroupCollectionProvider>();
            services.Add(ServiceDescriptor.Transient<IApiDescriptionProvider, DefaultApiDescriptionProvider>());

            services.AddSingleton<ITokenParser, TokenParser>();
            services.AddSingleton<IApiResponseFactory, ApiResponseFactory>();
            services.AddSingleton<IParameterDescriptionFactory, ParameterDescriptionFactory>();
            services.AddSingleton<IApiDescriptionGroupCollectionProvider, FunctionApiDescriptionGroupCollectionProvider>();

            services.AddSingleton<IOutputFormatter>(c => new NativeJsonOutputFormatter());
            services.AddSingleton<IModelMetadataProvider, DefaultModelMetadataProvider>();

            services.AddSingleton<IFunctionHttpMethodProvider, FunctionHttpMethodProvider>();

            // add default mappers
            services.AddSingleton<IMapApiParameterDescriptions, SwaggerRequestParameterMapper>();
            services.AddSingleton<IMapApiResponses, SwaggerResponseTypeMapper>();
            services.AddSingleton<IMapApiResponses, ProducesResponseTypeMapper>();
        }
    }
}