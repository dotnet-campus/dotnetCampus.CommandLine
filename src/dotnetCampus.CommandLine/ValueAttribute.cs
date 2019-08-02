using System;

namespace dotnetCampus.Cli
{
    /// <summary>
    /// 标记一个属性对应命令行中的一个不带选项的值。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ValueAttribute : Attribute
    {
        /// <summary>
        /// 获取此值在所有命令行不带选项值中的位置。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 将此属性与命令行中指定位置的一个值进行绑定。
        /// </summary>
        /// <param name="index">命令行中值的位置。</param>
        public ValueAttribute(int index)
        {
            Index = index;
        }
    }
}