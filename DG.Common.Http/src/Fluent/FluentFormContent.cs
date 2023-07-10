using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    public class FluentFormContent
    {
        private readonly List<KeyValuePair<string, HttpContent>> _content;

        public MultipartFormDataContent Content
        {
            get
            {
                var formContent = new MultipartFormDataContent();
                _content.ForEach(kv => formContent.Add(kv.Value, kv.Key));
                return formContent;
            }
        }

        public FluentFormContent(List<KeyValuePair<string, HttpContent>> content)
        {
            _content = content;
        }

        private FluentFormContent AddContent(string name, HttpContent data)
        {
            var content = _content.ToList();
            content.Add(new KeyValuePair<string, HttpContent>(name, data));
            return new FluentFormContent(content);
        }

        public FluentFormContent AndWith(string name, string value)
        {
            return AddContent(name, new StringContent(value));
        }

        public static FluentFormContent With(string name, string value)
        {
            return new FluentFormContent(new List<KeyValuePair<string, HttpContent>>() { new KeyValuePair<string, HttpContent>(name, new StringContent(value)) });
        }
    }
}
