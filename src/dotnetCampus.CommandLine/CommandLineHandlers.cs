using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace dotnetCampus.Cli
{
    /// <summary>
    /// 包含命令行谓词处理相关的扩展方法。
    /// </summary>
    public static class CommandLineHandlers
    {
        /// <summary>
        /// 添加一个谓词处理方法，用于在命令行匹配谓词的时候执行。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始执行。</returns>
        [Pure]
        public static CommandLineHandlerBuilder AddHandler<TVerb>(this CommandLine commandLine,
            Action<TVerb> handler, ICommandLineOptionParser<TVerb>? parser = null) =>
            new CommandLineHandlerBuilder(commandLine).AddHandler(handler, parser);

        /// <summary>
        /// 添加一个带有退出代码返回值的谓词处理方法，用于在命令行匹配谓词的时候执行并返回退出代码。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始执行。</returns>
        [Pure]
        public static CommandLineHandlerBuilder AddHandler<TVerb>(this CommandLine commandLine,
            Func<TVerb, int> handler, ICommandLineOptionParser<TVerb>? parser = null)
            => new CommandLineHandlerBuilder(commandLine).AddHandler(handler, parser);

        /// <summary>
        /// 添加一个异步的谓词处理方法，用于在命令行匹配谓词的时候异步执行。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public static CommandLineAsyncHandlerBuilder AddHandler<TVerb>(this CommandLine commandLine,
            Func<TVerb, Task> handler, ICommandLineOptionParser<TVerb>? parser = null) =>
            new CommandLineAsyncHandlerBuilder(commandLine).AddHandler(handler, parser);

        /// <summary>
        /// 添加一个带有退出代码返回值的异步谓词处理方法，用于在命令行匹配谓词的时候异步执行并返回退出代码。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public static CommandLineAsyncHandlerBuilder AddHandler<TVerb>(this CommandLine commandLine,
            Func<TVerb, Task<int>> handler, ICommandLineOptionParser<TVerb>? parser = null)
            => new CommandLineAsyncHandlerBuilder(commandLine).AddHandler(handler, parser);
    }
}
