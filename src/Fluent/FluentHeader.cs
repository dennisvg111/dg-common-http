using DG.Common.Http.Authorization;
using System;
using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Represents an HTTP header in a request or response.
    /// </summary>
    public class FluentHeader
    {
        private readonly string _name;
        private readonly Func<string> _value;
        private readonly bool _isContentHeader;
        private readonly bool _withoutValidation;
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
        /// <para>Indicates if this header is a content header, when used for a request.</para>
        /// <para>Content headers get set to <see cref="HttpContent.Headers"/> instead of <see cref="HttpRequestMessage.Headers"/>.</para>
        /// </summary>
        public bool IsContentHeader => _isContentHeader;

        /// <summary>
        /// <para>Indicates if this header should be applied to a request. For headers initialized using <see cref="FluentHeader.FluentHeader(string, string, bool, bool)"/> this always returns <see langword="true"/>.</para>
        /// <para>This is used with some special headers like <c>Authorization</c> headers where <see cref="IAuthorizationHeaderProvider.IsAuthorized"/> is <see langword="false"/>.</para>
        /// </summary>
        public bool ShouldApply => _apply();

        /// <summary>
        /// Indicates if this header should be validated when adding it to a request.
        /// </summary>
        public bool SkipValidation => _withoutValidation;

        private FluentHeader(string name, Func<string> value, bool isContentHeader, bool withoutValidation, Func<bool> apply)
        {
            _name = name;
            _value = value;
            _isContentHeader = isContentHeader;
            _withoutValidation = withoutValidation;
            _apply = apply;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> with the given <paramref name="name"/> and <paramref name="value"/>, and optionally a value indicating if this header is a content header.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isContentHeader"></param>
        /// <param name="withoutValidation"></param>
        public FluentHeader(string name, string value, bool isContentHeader = false, bool withoutValidation = false)
            : this(name, () => value, isContentHeader, withoutValidation, () => true) { }

        /// <summary>
        /// <para>Starts the initialization of a <see cref="FluentHeader"/> for a header with the given name.</para>
        /// <para>Note this should be followed by calling <see cref="IFluentHeaderBuilder.AndValue(string)"/> on the result.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFluentHeaderBuilder WithName(string name)
        {
            return new FluentHeaderBuilder(name);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for a <c>User-Agent</c> header with the given value.
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static FluentHeader UserAgent(string userAgent)
        {
            return new FluentHeader("User-Agent", userAgent);
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
        /// Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header with the given <see cref="FluentAuthorization"/>.
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static FluentHeader Authorization(FluentAuthorization authorization)
        {
            return Authorization(authorization.ToString());
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header with the given header provider.
        /// </summary>
        /// <param name="authorizationHeaderProvider"></param>
        /// <returns></returns>
        public static FluentHeader Authorization(IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            return new FluentHeader("Authorization", () => authorizationHeaderProvider.GetAuthorizationHeaderValue().ToString(), false, false, () => true);
        }

        /// <summary>
        /// <para>Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header with the given header provider.</para>
        /// <para>This header will only be applied if <see cref="IAuthorizationHeaderProvider.IsAuthorized"/> returns <see langword="true"/></para>
        /// </summary>
        /// <param name="authorizationHeaderProvider"></param>
        /// <returns></returns>
        public static FluentHeader OptionalAuthorization(IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            return new FluentHeader("Authorization", () => authorizationHeaderProvider.GetAuthorizationHeaderValue().ToString(), false, false, () => authorizationHeaderProvider.IsAuthorized);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for a <c>Content-Length</c> header with the given content length in bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static FluentHeader ContentLength(long bytes)
        {
            return new FluentHeader("Content-Length", bytes.ToString(), true);
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
            return new FluentHeader("Content-Range", total, true);
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
        /// Returns a copy of this header that should not be validated before adding it to a request.
        /// </summary>
        /// <returns></returns>
        public FluentHeader WithoutValidation()
        {
            return new FluentHeader(_name, _value, _isContentHeader, true, _apply);
        }

        /// <summary>
        /// Returns a string representing the current headername and value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}: {Value}";
        }


        internal class FluentHeaderBuilder : IFluentHeaderBuilder
        {
            private readonly string _name;

            internal FluentHeaderBuilder(string name)
            {
                _name = name;
            }

            public FluentHeader AndValue(string value)
            {
                return new FluentHeader(_name, value);
            }
        }

        /// <summary>
        /// Defines a way to create an instance of <see cref="FluentHeader"/> when the name is already known.
        /// </summary>
        public interface IFluentHeaderBuilder
        {
            /// <summary>
            /// Initializes a new instance of <see cref="FluentHeader"/> for a header with the previously specified name and the given <paramref name="value"/>.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            FluentHeader AndValue(string value);
        }
    }
}
