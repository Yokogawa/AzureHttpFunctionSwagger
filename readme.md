# Installation

[Nuget](https://www.nuget.org/packages/Yokogawa.IIoT.AzureHttpFunctionSwagger/)

# Purpose

This package provides the Swagger UI and JSON endpoints for Azure Function apps with Http triggered functions. 

# Configuration


## Swagger Http Functions

This repository contains two http triggered functions that provide the resources for Swagger. You can either copy those functions into your endpoint, use them as a starting point, or enable `FunctionsInDependencies` in the build package `Microsoft.NET.Sdk.Functions` >= v1.0.31

example csproj:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netcoreapp2.2</TargetFramework>
    <AzureFunctionsVersion>v2</AzureFunctionsVersion>
    <FunctionsInDependencies>true</FunctionsInDependencies>
  </PropertyGroup>
</Project>
```

`<FunctionsInDependencies>true</FunctionsInDependencies>` will find functions in referenced packages and compile/deploy them alongside your app.

## Function Startup
example startup.cs
```csharp
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
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
            builder.Services.AddFunctionSwagger(new OpenApiInfo
            {
                Description = "My Api Description",
                Title = "My API Title",
                Version = "1"
            }, $"{container.AppDirectory}\\MyApi.xml"); // optional: xml documentation file. Specifying this file will include the xml comments on types from the assembly in the swagger document.

            container
                .Services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddApiExplorer();
        }
    }
}
```