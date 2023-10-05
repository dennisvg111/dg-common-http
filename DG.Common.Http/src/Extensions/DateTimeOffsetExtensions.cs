using System;
using System.Globalization;

namespace DG.Common.Http.Extensions
{
    /// <summary>
    /// This class provides extension methods for <see cref="DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        private const string _format = "ddd, dd MMM yyyy HH:mm:ss";
        private static readonly IFormatProvider _invariantCulture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Converts the value of the current <see cref="DateTimeOffset"/> in the correct format to be used in a <c>Set-Cookie</c> header.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToCookieExpiresString(this DateTimeOffset date)
        {
            date = date.ToUniversalTime();
            return date.ToString(_format, _invariantCulture) + " GMT";
        }
    }
}
