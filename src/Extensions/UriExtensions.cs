﻿using System;
using System.Linq;

namespace DG.Common.Http.Extensions
{
    /// <summary>
    /// This class provides extension methods for <see cref="Uri"/>.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Detects if the <see cref="Uri.Scheme"/> of the given <see cref="Uri"/> is HTTPS.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsSecure(this Uri uri)
        {
            return uri.Scheme == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// Get the root (combination of <see cref="Uri.Scheme"/>, <see cref="Uri.Host"/>, and <see cref="Uri.Port"/> if needed) for the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetRoot(this Uri uri)
        {
            return uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
        }

        /// <summary>
        /// Removes the last path segment of the given <see cref="Uri"/>, if it exists.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Uri RemoveLastSegment(this Uri uri)
        {
            var newSegments = uri.Segments.Take(uri.Segments.Length - 1).ToArray();
            var builder = new UriBuilder(uri);
            builder.Path = string.Concat(newSegments).TrimEnd('/');
            return builder.Uri;
        }

        /// <summary>
        /// Gets the default path of the given <see cref="Uri"/>, as defined by RFC 6265 section 5.1.4.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetCookieDefaultPath(this Uri uri)
        {
            string cookiePath = uri?.AbsolutePath ?? "";
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

        /// <summary>
        /// Gets the absolute <see cref="Uri"/> that is the result of the given original uri and the new (relative or absolute) uri given by the location header.
        /// </summary>
        /// <param name="originalUri"></param>
        /// <param name="newUri"></param>
        /// <returns></returns>
        public static Uri CombineForRedirectLocation(this Uri originalUri, Uri newUri)
        {
            string location = newUri?.OriginalString;

            if (location == null)
            {
                return originalUri;
            }

            if (Uri.IsWellFormedUriString(location, UriKind.Absolute))
            {
                return ReplaceFragmentIfNeeded(location, originalUri);
            }

            if (location.StartsWith("//", StringComparison.Ordinal))
            {
                return ReplaceFragmentIfNeeded(originalUri.Scheme + ":" + location, originalUri);
            }

            if (location.StartsWith("/", StringComparison.Ordinal))
            {
                string root = originalUri.GetRoot();
                return ReplaceFragmentIfNeeded(root + location, originalUri);
            }

            if (!location.StartsWith("?", StringComparison.Ordinal) && !location.StartsWith("#", StringComparison.Ordinal))
            {
                originalUri = originalUri.RemoveLastSegment();
            }

            string combinedUrl = CombineUrlParts(originalUri.GetRoot() + originalUri.AbsolutePath, location);
            return ReplaceFragmentIfNeeded(combinedUrl, originalUri);
        }

        private static Uri ReplaceFragmentIfNeeded(string formedUri, Uri originalUri)
        {
            Uri newUrl = new Uri(formedUri, UriKind.Absolute);
            if (string.IsNullOrEmpty(newUrl.Fragment) && !string.IsNullOrEmpty(originalUri.Fragment))
            {
                return new Uri(newUrl.AbsoluteUri + "" + originalUri.Fragment, UriKind.Absolute);
            }
            return newUrl;
        }

        private static string CombineUrlParts(string part1, string part2)
        {
            if (part1.EndsWith("?", StringComparison.Ordinal) || part2.StartsWith("?", StringComparison.Ordinal))
            {
                return CombineWithSingleSeperator(part1, part2, '?');
            }
            if (part1.EndsWith("#", StringComparison.Ordinal) || part2.StartsWith("#", StringComparison.Ordinal))
            {
                return CombineWithSingleSeperator(part1, part2, '#');
            }
            return CombineWithSingleSeperator(part1, part2, '/');
        }

        private static string CombineWithSingleSeperator(string part1, string part2, char separator)
        {
            if (string.IsNullOrEmpty(part1))
            {
                return part2;
            }
            if (string.IsNullOrEmpty(part2))
            {
                return part1;
            }
            return part1.TrimEnd(separator) + separator + part2.TrimStart(separator);
        }
    }
}
