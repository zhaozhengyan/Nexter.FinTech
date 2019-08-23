using System.Collections.Generic;
using System.Linq;

namespace FinTech.Infrastructure
{
    public static class CollectionExtensions
    {
        public static bool IsAny<T>(this IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }
        public static bool NotAny<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }
}