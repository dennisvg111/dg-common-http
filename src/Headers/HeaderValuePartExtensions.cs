using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Http.Headers
{
    internal static class HeaderValuePartExtensions
    {
        public static bool TryGet(this IEnumerable<HeaderValuePart> properties, string name, out string value)
        {
            var property = properties.LastOrDefault(p => p.NameEquals(name));
            if (property == null)
            {
                value = null;
                return false;
            }
            value = property.Value;
            return true;
        }
    }
}
