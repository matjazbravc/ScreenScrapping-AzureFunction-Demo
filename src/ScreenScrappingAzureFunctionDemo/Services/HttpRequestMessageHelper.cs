using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using ScreenScrappingAzureFunctionDemo.Services.Enums;
using ScreenScrappingAzureFunctionDemo.Services.Extensions;
using ScreenScrappingAzureFunctionDemo.Services.Formatters;
using ScreenScrappingAzureFunctionDemo.Services.Ioc;
using ScreenScrappingAzureFunctionDemo.Services.Logging;
using ScreenScrappingAzureFunctionDemo.Services.Models;

namespace ScreenScrappingAzureFunctionDemo.Services
{
    public class HttpRequestMessageHelper
    {
        private readonly ILog _log;

        public HttpRequestMessageHelper(ILog log)
        {
            _log = log;
            _log.Debug();
        }

        private static readonly List<string> _validContentTypes = new[]
            {
                ContentType.ApplicationJson.GetDescription(), ContentType.ApplicationYaml.GetDescription()
            }.ToList();

        private static readonly List<string> _yamlMediaTypes = new[]
            {
                ContentType.ApplicationXYaml.GetDescription(), ContentType.ApplicationYaml.GetDescription(), ContentType.ApplicationTextYaml.GetDescription()
            }.ToList();

        private bool _disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            _log.Debug();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Checks whether the request contains payload or not.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <returns>Returns <c>True</c>, if the request contains payload; otherwise returns <c>False</c>.</returns>
        public async Task<bool> ContainsPayloadAsync(HttpRequestMessage req)
        {
            var content = await req.Content.ReadAsStringAsync().ConfigureAwait(false);
            return !content.IsNullOrWhiteSpace();
        }

        /// <summary>
        ///     Creates a 400 Bad Request response for a request.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <param name="message">The response content.</param>
        /// <returns>The 400 Bad Request response.</returns>
        public HttpResponseMessage CreateBadRequestResponse(HttpRequestMessage req, string message)
        {
            var errorResponse = new ErrorResponseModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = message
            };
            return CreateBadRequestResponse(req, errorResponse);
        }

        /// <summary>
        ///     Creates a 400 Bad Request response for a request.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="req">The request.</param>
        /// <param name="value">The response content.</param>
        /// <returns>The 400 Bad Request response.</returns>
        public HttpResponseMessage CreateBadRequestResponse<T>(HttpRequestMessage req, T value)
        {
            return CreateResponse(req, HttpStatusCode.BadRequest, value);
        }

