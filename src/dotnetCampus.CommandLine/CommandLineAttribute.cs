using System;

namespace dotnetCampus.Cli
{
    /// <summary>
    /// 为命令行参数与类型属性的关联提供特性基类。
    /// </summary>
    public abstract class CommandLineAttribute : Attribute
    {
        /// <summary>
        /// 只允许内部的类型继承自 <see cref="CommandLineAttribute"/>。
        /// </summary>
        internal CommandLineAttribute()
        {
        }
    }
}