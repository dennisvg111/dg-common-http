using System;
using System.Linq;

namespace DG.Common.Http.Headers
{
    internal class HeaderValuePart
    {
        private readonly string _name;
        private readonly string _value;

        public string Name => _name;
        public string Value => _value;

        public HeaderValuePart(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public bool NameEquals(string name)
        {
            return _name != null && _name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        public static HeaderValuePart Parse(string header)
        {
            int index = header.IndexOf('=');
            if (index <= 0)
            {
                return new HeaderValuePart(header.Substring(index < 0 ? 0 : 1).Trim(), string.Empty);
            }
            if (index == header.Length - 1)
            {
                return new HeaderValuePart(header.Substring(0, header.Length - 1).Trim(), string.Empty);
            }
            return new HeaderValuePart(header.Substring(0, index).Trim(), header.Substring(index + 1).Trim());
        }

        public static HeaderValuePart[] ParseList(string header)
        {
            return header.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => Parse(p))
                .ToArray();
        }

        public override string ToString()
        {
            return $"{_name}={_value}";
        }
    }
}
