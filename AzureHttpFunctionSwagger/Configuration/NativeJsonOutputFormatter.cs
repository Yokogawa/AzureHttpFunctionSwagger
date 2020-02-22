#if NETCOREAPP3_0
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger.Configuration
{
    public class NativeJsonOutputFormatter: TextOutputFormatter
    {
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            using (var writer = context.WriterFactory(response.Body, selectedEncoding))
            {
                await writer
                    .WriteAsync(new StringBuilder(JsonSerializer.Serialize(context.Object)))
                    .ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
#endif