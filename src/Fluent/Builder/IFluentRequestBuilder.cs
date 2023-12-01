using System;

namespace DG.Common.Http.Fluent.Builder
{
    /// <summary>
    /// Defines methods to build a new <see cref="FluentRequest"/> with a given url.
    /// </summary>
    public interface IFluentRequestBuilder
    {
        /// <summary>
        /// Specifies the url to execute this method on.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        FluentRequest To(string url);

        /// <summary>
        /// Specifies the url to execute this method on.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        FluentRequest To(Uri url);

        /// <summary>
        /// Specifies the url (using <see cref="UriBuilder.Uri"/>) to execute this method on.
        /// </summary>
        /// <param name="urlBuilder"></param>
        /// <returns></returns>
        FluentRequest To(UriBuilder urlBuilder);
    }
}