using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.Http;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Base class for building endpoints that use ETags and Last-Modified timestamps for caching and to avoid lost updates.
    /// </summary>
    public abstract class CachingEndpointBase : EndpointBase, ICachingEndpoint
    {
        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        protected CachingEndpointBase(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        protected CachingEndpointBase(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public ResponseCache? ResponseCache { get; set; }

        /// <summary>
        /// Performs an HTTP GET request on the <see cref="IEndpoint.Uri"/> and caches the response if the server sends an <see cref="HttpResponseHeaders.ETag"/>.
        /// </summary>
        /// <remarks>Sends If-None-Match header if there is already a cached ETag.</remarks>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <param name="caller">The name of the method calling this method.</param>
        /// <returns>The response of the request or the cached response if the server responded with <see cref="HttpStatusCode.NotModified"/>.</returns>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/> or empty response body</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected async Task<HttpContent> GetContentAsync(CancellationToken cancellationToken, [CallerMemberName] string caller = "unknown")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uri);
            var cache = ResponseCache; // Copy reference for thread-safety
            cache?.SetIfModifiedHeaders(request.Headers);

            var response = await HttpClient.SendAsync(request, cancellationToken).NoContext();
            if (response.StatusCode == HttpStatusCode.NotModified && cache != null && !cache.IsExpired)
                return cache.GetContent();
            else
            {
                await HandleAsync(() => Task.FromResult(response), caller).NoContext();
                ResponseCache = ResponseCache.From(response);
                return response.Content;
            }
        }

        /// <summary>
        /// Performs an <see cref="HttpMethod.Put"/> request on the <see cref="IEndpoint.Uri"/>. Sets <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached <see cref="HttpResponseHeader.ETag"/> to detect lost updates.
        /// </summary>
        /// <param name="content">The content to send to the server.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <param name="caller">The name of the method calling this method.</param>
        /// <returns>The response message.</returns>
        /// <exception cref="InvalidOperationException">The content has changed since it was last retrieved with <see cref="GetContentAsync"/>. Your changes were rejected to prevent a lost update.</exception>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected Task<HttpResponseMessage> PutContentAsync(HttpContent content, CancellationToken cancellationToken, [CallerMemberName] string caller = "unknown")
        {
            var request = new HttpRequestMessage(HttpMethod.Put, Uri) {Content = content};
            var cache = ResponseCache; // Copy reference for thread-safety
            cache?.SetIfUnmodifiedHeaders(request.Headers);

            ResponseCache = null;
            return HandleAsync(() => HttpClient.SendAsync(request, cancellationToken), caller);
        }

        /// <summary>
        /// Performs an <see cref="HttpMethod.Delete"/> request on the <see cref="IEndpoint.Uri"/>. Sets <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached ETag to detect lost updates.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <param name="caller">The name of the method calling this method.</param>
        /// <returns>The response message.</returns>
        /// <exception cref="InvalidOperationException">The content has changed since it was last retrieved with <see cref="GetContentAsync"/>. Your changes were rejected to prevent a lost update.</exception>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected Task<HttpResponseMessage> DeleteContentAsync(CancellationToken cancellationToken, [CallerMemberName] string caller = "unknown")
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, Uri);
            var cache = ResponseCache; // Copy reference for thread-safety
            cache?.SetIfUnmodifiedHeaders(request.Headers);

            ResponseCache = null;
            return HandleAsync(() => HttpClient.SendAsync(request, cancellationToken), caller);
        }
    }
}
