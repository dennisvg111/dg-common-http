using DG.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// This class provides extension methods for <see cref="UriBuilder"/>.
    /// </summary>
    public static class UriBuilderExtensions
    {
        /// <summary>
        /// Gets the current Query part of a <see cref="UriBuilder"/> as a collection of name value pairs.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetQueryParts(this UriBuilder builder)
        {
            var query = builder.Query;
            query = query.TrimStart('?');
            var currentParts = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in currentParts)
            {
                if (!part.Contains("="))
                {
                    yield return new KeyValuePair<string, string>(Uri.UnescapeDataString(part), null);
                    continue;
                }
                var name = part.Substring(0, part.IndexOf("="));
                var value = part.Substring(part.IndexOf("=") + 1);
                yield return new KeyValuePair<string, string>(Uri.UnescapeDataString(name), Uri.UnescapeDataString(value));
            }
        }

        /// <summary>
        /// Sets the Query part of a <see cref="UriBuilder"/> based on a collection of name value pairs.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="queryParts"></param>
        public static void SetQueryParts(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> queryParts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var part in queryParts)
            {
                sb.Append(Uri.EscapeDataString(part.Key));
                if (part.Value != null)
                {
                    sb.Append("=" + Uri.EscapeDataString(part.Value));
                }
                sb.Append("&");
            }
            builder.Query = sb.ToString().TrimEnd('&');
        }

        /// <summary>
        /// Adds a new value to the current query. Returns the current <see cref="UriBuilder"/>, to allow this method to be called using a fluent pattern.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UriBuilder WithQuery(this UriBuilder builder, string value)
        {
            return builder.WithQuery(value, null);
        }

        /// <summary>
        /// Adds a new name value pair to the current query. Returns the current <see cref="UriBuilder"/>, to allow this method to be called using a fluent pattern.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UriBuilder WithQuery(this UriBuilder builder, string name, string value)
        {
            return builder.WithQueryParts(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(name, value) });
        }

        /// <summary>
        /// Adds a collection of name value pairs to the current query. Returns the current <see cref="UriBuilder"/>, to allow this method to be called using a fluent pattern.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="newQueryParts"></param>
        /// <returns></returns>
        public static UriBuilder WithQueryParts(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> newQueryParts)
        {
            List<KeyValuePair<string, string>> parts = builder.GetQueryParts().ToList();
            foreach (var kv in newQueryParts)
            {
                AddParts(parts, kv.Key, kv.Value);
            }
            builder = new UriBuilder(builder.Uri);
            builder.SetQueryParts(parts);
            return builder;
        }

        private static void AddParts(List<KeyValuePair<string, string>> oldQueryParts, string name, string value)
        {
            if (name == null)
            {
                name = value;
                value = null;
            }
            ThrowIf.Parameter.IsNull(name, nameof(name), "Name and value of query cannot both be null.");
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), "Name and value of query cannot both be null.");
            }
            oldQueryParts.Add(new KeyValuePair<string, string>(name, value));
        }
    }
}
