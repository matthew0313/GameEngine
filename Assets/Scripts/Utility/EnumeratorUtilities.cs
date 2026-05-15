using System;
using System.Collections.Generic;

public static class EnumeratorUtilities
{
    public static T Find<T>(this IEnumerable<T> self, Func<T, bool> predicate)
    {
        foreach(var item in self)
        {
            if (predicate.Invoke(item)) return item;
        }
        return default;
    }
}