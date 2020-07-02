using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace dotnetCampus.Cli.Utils
{
    internal static class CommandLineAssigningExtensions
    {
        /// <summary>
        /// 将命令行中解析出来的字符串集合解析成 <paramref name="assignableType"/> 类型属性能接收的值。
        /// </summary>
        /// <param name="list">命令行参数集合。</param>
        /// <param name="assignableType">属性类型。</param>
        /// <returns>此类型属性能接收的值。</returns>
        public static object ToAssignableValue(this IReadOnlyList<string> list, Type assignableType)
        {
            // 可以将值赋值给以下 if 中所写类型的基类。
            if (assignableType.IsAssignableFrom(typeof(Dictionary<string, string>)))
            {
                return ParseArgsToDictionary(list);
            }
            else if (assignableType.IsAssignableFrom(typeof(IReadOnlyDictionary<string, string>)))
            {
                return ParseArgsToDictionary(list);
            }
            else if (assignableType.IsAssignableFrom(typeof(IDictionary<string, string>)))
            {
                return ParseArgsToDictionary(list);
            }
            else if (assignableType.IsAssignableFrom(typeof(KeyValuePair<string, string>)))
            {
                return ParseArgsToDictionary(list);
            }
            else if (assignableType.IsAssignableFrom(typeof(string[])))
            {
                return list.ToArray();
            }
            else if (assignableType.IsAssignableFrom(typeof(List<string>)))
            {
                return list.ToList();
            }
            else if (assignableType.IsAssignableFrom(typeof(Collection<string>)))
            {
                return new Collection<string>(list.ToList());
            }
            else if (assignableType.IsAssignableFrom(typeof(IReadOnlyList<string>)))
            {
                return list;
            }
            else if (assignableType.IsAssignableFrom(typeof(IList<string>)))
            {
                return list.ToList();
            }
            else if (assignableType.IsAssignableFrom(typeof(IEnumerable<string>)))
            {
                return list;
            }
            else
            {
                return list;
            }
        }

        private static Dictionary<string, string> ParseArgsToDictionary(IReadOnlyList<string> list)
        {
            var text = list.Count == 1 ? list[0] : string.Join(" ", list);
            return text.Split(';')
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0].Trim(), x => x[1].Trim(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
