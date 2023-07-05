﻿using DG.Common.Caching;
using System;
using System.Net.Http;
using System.Runtime.Caching;

namespace DG.Common.Http.FluentBuilders
{
    /// <summary>
    /// This class provides a custom way to create instances of <see cref="HttpClient"/> to prevent socket exhaustion and DNS problems.
    /// </summary>
    public static class HttpClientProvider
    {
        //We cache clients to prevent socket exhaustion, and cache it only for 5 minutes to prevent DNS problems.
        private const string _cacheName = "DG.Common.Http Client Provider Cache";
        private static readonly TypedCache<HttpClient> _cache = new TypedCache<HttpClient>(
            ExpirationPolicy.ForAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            new MemoryCache(_cacheName)
        );

        /// <summary>
        /// <para>Gets a <see cref="HttpClient"/> instance constructed by the given <see cref="HttpClientBuilder"/>.</para>
        /// <para>Note that this may return the same instance multiple times if no new instance is needed.</para>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static HttpClient GetClientForBuilder(HttpClientBuilder builder)
        {
            return _cache.GetOrCreate(builder.CacheId, () => builder.Client);
        }
    }
}
