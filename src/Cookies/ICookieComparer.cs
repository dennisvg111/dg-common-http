using System.Collections.Generic;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// <para>Sorts cookies as defined by RFC 6265 section 5.4</para>
    /// <para>This represents the order in which they should appear in an HTTP <c>Cookie</c> header</para>
    /// </summary>
    public class ICookieComparer : IComparer<ICookie>
    {
        /// <summary>
        /// Compares two instances of <see cref="ICookie"/> and returns a value indicating which should appear first in a <c>Cookie</c> header.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following list.
        /// <list type="bullet">
        /// <item><description>Less than zero: <paramref name="x"/> should appear before <paramref name="y"/></description></item>
        /// <item><description>Greater than zero: <paramref name="x"/> should appear after <paramref name="y"/></description></item>
        /// </list>
        /// </returns>
        public int Compare(ICookie x, ICookie y)
        {
            int pathComparison = (x.Path?.Length ?? 0).CompareTo(y.Path?.Length ?? 0);
            if (pathComparison != 0)
            {
                // Cookies with longer paths are listed before cookies with shorter paths.
                return -pathComparison;
            }

            // Among cookies that have equal-length path fields, cookies with earlier creation-times are listed before cookies with later creation-times.
            return x.ReceivedDate.CompareTo(y.ReceivedDate);
        }
    }
}
