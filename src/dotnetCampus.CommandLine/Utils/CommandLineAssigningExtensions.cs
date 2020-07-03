using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace dotnetCampus.Cli.Utils
{
    internal static class CommandLineAssigningExtensions
    {
        /// <summary>
        /// 将命令行中解析出来的字符串集合解析成 <paramref name="assignableType"/> 类型属性能接收的值。
        /// </summary>
        /// <param name="values">命令行参数集合。</param>
        /// <param name="assignableType">属性类型。</param>
        /// <returns>此类型属性能接收的值。</returns>
        public static object ToAssignableValue(this IReadOnlyList<string> values, Type assignableType)
        {
            // 使用方写了个属性，assignableType 类型的，我们要将子类赋值给它。
            // 而我们要给它的类型，就是下面 return 的实例的类型。

            if (assignableType.IsAssignableFrom(typeof(string)))
            {
                return MergeList(values);
            }
            else if (assignableType.IsAssignableFrom(typeof(int))
                && int.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out var @int))
            {
                return @int;
            }
            else if (assignableType.IsAssignableFrom(typeof(double))
                && double.TryParse(MergeList(values), NumberStyles.Float, CultureInfo.InvariantCulture, out var @double))
            {
                return @double;
            }
            else if (assignableType.IsAssignableFrom(typeof(string[])))
            {
                return values.ToArray();
            }
            else if (assignableType.IsAssignableFrom(typeof(List<string>)))
            {
                return values.ToList();
            }
            else if (assignableType.IsAssignableFrom(typeof(Collection<string>)))
            {
                return new Collection<string>(values.ToList());
            }
            else if(assignableType.IsAssignableFrom(typeof(Dictionary<string, string>)))
            {
                return ParseArgsToDictionary(values);
            }
            else if (assignableType.IsAssignableFrom(typeof(KeyValuePair<string, string>)))
            {
                return ParseArgsToDictionary(values).FirstOrDefault();
            }
            else
            {
                throw new NotSupportedException($@"The property type {assignableType.FullName} is not supported.
Supported types are: bool, string?, string[]?, List<string>? and all it's interfaces, Collection<string>? and all it's interfaces, Dictionary<string, string>? and all it's interfaces.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<string, string> ParseArgsToDictionary(IReadOnlyList<string> list) => MergeList(list)
            .Split(';')
            .Select(x => x.Split('='))
            .Where(x => x.Length == 2)
            .ToDictionary(x => x[0].Trim(), x => x[1].Trim(), StringComparer.OrdinalIgnoreCase);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string MergeList(IReadOnlyList<string> list) => list.Count == 1 ? list[0] : string.Join(" ", list);
    }
}
