using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TestApi;
using Yokogawa.IIoT.AzureHttpFunctionSwagger.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TestApi
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddFunctionSwagger(new OpenApiInfo
            {
                Description = "Test API",
                Title = "Test API",
                Version = "1"
            });
        }
    }
}