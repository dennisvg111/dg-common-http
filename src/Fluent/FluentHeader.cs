namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Represents an HTTP header in a request or response.
    /// </summary>
    public class FluentHeader
    {
        private readonly string _name;
        private readonly string _value;

        /// <summary>
        /// The name of this header.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The value of this header.
        /// </summary>
        public string Value => _value;

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> with the given <paramref name="name"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public FluentHeader(string name, string value)
        {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for an <c>Authorization</c> header with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FluentHeader Authorization(string value)
        {
            return new FluentHeader("Authorization", value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for a <c>Content-Length</c> header with the given value.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static FluentHeader ContentLength(int bytes)
        {
            return new FluentHeader("Content-Length", bytes.ToString());
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentHeader"/> for a <c>Content-Range</c> header with the given value.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="bytes"></param>
        /// <param name="totalBytes"></param>
        /// <returns></returns>
        public static FluentHeader ContentRange(int startIndex, int? bytes, int? totalBytes)
        {
            string headerValue = "/" + (totalBytes.HasValue ? totalBytes.Value.ToString() : "*");

            if (bytes.HasValue)
            {
                headerValue = $"bytes {startIndex}-{(startIndex + bytes.Value - 1)}{headerValue}";
            }
            else
            {
                headerValue = "bytes *" + headerValue;
            }
            return new FluentHeader("Content-Range", headerValue);
        }

        /// <summary>
        /// Returns a string representing the current headername and value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{_name}: {_value}";
        }
    }
}
