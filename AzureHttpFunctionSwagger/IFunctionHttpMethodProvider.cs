using System.Collections.Generic;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public interface IFunctionHttpMethodProvider
    {
        IEnumerable<HttpFunctionData> GetMethods();
    }
}