using System;
using System.Runtime.InteropServices;

namespace dotnetCampus.Cli.StateMachine
{
    /// <summary>
    /// 为 CommandLineHandlers.MatchAndHandle 方法提供返回值。用于记录三种不同的可能返回值。
    /// </summary>
    /// <typeparam name="T">
    /// 对于同步处理器，使用处理器返回值 int；
    /// 对于异步处理器，使用处理器返回值 Task&lt;int&gt;。
    /// </typeparam>
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct CommandLineVerbMatch<T> where T : notnull
    {
        /// <summary>
        /// 记录定义此谓词的类型。
        /// </summary>
        public Type VerbType { get; }

        /// <summary>
        /// 执行此谓词的处理器函数，并返回匹配和执行结果。
        /// </summary>
        public readonly Func<string?, MatchHandleResult<T>> Handler { get; }

        public CommandLineVerbMatch(Type verbType, Func<string?, MatchHandleResult<T>> handler)
        {
            VerbType = verbType ?? throw new ArgumentNullException(nameof(verbType));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
    }
}
