﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint, i.e. a remote HTTP resource.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// The HTTP client used to communicate with the remote resource.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// The HTTP URI of the remote resource.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Retrieves all links with a specific relation type cached from the last request.
        /// </summary>
        /// <param name="rel">The relation type of the links to look for.</param>
        /// <returns>The hrefs of the links resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link with the specified <paramref name="rel"/> could be found.</exception>
        IEnumerable<Uri> GetLinks(string rel);

        /// <summary>
        /// Retrieves a single link with a specific relation type.
        /// May be cached from the last request or may be lazily requested.
        /// </summary>
        /// <param name="rel">The relation type of the link to look for.</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link with the specified <paramref name="rel"/> could be found.</exception>
        Uri Link(string rel);

        /// <summary>
        /// Retrieves a link template with a specific relation type.
        /// May be cached from the last request or may be lazily requested.
        /// </summary>
        /// <param name="rel">The relation type of the link template to look for. "-template" is appended implicitly for HTTP Link Headers.</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI; <c>null</c> if no link with the specified <paramref name="rel"/> could be found.</returns>
        UriTemplate LinkTemplate(string rel);
    }
}