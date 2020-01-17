using Microsoft.Azure.WebJobs;
using System.Reflection;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class HttpFunctionData
    {
        public string Name { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public HttpTriggerAttribute TriggerAttribute { get; set; }
        public ParameterInfo TriggerParameter { get; set; }
    }

}
