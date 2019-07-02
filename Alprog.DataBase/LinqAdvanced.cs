using System;
using System.Collections.Generic;
using System.Text;

namespace Alprog.DataBase
{
    public static class LinqAdvanced
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
        public static bool Contains<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var item in enumerable)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }


    }
}
