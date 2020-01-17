using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    public class TokenParser : ITokenParser
    {
        private static Regex _regex = new Regex(@"\{\w+\}");
        public IEnumerable<string> Parse(string input)
        {
            foreach (Match match in _regex.Matches(@input))
            {
                yield return match.Value.Substring(1, match.Value.Length - 2);
            }
        }
    }
}
