using System;

namespace DG.Common.Http.Cookies
{
    public class SerializedCookie : ICookieIngredients
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Value { get; set; }

        /// <inheritdoc/>
        public Uri OriginUri { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset ReceivedDate { get; set; }

        /// <inheritdoc/>
        public string Domain { get; set; }

        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public bool IsSecure { get; set; }

        /// <inheritdoc/>
        public bool HttpOnly { get; set; }

        /// <inheritdoc/>
        public SameSitePolicy SameSitePolicy { get; set; } = SameSitePolicy.Lax;

        /// <inheritdoc/>
        public DateTimeOffset? Expires { get; set; }

        /// <inheritdoc/>
        public int? MaxAge { get; set; }
    }
}
