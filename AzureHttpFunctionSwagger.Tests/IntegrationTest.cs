using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Yokogawa.IIoT.AzureHttpFunctionSwagger.Configuration;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Tests
{
    public abstract class IntegrationTest
    {
        protected static IHost _host => new HostBuilder()
            .ConfigureWebJobs(x =>
            {
                x.AddHttp();
            })
            .ConfigureServices(services =>
            {
                services.AddFunctionSwagger(commentsFiles: new[] { "AzureHttpFunctionSwagger.Tests.xml" });
            })
            .Build();
        protected JobHost _jobHost => _host.Services.GetService<IJobHost>() as JobHost;
        protected ISwaggerProvider _swaggerProvider => _host.Services.GetService<ISwaggerProvider>();
        protected OpenApiInfo _openApiInfo => _host.Services.GetService<OpenApiInfo>();
        protected OpenApiDocument _document => _swaggerProvider.GetSwagger(_openApiInfo.Version);
    }
}
