using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace dotnetCampus.Cli.Core
{
    internal static class SingleOptimizedListExtensions
    {
        public static object ToAssignableCollection<T>(this IReadOnlyList<T> list, Type assignableType)
        {
            // 可以将值赋值给以下 if 中所写类型的基类。
            if (assignableType.IsAssignableFrom(typeof(string[])))
            {
                return list.ToArray();
            }
            else if (assignableType.IsAssignableFrom(typeof(List<T>)))
            {
                return list.ToList();
            }
            else if (assignableType.IsAssignableFrom(typeof(Collection<T>)))
            {
                return new Collection<T>(list.ToList());
            }
            else if (assignableType.IsAssignableFrom(typeof(IReadOnlyList<T>)))
            {
                return list;
            }
            else if (assignableType.IsAssignableFrom(typeof(IList<T>)))
            {
                return list.ToList();
            }
            else if (assignableType.IsAssignableFrom(typeof(IEnumerable<T>)))
            {
                return list;
            }
            else
            {
                return list;
            }
        }
    }
}
