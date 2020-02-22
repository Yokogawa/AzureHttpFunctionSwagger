using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Tests
{
    public class TestHttpFunction
    {
        /// <summary>
        /// This dto describes the input
        /// </summary>
        public class FakeFunctionOneInputDto
        {
            /// <summary>
            /// This property is a string
            /// </summary>
            public string StringProperty { get; set; }

            /// <summary>
            /// This property is an integer
            /// </summary>
            public int IntProperty { get; set; }
        }

        /// <summary>
        /// This response is returned from a post request
        /// </summary>
        public class FakeFunctionOneCreatedResponse
        {
            /// <summary>
            /// This is a response message parameter
            /// </summary>
            public string ResponseMessage { get; set; }
        }

        public const string route = nameof(FakeFunctionOne) + "/route";
        public const string HeaderParameterDescription = nameof(HeaderParameterDescription);
        public const string HeaderParameterSummary = nameof(HeaderParameterSummary);
        public const string HeaderParameterName = nameof(HeaderParameterName);
        public const bool HeaderParameterRequired = true;

        public const string PathParameterDescription = nameof(PathParameterDescription);
        public const string PathParameterSummary = nameof(PathParameterSummary);
        public const string PathParameterName = nameof(PathParameterName);
        public const bool PathParameterRequired = true;

        public const string QueryParameterDescription = nameof(QueryParameterDescription);
        public const string QueryParameterSummary = nameof(QueryParameterSummary);
        public const string QueryParameterName = nameof(QueryParameterName);
        public const bool QueryParameterRequired = true;

        public const int CreatedResponseStatusCode = 201;
        public const string CreatedResponseDescription = nameof(CreatedResponseDescription);

        [FunctionName(nameof(FakeFunctionOne))]
        [SwaggerRequest(required: true, In = RequestSource.Body, Type = typeof(FakeFunctionOneInputDto))]
        [SwaggerRequest(
            required: HeaderParameterRequired,
            In = RequestSource.Header, 
            Description = HeaderParameterDescription,
            Name = HeaderParameterName,
            Type = typeof(string))]

        [SwaggerRequest(
            required: QueryParameterRequired,
            In = RequestSource.Query,
            Description = QueryParameterDescription,
            Name = QueryParameterName,
            Type = typeof(string))]

        [SwaggerRequest(
            required: PathParameterRequired,
            In = RequestSource.Path,
            Description = PathParameterDescription,
            Name = PathParameterName,
            Type = typeof(string))]
        [SwaggerResponse(
            CreatedResponseStatusCode, 
            Description = CreatedResponseDescription, 
            Type = typeof(FakeFunctionOneCreatedResponse))]
        public async Task<IActionResult> FakeFunctionOne(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = route)] HttpRequest request)
        {
            return new OkResult();
        }
    }
}
