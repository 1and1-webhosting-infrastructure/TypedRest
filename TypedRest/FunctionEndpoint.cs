﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents an RPC-like function which returns <typeparamref name="TResult"/> as output.
    /// </summary>
    /// <typeparam name="TResult">The type of entity the endpoint returns as output.</typeparam>
    public class FunctionEndpoint<TResult> : TriggerEndpointBase, IFunctionEndpoint<TResult>
    {
        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public FunctionEndpoint(IEndpoint parent, Uri relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public FunctionEndpoint(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        public async Task<TResult> TriggerAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            HttpResponseMessage response =
                await
                    HandleResponseAsync(HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, Uri),
                        cancellationToken));

            return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted
                ? await response.Content.ReadAsAsync<TResult>(cancellationToken)
                : default(TResult);
        }
    }

    /// <summary>
    /// REST endpoint that represents an RPC-like function which takes <typeparamref name="TEntity"/> as input and returns <typeparamref name="TResult"/> as output.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
    /// <typeparam name="TResult">The type of entity the endpoint returns as output.</typeparam>
    public class FunctionEndpoint<TEntity, TResult> : TriggerEndpointBase, IFunctionEndpoint<TEntity, TResult>
    {
        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public FunctionEndpoint(IEndpoint parent, Uri relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public FunctionEndpoint(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        public async Task<TResult> TriggerAsync(TEntity entity,
            CancellationToken cancellationToken = new CancellationToken())
        {
            HttpResponseMessage response =
                await HandleResponseAsync(HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken));

            return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted
                ? await response.Content.ReadAsAsync<TResult>(cancellationToken)
                : default(TResult);
        }
    }
}