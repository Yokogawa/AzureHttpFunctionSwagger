# Installation

[Nuget](https://www.nuget.org/packages/Yokogawa.IIoT.AzureHttpFunctionSwagger/)

# Purpose

This package provides the Swagger UI and JSON endpoints for Azure Function apps with Http triggered functions. The goal was to reuse as many of the Swagger attributes as possible to allow an easier transition from traditional API projects to Azure Function HTTP APIs. There was also a secondary goal to provide a pipeline for flexibility, in the event that there was a need to support custom attribute implementations. Those implementations can be provided by the developers consuming this package.

# Configuration


## Swagger Http Functions

This repository contains two http triggered functions that provide the resources for Swagger. You can either copy those functions into your endpoint, use them as a starting point, or enable `FunctionsInDependencies` in the build package `Microsoft.NET.Sdk.Functions` >= v3.0.5

example csproj:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <AzureFunctionsVersion>v3</AzureFunctionsVersion>
    <FunctionsInDependencies>true</FunctionsInDependencies>
  </PropertyGroup>
</Project>
```

`<FunctionsInDependencies>true</FunctionsInDependencies>` will find functions in referenced packages and compile/deploy them alongside your app.

## Function Startup
example startup.cs
```csharp
using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Yokogawa.IIoT.AzureHttpFunctionSwagger.Configuration;
using Microsoft.OpenApi.Models;

[assembly: FunctionsStartup(typeof(MyApi.Startup))]
namespace MyApi
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder?.Services ?? throw new ApplicationException("Something has gone wrong. Services is not available.");

            var executionContextOptions = services
                ?.BuildServiceProvider()
                ?.GetService<IOptions<ExecutionContextOptions>>()
                ?.Value;

            var appDirectory = executionContextOptions?.AppDirectory ??
                               Environment.CurrentDirectory;

            services.AddFunctionSwagger(new OpenApiInfo
            {
                Description = "My Api Description",
                Title = "My API Title",
                Version = "1"
            }, $"{appDirectory}\\MyApi.xml"); // optional: xml documentation file. Specifying this file will include the xml comments on types from the assembly in the swagger document.
        }
    }
}
```

# How To Use

This package relies on primarily Function Attributes to describe API actions.

**Swagger Operation**

```csharp
[SwaggerOperation(
            OperationId = "uniqueOperationId",
            Tags = new[] { "TagGroupOne", "TagGroupTwo" },
            Description = "A lengthy description of this action",
            Summary = "A short summary")]
```

**Body**

```csharp
[Body(typeof(InputDto), Required = false)]
```

**Swagger Request**

```csharp
[SwaggerRequest(In = RequestSource.Header, Type = typeof(string), Name = "parameterName")]
```

**Swagger Response**

```csharp
[SwaggerResponse((int)HttpStatusCode.Created, "application/json", typeof(ResponseDto))]
```

**Ignore Endpoint**
```csharp
[ApiExplorerSettings(IgnoreApi = true)] 
```

# Newtonsoft Json Integration

To integrate with the Newtonsoft library and take advantage of property attributes like `JsonProperty`

Install the package `Swashbuckle.AspNetCore.Newtonsoft` from nuget

Then add the following line in your startup after `AddFunctionSwagger`

`services.AddSwaggerGenNewtonsoftSupport();`

# Custom Attributes

You can create your own attributes to describe the swagger operations.

**IMapApiParameterDescriptions**

Implementing this interface with a custom attribute and registering the type in the dependency injection container will allow you customize the creation of parameter descriptions in the swagger document.

[SwaggerRequestAttribute mapping](https://github.com/Yokogawa/AzureHttpFunctionSwagger/blob/master/AzureHttpFunctionSwagger/Pipeline/SwaggerRequestParameterMapper.cs)

[Register in the DI container](https://github.com/Yokogawa/AzureHttpFunctionSwagger/blob/master/AzureHttpFunctionSwagger/Configuration/ServiceCollectionExtensions.cs#L95)

**IMapApiResponses**

Implementing this interface with a custom attribute and registering the type in the dependency injection container will allow you customize the responses in the swagger document.

[SwaggerResponseAttribute mapping](https://github.com/Yokogawa/AzureHttpFunctionSwagger/blob/master/AzureHttpFunctionSwagger/Pipeline/SwaggerResponseTypeMapper.cs)

[Register in the DI container](https://github.com/Yokogawa/AzureHttpFunctionSwagger/blob/master/AzureHttpFunctionSwagger/Configuration/ServiceCollectionExtensions.cs#L96)
