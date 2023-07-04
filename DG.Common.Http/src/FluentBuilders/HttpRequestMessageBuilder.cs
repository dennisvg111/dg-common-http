using System.Net.Http;

namespace DG.Common.Http.FluentBuilders
{
    public class HttpRequestMessageBuilder
    {
        public HttpRequestMessage Message
        {
            get
            {
                return new HttpRequestMessage();
            }
        }
    }
}
