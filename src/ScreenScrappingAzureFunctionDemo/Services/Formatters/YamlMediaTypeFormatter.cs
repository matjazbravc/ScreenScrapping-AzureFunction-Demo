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
    /// <summary>
    /// What is YAML?
    /// YAML, which stands for "YAML Ain't Markup Language", is described as "a human friendly data serialization standard for 
    /// all programming languages". Like XML, it allows to represent about any kind of data in a portable, 
    /// platform-independent format.Unlike XML, it is "human friendly", which means that it is easy for a human to read or 
    /// produce a valid YAML document.
    /// This represents the formatter entity for "application/yaml" and "application/x-yaml" mime type.
    /// More info: http://aaubry.net/pages/yamldotnet.html
    /// </summary>
    public class YamlMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlMediaTypeFormatter" /> class.
        /// </summary>
        public YamlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/yaml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-yaml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/yaml"));
        }

        /// <inheritdoc />
        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        /// <inheritdoc />
        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        /// <inheritdoc />
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
        {
            var buff = Encoding.UTF8.GetBytes(value.ToString());
            return writeStream.WriteAsync(buff, 0, buff.Length, cancellationToken);
        }
    }
}
