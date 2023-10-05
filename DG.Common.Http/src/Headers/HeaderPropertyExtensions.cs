using System.Linq;

namespace DG.Common.Http.Headers
{
    public static class HeaderPropertyExtensions
    {
        public static bool TryGet(this HeaderProperty[] properties, string name, out string value)
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
