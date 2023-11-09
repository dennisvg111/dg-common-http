using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using static DG.Common.Http.Fluent.FluentFormContentBuilder;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Provides a way to build either <see cref="FormUrlEncodedContent"/> or <see cref="MultipartFormDataContent"/> using methods that can be chained.
    /// </summary>
    public class FluentFormContentBuilder : IFluentMultipartFormContentBuilder
    {
        private readonly List<KeyValuePair<string, string>> _stringContent;

        /// <summary>
        /// Builds a new instance of <see cref="FormUrlEncodedContent"/> with all currently added content.
        /// </summary>
        /// <returns></returns>
        public FormUrlEncodedContent BuildUrlEncodedForm()
        {
            return new FormUrlEncodedContent(_stringContent);
        }

        /// <inheritdoc/>
        public MultipartFormDataContent BuildMultipartForm()
        {
            var builder = new FluentMultipartFormContentBuilder(_stringContent);
            return builder.BuildMultipartForm();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentFormContentBuilder"/> with the given content list.
        /// </summary>
        /// <param name="content"></param>
        private FluentFormContentBuilder(List<KeyValuePair<string, string>> content)
        {
            _stringContent = content;
        }

        /// <inheritdoc/>
        public IFluentMultipartFormContentBuilder AndWith(string name, HttpContent content)
        {
            return new FluentMultipartFormContentBuilder(_stringContent, name, content);
        }

        /// <summary>
        /// Returns a copy of this content with the given name and value added.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public FluentFormContentBuilder AndWith(string name, string value)
        {
            var combinedContent = new List<KeyValuePair<string, string>>(_stringContent)
            {
                new KeyValuePair<string, string>(name, value)
            };
            return new FluentFormContentBuilder(combinedContent);
        }

        /// <summary>
        /// Returns a new instance of <see cref="IFluentMultipartFormContentBuilder"/> with the given <paramref name="content"/> added, using the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IFluentMultipartFormContentBuilder With(string name, HttpContent content)
        {
            return new FluentMultipartFormContentBuilder(name, content);
        }

        /// <summary>
        /// Returns a new instance of <see cref="FluentFormContentBuilder"/> with the given name and value added.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FluentFormContentBuilder With(string name, string value)
        {
            var content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(name, value)
            };
            return new FluentFormContentBuilder(content);
        }

        /// <inheritdoc/>
        IFluentMultipartFormContentBuilder IFluentMultipartFormContentBuilder.AndWith(string name, string value)
        {
            var newContent = new StringContent(value);
            return new FluentMultipartFormContentBuilder(_stringContent, name, newContent);
        }

        /// <summary>
        /// Defines functionality specifically for creating an instance of <see cref="MultipartFormDataContent"/>.
        /// </summary>
        public interface IFluentMultipartFormContentBuilder
        {
            /// <summary>
            /// Returns a copy of this form builder with the given <see cref="HttpContent"/> added, using the specified name.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="content"></param>
            /// <returns></returns>
            IFluentMultipartFormContentBuilder AndWith(string name, HttpContent content);

            /// <summary>
            /// Returns a copy of this form builder with the given <paramref name="value"/> added as <see cref="StringContent"/>, using the specified name.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            IFluentMultipartFormContentBuilder AndWith(string name, string value);

            /// <summary>
            /// Builds a new instance of <see cref="MultipartFormDataContent"/> with all currently added content.
            /// </summary>
            /// <returns></returns>
            MultipartFormDataContent BuildMultipartForm();
        }

        private sealed class FluentMultipartFormContentBuilder : IFluentMultipartFormContentBuilder
        {
            private readonly List<KeyValuePair<string, HttpContent>> _content;

            public FluentMultipartFormContentBuilder(List<KeyValuePair<string, string>> stringContent)
            {
                _content = stringContent.Select(c => new KeyValuePair<string, HttpContent>(c.Key, new StringContent(c.Value))).ToList();
            }

            public FluentMultipartFormContentBuilder(List<KeyValuePair<string, string>> stringContent, string name, HttpContent content) : this(stringContent)
            {
                _content.Add(new KeyValuePair<string, HttpContent>(name, content));
            }

            public FluentMultipartFormContentBuilder(string name, HttpContent content)
            {
                _content = new List<KeyValuePair<string, HttpContent>>()
                {
                    new KeyValuePair<string, HttpContent>(name, content)
                };
            }

            private FluentMultipartFormContentBuilder(List<KeyValuePair<string, HttpContent>> content)
            {
                _content = content;
            }

            /// <inheritdoc/>
            public IFluentMultipartFormContentBuilder AndWith(string name, HttpContent content)
            {
                var newContent = new KeyValuePair<string, HttpContent>(name, content);
                var combinedContent = new List<KeyValuePair<string, HttpContent>>(_content)
                {
                    newContent
                };
                return new FluentMultipartFormContentBuilder(combinedContent);
            }

            /// <inheritdoc/>
            public IFluentMultipartFormContentBuilder AndWith(string name, string value)
            {
                var newContent = new KeyValuePair<string, HttpContent>(name, new StringContent(value));
                var combinedContent = new List<KeyValuePair<string, HttpContent>>(_content)
                {
                    newContent
                };
                return new FluentMultipartFormContentBuilder(combinedContent);
            }

            /// <inheritdoc/>
            public MultipartFormDataContent BuildMultipartForm()
            {
                var formContent = new MultipartFormDataContent();
                _content.ForEach(kv => formContent.Add(kv.Value, kv.Key));
                return formContent;
            }
        }
    }
}
