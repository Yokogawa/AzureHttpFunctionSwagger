using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Reflection;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Pipeline
{
    public interface IMapApiResponses
    {
        IEnumerable<ApiResponseType> Map(MethodInfo methodInfo);
    }
}
