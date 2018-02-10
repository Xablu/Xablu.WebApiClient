using System;
using System.Collections.Generic;

namespace Xablu.WebApiClient.Abstractions
{
    public static class InitializerExtensions
    {
        public static void Add<T1, T2>(this ICollection<KeyValuePair<T1, T2>> target, T1 item1, T2 item2)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Add(new KeyValuePair<T1, T2>(item1, item2));
        }
    }
}
