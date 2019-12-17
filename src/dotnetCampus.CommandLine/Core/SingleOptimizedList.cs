using System.Collections;
using System.Collections.Generic;

namespace dotnetCampus.Cli.Core
{
    /// <summary>
    /// 为 0 个和 1 个值特殊优化性能的列表。
    /// </summary>
    internal class SingleOptimizedList : IReadOnlyList<string>
    {
        /// <summary>
        /// 如果有值储存，则此值不为 null。在此命令行解析的上下文中，通常也不会为空字符串或空白字符串。
        /// </summary>
        private readonly string _firstValue;

        /// <summary>
        /// 当所需储存的值超过 1 个时，将启用此列表。所以此列表要么为 null，要么有多于 1 个的值。
        /// </summary>
        private List<string>? _restValues;

        /// <summary>
        /// 创建带有一个值的 <see cref="SingleOptimizedList"/> 的实例。
        /// </summary>
        /// <param name="firstValue"></param>
        public SingleOptimizedList(string firstValue)
        {
            _firstValue = firstValue;
        }

        /// <summary>
        /// 添加一个值到集合中。
        /// </summary>
        /// <param name="value">要添加的值。</param>
        public void Add(string value)
        {
            if (value is null)
            {
                throw new System.ArgumentNullException(nameof(value));
            }

            if (_restValues == null)
            {
                _restValues = new List<string>(1)
                {
                    value,
                };
            }
            else
            {
                _restValues.Add(value);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return _firstValue;

            if (_restValues != null)
            {
                foreach (var value in _restValues)
                {
                    yield return value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 获取集合中值的个数。
        /// </summary>
        public int Count => (_restValues?.Count ?? 0) + 1;

        /// <summary>
        /// 获取集合中指定索引处的值。
        /// </summary>
        public string this[int index] => index is 0 ? _firstValue : _restValues![index - 1];
    }
}