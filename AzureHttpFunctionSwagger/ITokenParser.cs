using System.Collections.Generic;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public interface ITokenParser
    {
        IEnumerable<string> Parse(string input);
    }
}