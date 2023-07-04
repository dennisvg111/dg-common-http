using System.Net.Http;
using System.Threading.Tasks;

namespace DG.Common.Http.FluentBuilders
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Send the HTTP request created by the given <see cref="HttpRequestMessageBuilder"/> as an asynchronous operation.
        /// </summary>
        /// <param name="client">The client used to send a <see cref="HttpRequestMessage"/>.</param>
        /// <param name="messageBuilder">The builder used to create a <see cref="HttpRequestMessage"/>.</param>
        /// <returns>Returns a <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<HttpResponseMessage> SendMessage(this HttpClient client, HttpRequestMessageBuilder messageBuilder)
        {
            var request = messageBuilder.Message;

            var response = await client.SendAsync(request);

            return response;
        }
    }
}
