using DG.Common.Http.Authorization;
using System;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Represents an HTTP header in a request or response.
    /// </summary>
    public class FluentHeader
    {
        private readonly string _name;
        private readonly Func<string> _value;
        private readonly Func<bool> _apply;

        /// <summary>
        /// The name of this header.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The value of this header.
        /// </summary>
        public string Value => _value();

        /// <summary>
        /// <para>Indicates if this header should be applied to a request. For headers initialized using <see cref="FluentHeader.FluentHeader(string, string)"/> this always returns <see langword="true"/>.</para>
        /// <para>This is used with some special headers like <c>Authorization</c> headers where <see cref="IAuthorizationHeaderProvider.IsAuthorized"/> is <see langword="false"/>.</para>
        /// </summary>
        public bool ShouldApply => _apply();

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> with the given <paramref name="name"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="apply"></param>
        private FluentHeader(string name, Func<string> value, Func<bool> apply)
        {
            _name = name;
            _value = value;
            _apply = apply;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> with the given <paramref name="name"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public FluentHeader(string name, string value) : this(name, () => value, () => true) { }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header with the given header provider.
        /// </summary>
        /// <param name="authorizationHeaderProvider"></param>
        /// <returns></returns>
        public static FluentHeader Authorization(IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            return new FluentHeader("Authorization", () => authorizationHeaderProvider.GetAuthorizationHeaderValue(), () => authorizationHeaderProvider.IsAuthorized);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FluentHeader Authorization(string value)
        {
            return new FluentHeader("Authorization", value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header with the given header value.
        /// </summary>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        public static FluentHeader Authorization(AuthorizationHeader headerValue)
        {
            return Authorization(headerValue.GetHeaderValue());
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header based on the given authorization provider.
        /// </summary>
        /// <param name="expiringAuthorizationProvider"></param>
        /// <returns></returns>
        public static FluentHeader Authorization(IExpiringAuthorizationProvider expiringAuthorizationProvider)
        {
            var headerProvider = new ExpiringAuthorizationHeaderProvider(expiringAuthorizationProvider);
            return Authorization(headerProvider);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for a <c>Content-Length</c> header with the given content length in bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static FluentHeader ContentLength(long bytes)
        {
            return new FluentHeader("Content-Length", bytes.ToString());
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for a <c>Content-Range</c> header with the given content range.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="bytes"></param>
        /// <param name="totalBytes"></param>
        /// <returns></returns>
        public static FluentHeader ContentRange(long startIndex, long? bytes, long? totalBytes)
        {
            string total = totalBytes.HasValue ? totalBytes.Value.ToString() : "*";

            if (bytes.HasValue)
            {
                var endIndex = startIndex + bytes.Value - 1;
                total = $"bytes {startIndex}-{endIndex}/{total}";
            }
            else
            {
                total = "bytes */" + total;
            }
            return new FluentHeader("Content-Range", total);
        }

        /// <summary>
        /// <para>Initializes a new instance of <see cref="FluentHeader"/> for a <c>Referer</c> header with the given referer uri.</para>
        /// <para>Note that the header name is Referer, not the correctly spelled Referrer.</para>
        /// </summary>
        /// <param name="referrer"></param>
        /// <returns></returns>
        public static FluentHeader Referrer(Uri referrer)
        {
            return new FluentHeader("Referer", referrer.ToString());
        }

        /// <summary>
        /// Returns a string representing the current headername and value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
