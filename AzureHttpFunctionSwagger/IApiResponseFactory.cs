using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public interface IApiResponseFactory
    {
        ApiResponseType CreateResponse(int statusCode, Type type);
    }
}