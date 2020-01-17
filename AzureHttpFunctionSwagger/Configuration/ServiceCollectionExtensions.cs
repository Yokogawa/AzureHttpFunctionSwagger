using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.Buffers;
using System.Collections.Generic;
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
            var swashbuckleInfo = new Info
            {
                Description = options?.Description,
                Title = options?.Title,
                Version = options?.Version,
                TermsOfService = options?.TermsOfService?.ToString(),

            };
            if (options?.License != null)
            {
                var license = new License
                {
                    Name = options?.License?.Name,
                    Url = options?.License?.Url?.ToString()
                };
                swashbuckleInfo.License = license;
            }

            if (options?.Contact != null)
            {
                swashbuckleInfo.Contact = new Contact
                {
                    Email = options?.Contact?.Email,
                    Name = options?.Contact?.Name,
                    Url = options?.Contact?.Url?.ToString()
                };
            }
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options?.Version, swashbuckleInfo);
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
            services.AddMvcCore().AddJsonFormatters().AddApiExplorer();
            services.AddSingleton<ITokenParser, TokenParser>();
            services.AddSingleton<IApiResponseFactory, ApiResponseFactory>();
            services.AddSingleton<IParameterDescriptionFactory, ParameterDescriptionFactory>();
            services.AddSingleton<IApiDescriptionGroupCollectionProvider, FunctionApiDescriptionGroupCollectionProvider>();
            services.AddSingleton<IOutputFormatter>(c =>
                new JsonOutputFormatter(new JsonSerializerSettings(), ArrayPool<char>.Create()));
            services.AddSingleton<IModelMetadataProvider>(c => new DefaultModelMetadataProvider(
                new DefaultCompositeMetadataDetailsProvider(
                    new List<IMetadataDetailsProvider>()
                    {
                        new DefaultValidationMetadataProvider(),
                    })));
            services.AddSingleton<IFunctionHttpMethodProvider, FunctionHttpMethodProvider>();

            // add default mappers
            services.AddSingleton<IMapApiParameterDescriptions, SwaggerRequestParameterMapper>();
            services.AddSingleton<IMapApiResponses, SwaggerResponseTypeMapper>();
            services.AddSingleton<IMapApiResponses, ProducesResponseTypeMapper>();
        }
    }
}
