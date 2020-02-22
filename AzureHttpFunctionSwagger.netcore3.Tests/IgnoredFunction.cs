using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Tests
{
    public class IgnoredFunction
    {
        public const string route = "IgnoredFunctionRoute";

        [FunctionName(nameof(FakeFunctionOne))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void FakeFunctionOne([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = route)] HttpRequest request)
        {
        }
    }
}
