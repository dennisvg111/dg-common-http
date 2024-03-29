﻿using System;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Represents the uri data of a cookie, based on <see cref="ICookie.Path"/>, <see cref="ICookie.Domain"/>, and <see cref="ICookie.OriginUri"/>.
    /// </summary>
    public class CookieUriData
    {
        private readonly Uri _originUri;
        private readonly string _trimmedDomain;
        private readonly string _path;

        private readonly Lazy<string> _matchPath;

        /// <summary>
        /// Initializes a new instance of <see cref="CookieUriData"/> based on the given <see cref="ICookie"/>.
        /// </summary>
        /// <param name="cookie"></param>
        public CookieUriData(ICookie cookie)
        {
            _originUri = cookie.OriginUri;
            _path = cookie.Path;

            _trimmedDomain = cookie.Domain;
            if (_trimmedDomain?.StartsWith(".", StringComparison.Ordinal) ?? false)
            {
                _trimmedDomain = _trimmedDomain.Substring(1);
            }

            _matchPath = new Lazy<string>(() => GetMatchPath());
        }

        /// <summary>
        /// Returns a value indicating if this <see cref="CookieUriData"/> matches <paramref name="requestUri"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public bool IsMatch(Uri requestUri)
        {
            return IsDomainMatch(requestUri) && IsPathMatch(requestUri);
        }

        /// <summary>
        /// Returns a value indicating if the domain part of this <see cref="CookieUriData"/> matches <paramref name="requestUri"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public bool IsDomainMatch(Uri requestUri)
        {
            if (string.IsNullOrEmpty(_trimmedDomain))
            {
                return requestUri.Host.Equals(_originUri.Host, StringComparison.OrdinalIgnoreCase);
            }

            if (requestUri.Host.Equals(_trimmedDomain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (requestUri.Host.EndsWith("." + _trimmedDomain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a value indicating if the path part of this <see cref="CookieUriData"/> matches <paramref name="requestUri"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public bool IsPathMatch(Uri requestUri)
        {
            var cookiePath = _matchPath.Value;
            if (cookiePath == "/")
            {
                return true;
            }

            var requestPath = (requestUri.AbsolutePath.Length > 0) ? requestUri.AbsolutePath : "/";

            //case-sensitive
            if (requestPath.Equals(cookiePath, StringComparison.Ordinal))
            {
                return true;
            }
            if (requestPath.Length > cookiePath.Length && requestPath.StartsWith(cookiePath, StringComparison.Ordinal) && requestPath[cookiePath.Length] == '/')
            {
                return true;
            }

            return false;
        }

        private string GetMatchPath()
        {
            string cookiePath = _path;
            if (string.IsNullOrEmpty(cookiePath) || !cookiePath.StartsWith("/", StringComparison.Ordinal))
            {
                return cookiePath = GetDefaultPath();
            }
            if (cookiePath != null && cookiePath.EndsWith("/", StringComparison.Ordinal))
            {
                cookiePath = cookiePath.TrimEnd('/');
            }
            return cookiePath;
        }

        private string GetDefaultPath()
        {
            string cookiePath = _originUri.AbsolutePath ?? "";
            if (string.IsNullOrEmpty(cookiePath))
            {
                return "/";
            }
            int lastIndex = cookiePath.LastIndexOf('/', 1);
            if (cookiePath[0] != '/' || lastIndex < 0)
            {
                return "/";
            }
            return cookiePath.Substring(0, lastIndex);
        }
    }
}
