using System;

namespace DG.Common.Http.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        /// Get the root (combination of <see cref="Uri.Scheme"/> and <see cref="Uri.Authority"/>) for the <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetRoot(this Uri uri)
        {
            return string.Concat(uri.Scheme, uri.Scheme?.Length > 0 ? ":" : "", uri.Authority?.Length > 0 ? "//" : "", uri.Authority);
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

            if (location.StartsWith("//"))
            {
                return ReplaceFragmentIfNeeded(originalUri.Scheme + ":" + location, originalUri);
            }

            if (location.StartsWith("/"))
            {
                string root = originalUri.GetRoot();
                return ReplaceFragmentIfNeeded(root + location, originalUri);
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
