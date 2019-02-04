using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenScrappingAzureFunctionDemo.Services.Formatters
{
    /// <inheritdoc />
    /// <summary>
    ///     This represents the formatter entity for "text/plain" mime type.
    /// </summary>
    public class TextPlainMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ScreenScrappingAzureFunctionDemo.Services.Formatters.TextPlainMediaTypeFormatter" /> class.
        /// </summary>
        public TextPlainMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        /// <inheritdoc />
        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
        {
            var buff = Encoding.UTF8.GetBytes(value.ToString());
            return writeStream.WriteAsync(buff, 0, buff.Length, cancellationToken);
        }
    }
}
