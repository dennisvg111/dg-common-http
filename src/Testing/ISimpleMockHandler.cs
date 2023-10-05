namespace DG.Common.Http.Testing
{
    /// <summary>
    /// <inheritdoc cref="IMockHandler"/>
    /// </summary>
    public interface ISimpleMockHandler
    {
        /// <summary>
        /// <inheritdoc cref="IMockHandler.GetResponse(System.Net.Http.HttpRequestMessage)"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SimpleMockResponse GetResponse(SimpleMockRequest request);
    }
}
