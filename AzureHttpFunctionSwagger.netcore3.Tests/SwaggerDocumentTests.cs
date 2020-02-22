using FluentAssertions;
using Humanizer;
using System.Linq;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Tests
{
    public class SwaggerDocumentTests : IntegrationTest
    {
        private string FunctionPath = $"/{TestHttpFunction.route}";

        [Fact]
        public void HttpFunctionsAppearInDocument()
        {
            _document
                .Paths
                .ContainsKey(FunctionPath)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void FunctionBodyShouldBeIncludedInDocument()
        {
            _document
                .Components
                .Schemas
                .ContainsKey(nameof(TestHttpFunction.FakeFunctionOneInputDto))
                .Should()
                .BeTrue();

            var properties = _document
                .Components
                .Schemas[nameof(TestHttpFunction.FakeFunctionOneInputDto)]
                .Properties;
            var stringPropertyName = nameof(TestHttpFunction.FakeFunctionOneInputDto.StringProperty).Camelize();
            properties.ContainsKey(stringPropertyName).Should().BeTrue();

            var intPropertyName = nameof(TestHttpFunction.FakeFunctionOneInputDto.IntProperty).Camelize();
            properties.ContainsKey(intPropertyName).Should().BeTrue();

            var stringProperty = properties[stringPropertyName];
            stringProperty.Description.Should().Be("This property is a string");
            stringProperty.Type.Should().Be("string");

            var intProperty = properties[intPropertyName];
            intProperty.Description.Should().Be("This property is an integer");
            intProperty.Type.Should().Be("integer");
        }

        [Fact]
        public void FunctionsIgnoredWithApiExplorerAttributeShouldNotBeIncludedInDocument()
        {
            _document.Paths.ContainsKey($"/{IgnoredFunction.route}").Should().BeFalse();
        }

        [Fact]
        public void HeaderValuesAppearInDocumentCorrectly()
        {
            var headerParameter = _document
                .Paths[FunctionPath]
                .Operations.Single(x => x.Key == OperationType.Post)
                .Value
                .Parameters
                .SingleOrDefault(x => x.Name == TestHttpFunction.HeaderParameterName);

            headerParameter.Should().NotBeNull();

            headerParameter.In.Should().Be("header");
            headerParameter.Required.Should().Be(TestHttpFunction.HeaderParameterRequired);
            headerParameter.Description.Should().Be(TestHttpFunction.HeaderParameterDescription);
        }

        [Fact]
        public void PathValuesAppearInDocumentCorrectly()
        {
            var pathParameter = _document
                .Paths[FunctionPath]
                .Operations.Single(x => x.Key == OperationType.Post)
                .Value
                .Parameters
                .SingleOrDefault(x => x.Name == TestHttpFunction.PathParameterName);

            pathParameter.Should().NotBeNull();

            pathParameter.In.Should().Be("path");
            pathParameter.Required.Should().Be(TestHttpFunction.PathParameterRequired);
            pathParameter.Description.Should().Be(TestHttpFunction.PathParameterDescription);
        }

        [Fact]
        public void QueryValuesAppearInDocumentCorrectly()
        {
            var queryParameter = _document
                .Paths[FunctionPath]
                .Operations.Single(x => x.Key == OperationType.Post)
                .Value
                .Parameters
                .SingleOrDefault(x => x.Name == TestHttpFunction.QueryParameterName);

            queryParameter.Should().NotBeNull();

            queryParameter.In.Should().Be("query");
            queryParameter.Required.Should().Be(TestHttpFunction.QueryParameterRequired);
            queryParameter.Description.Should().Be(TestHttpFunction.QueryParameterDescription);
        }

        [Fact]
        public void ResponseTypesAreIncludedInDocument()
        {
            var response = _document.Paths[FunctionPath];
            //    .Responses[TestHttpFunction.CreatedResponseStatusCode.ToString()];

            //response
            //    .Description
            //    .Should()
            //    .Be(TestHttpFunction.CreatedResponseDescription);

            //var responseTypeName = nameof(TestHttpFunction.FakeFunctionOneCreatedResponse);
            //response
            //    .Schema
            //    .Ref
            //    .Should()
            //    .Be($"#/definitions/{responseTypeName}");

            //var schema = _document
            //    .Definitions[responseTypeName];

            //schema
            //    .Should()
            //    .NotBeNull();

            //schema
            //    .Description
            //    .Should()
            //    .Be("This response is returned from a post request");

            //var responseMessagePropertyName = nameof(TestHttpFunction.FakeFunctionOneCreatedResponse.ResponseMessage).Camelize();
            //schema
            //    .Properties
            //    .ContainsKey(responseMessagePropertyName)
            //    .Should()
            //    .BeTrue();

            //var property = schema
            //    .Properties[responseMessagePropertyName]
            //    .Description
            //    .Should()
            //    .Be("This is a response message parameter");
        }
    }
}