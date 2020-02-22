using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
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
        private IHost _host;
        private JobHost _jobHost;
        private ISwaggerProvider _swaggerProvider;
        private OpenApiInfo _openApiInfo;
        protected OpenApiDocument _document;

        public IntegrationTest()
        {
            _host = new HostBuilder()
                .ConfigureWebJobs(x =>
                {
                    x.AddHttp();
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore().AddApiExplorer();
                    services.AddFunctionSwagger(new OpenApiInfo()
                    {
                        Description = "Test API",
                        Title = "Test API",
                        Version = "1"
                    }, commentsFiles: new[] { "AzureHttpFunctionSwagger.netcore3.Tests.xml" });
                })
                .Build();

            _jobHost = _host.Services.GetService<IJobHost>() as JobHost;
            // todo: fix these tests, some dependency injection error is happening
            _swaggerProvider = _host.Services.GetService<ISwaggerProvider>();
            _openApiInfo = _host.Services.GetService<OpenApiInfo>();
            _document = _swaggerProvider.GetSwagger(_openApiInfo.Version);
        }
    }
}