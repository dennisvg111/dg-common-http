using System.Collections.Generic;
using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Provides a way to build a <see cref="MultipartFormDataContent"/> using methods that can be chained.
    /// </summary>
    public class FluentFormContent
    {
        private readonly List<KeyValuePair<string, HttpContent>> _content;

        /// <summary>
        /// Initializes a new instance of <see cref="MultipartFormDataContent"/> with all currently added content.
        /// </summary>
        /// <returns></returns>
        public MultipartFormDataContent Build()
        {
            var formContent = new MultipartFormDataContent();
            _content.ForEach(kv => formContent.Add(kv.Value, kv.Key));
            return formContent;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentFormContent"/> with the given content list.
        /// </summary>
        /// <param name="content"></param>
        public FluentFormContent(List<KeyValuePair<string, HttpContent>> content)
        {
            _content = content;
        }

        /// <summary>
        /// Returns a copy of this content with the given <paramref name="content"/> added, using the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public FluentFormContent AndWith(string name, HttpContent content)
        {
            var combinedContent = new List<KeyValuePair<string, HttpContent>>(_content);
            combinedContent.Add(new KeyValuePair<string, HttpContent>(name, content));
            return new FluentFormContent(combinedContent);
        }

        /// <summary>
        /// Returns a copy of this content with the given <paramref name="value"/> added as <see cref="StringContent"/>, using the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public FluentFormContent AndWith(string name, string value)
        {
            var content = new StringContent(value);
            return AndWith(name, content);
        }

        /// <summary>
        /// Returns a new instance of <see cref="FluentFormContent"/> with the given <paramref name="content"/> added, using the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static FluentFormContent With(string name, HttpContent content)
        {
            var combinedContent = new List<KeyValuePair<string, HttpContent>>()
            {
                new KeyValuePair<string, HttpContent>(name, content)
            };
            return new FluentFormContent(combinedContent);
        }

        /// <summary>
        /// Returns a new instance of <see cref="FluentFormContent"/> with the given <paramref name="value"/> added as <see cref="StringContent"/>, using the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FluentFormContent With(string name, string value)
        {
            var content = new StringContent(value);
            return With(name, content);
        }
    }
}
