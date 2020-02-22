using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Swashbuckle.AspNetCore.Annotations;
using Yokogawa.IIoT.AzureHttpFunctionSwagger;

namespace TestApi
{
    public class PostRequestDto
    {
        public string inputProperty { get; set; }
    }

    public class PostRequestResponse
    {
        public int Count { get; set; }
    }

    public class PostRequest
    {
        [FunctionName(nameof(FakeFunctionOne))]
        [SwaggerRequest(required: true, In = RequestSource.Body, Type = typeof(PostRequestDto))]
        [SwaggerRequest(
            required: true,
            In = RequestSource.Header,
            Description = "adasdada",
            Name = "header parameter",
            Type = typeof(string))]

        [SwaggerRequest(
            required: false,
            In = RequestSource.Query,
            Description = "query parameter",
            Name = "QueryParameter",
            Type = typeof(string))]

        [SwaggerRequest(
            required: true,
            In = RequestSource.Path,
            Description = "Path parameter description",
            Name = "PathParameter",
            Type = typeof(string))]
        [SwaggerResponse(
            201,
            Description = "Created response description",
            Type = typeof(PostRequestResponse))]
        public IActionResult FakeFunctionOne(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "testroute")] HttpRequest request)
        {
            return new OkResult();
        }
    }
}