        /// <summary>
        ///     Creates a 404 Not Found response for a request.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <param name="message">The response content.</param>
        /// <returns>The 404 Not Found response.</returns>
        public HttpResponseMessage CreateNotFoundResponse(HttpRequestMessage req, string message)
        {
            var errorResponse = new ErrorResponseModel
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = message
            };
            return CreateNotFoundResponse(req, errorResponse);
        }

        /// <summary>
        ///     Creates a 404 Not Found response for a request.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="req">The request.</param>
        /// <param name="value">The response content.</param>
        /// <returns>The 404 Not Found response.</returns>
        public HttpResponseMessage CreateNotFoundResponse<T>(HttpRequestMessage req, T value)
        {
            return CreateResponse(req, HttpStatusCode.NotFound, value);
        }

        /// <summary>
        ///     Creates a 200 OK response for a request.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="req">The request.</param>
        /// <param name="value">The response content.</param>
        /// <param name="mediaType">Media type.</param>
        /// <returns>The 200 OK response.</returns>
        public HttpResponseMessage CreateOkResponse<T>(HttpRequestMessage req, T value, string mediaType = null)
        {
            return CreateResponse(req, HttpStatusCode.OK, value, mediaType);
        }

        /// <summary>
        ///     Creates a response for a request.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="req">The request.</param>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="value">The response content.</param>
        /// <param name="mediaType">Media type.</param>
        /// <returns>The response.</returns>
        public HttpResponseMessage CreateResponse<T>(HttpRequestMessage req, HttpStatusCode statusCode, T value, string mediaType = null)
        {
            var formatter = _yamlMediaTypes.ContainsEquivalent(mediaType)
                                ? ServiceLocator.GetInstance<YamlMediaTypeFormatter>() as MediaTypeFormatter
                                : ServiceLocator.GetInstance<JsonMediaTypeFormatter>();
            return req.CreateResponse(statusCode, value, formatter);
        }

        /// <summary>
        ///     Creates a 415 Unsupported Media Type response for a request.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <param name="message">The response content.</param>
        /// <returns>The 415 Unsupported Media Type response.</returns>
        public HttpResponseMessage CreateUnsupportedMediaTypeResponse(HttpRequestMessage req, string message)
        {
            var errorResponse = new ErrorResponseModel
            {
                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                Message = message
            };
            return CreateNotFoundResponse(req, errorResponse);
        }

        /// <summary>
        ///     Creates a 415 Unsupported Media Type response for a request.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="req">The request.</param>
        /// <param name="value">The response content.</param>
        /// <returns>The 415 Unsupported Media Type response.</returns>
        public HttpResponseMessage CreateUnsupportedMediaTypeResponse<T>(HttpRequestMessage req, T value)
        {
            return CreateResponse(req, HttpStatusCode.UnsupportedMediaType, value);
        }

        /// <summary>
        /// Decompress HttpRequestMessage Content
        /// </summary>
        /// <param name="request">HttpRequestMessage request</param>
        /// <returns></returns>
        public StringContent DecompressHttpContent(HttpRequestMessage request)
        {
            if (request.Content.Headers.ContentEncoding == null)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Content.Headers.ContentEncoding), "is null.");
            }
            var encodingType = request.Content.Headers.ContentEncoding.ToString();
            Stream outputStream = new MemoryStream();
            var task = request.Content.ReadAsStreamAsync().ContinueWith(t =>
            {
                var inputStream = t.Result;
                if (encodingType == EncodingType.Gzip.GetDescription())
                {
                    using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(outputStream);
                    }
                }
                else
                {
                    using (var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(outputStream);
                    }
                }
                outputStream.Seek(0, SeekOrigin.Begin);
            });
            task.Wait();

            var stringContent = outputStream.GetString();
            var resultContent = new StringContent(stringContent, Encoding.UTF8, ContentType.ApplicationJson.GetDescription());
            request.Content = resultContent;
            return resultContent;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Value indicating whether to dispose managed resources or not.</param>
        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                ReleaseManagedResources();
            }
            ReleaseUnmanagedResources();
            _disposed = true;
        }

        /// <summary>
        ///     Checks whether the content type has valid content type or not.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <returns>Returns <c>True</c>, if the content type has valid content type; otherwise returns <c>False</c>.</returns>
        public bool IsValidContentType(HttpRequestMessage req)
        {
            var contentType = req.Content.Headers.ContentType.MediaType;
            return _validContentTypes.ContainsEquivalent(contentType);
        }

        /// <summary>
        /// Checks whether the contentis compressed
        /// </summary>
        /// <param name="request">HttpRequestMessage as request</param>
        /// <returns>Returns <c>True</c>, if the content is compressed; otherwise returns <c>False</c>.</returns>
        public bool IsContentCompressed(HttpRequestMessage request)
        {
            return request.Content.Headers.ContentEncoding != null &&
                   (request.Content.Headers.ContentEncoding.Contains(EncodingType.Gzip.GetDescription()) ||
                    request.Content.Headers.ContentEncoding.Contains(EncodingType.Deflate.GetDescription()));
        }

        /// <summary>
        ///     Releases managed resources during the disposing event.
        /// </summary>
        public virtual void ReleaseManagedResources()
        {
            // Release managed resources here.
        }

        /// <summary>
        ///     Releases unmanaged resources during the disposing event.
        /// </summary>
        public virtual void ReleaseUnmanagedResources()
        {
            // Release unmanaged resources here.
        }
    }
}
