using System;
using System.Diagnostics.CodeAnalysis;
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
    internal readonly struct MatchHandleResult<T> where T : notnull
    {
        /// <summary>
        /// 记录处理器真实的返回值（或者异步处理器的异步返回值）。
        /// </summary>
        [MaybeNull, AllowNull]
        public readonly T Value;

        /// <summary>
        /// 记录谓词匹配结果。
        /// </summary>
        public readonly VerbMatchingResult MatchingResult;

        /// <summary>
        /// 记录默认处理器忽视谓词判断的执行方法。
        /// </summary>
        public readonly Func<T>? Handler;

        public MatchHandleResult(T value)
        {
            Value = value;
            MatchingResult = VerbMatchingResult.Matched;
            Handler = null;
        }

        public MatchHandleResult(VerbMatchingResult matchingResult)
        {
            Value = default;
            MatchingResult = matchingResult;
            Handler = null;
        }

        public MatchHandleResult(Func<T> handler)
        {
            Value = default;
            MatchingResult = VerbMatchingResult.Default;
            Handler = handler;
        }
    }
}
