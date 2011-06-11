using System.Collections.Generic;

namespace Torshify.Client.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> list, T item)
        {
            int i = 0;
            foreach (var listItem in list)
            {
                if (listItem.Equals(item))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }
    }
}