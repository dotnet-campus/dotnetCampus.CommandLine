using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;

using static dotnetCampus.Cli.Utils.CommandLineHelpers;

namespace dotnetCampus.Cli
{
    /// <summary>
    /// 用于收集命令行的谓词处理方法。
    /// </summary>
    public class CommandLineHandlerBuilder
    {
        /// <summary>
        /// 收集所对应的命令行。
        /// </summary>
        private readonly CommandLine _commandLine;

        /// <summary>
        /// 收集的谓词处理方法。
        /// </summary>
        private readonly List<Func<string, MatchHandleResult<Task<int>>>> _toMatchList
            = new List<Func<string, MatchHandleResult<Task<int>>>>();

        /// <summary>
        /// 创建一个 <see cref="CommandLineHandlerBuilder"/> 的新实例。
        /// </summary>
        /// <param name="commandLine">命令行参数。</param>
        internal CommandLineHandlerBuilder(CommandLine commandLine) => _commandLine = commandLine;

        /// <summary>
        /// 添加一个谓词处理方法，用于在命令行匹配谓词的时候执行。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始执行。</returns>
        [Pure]
        public CommandLineHandlerBuilder AddHandler<TVerb>(
            Action<TVerb> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            _toMatchList.Add(verb => MatchAndHandle(_commandLine, verb, options => Invoke(handler, options), parser));
            return this;
        }

        /// <summary>
        /// 添加一个带有退出代码返回值的谓词处理方法，用于在命令行匹配谓词的时候执行并返回退出代码。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始执行。</returns>
        [Pure]
        public CommandLineHandlerBuilder AddHandler<TVerb>(
            Func<TVerb, int> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            _toMatchList.Add(verb => MatchAndHandle(_commandLine, verb, options => Invoke(handler, options), parser));
            return this;
        }

        /// <summary>
        /// 添加一个异步的谓词处理方法，用于在命令行匹配谓词的时候异步执行。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public CommandLineAsyncHandlerBuilder AddHandler<TVerb>(
            Func<TVerb, Task> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            var builder = new CommandLineAsyncHandlerBuilder(_commandLine, _toMatchList);
            return builder.AddHandler(handler, parser);
        }

        /// <summary>
        /// 添加一个带有退出代码返回值的异步谓词处理方法，用于在命令行匹配谓词的时候异步执行并返回退出代码。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public CommandLineAsyncHandlerBuilder AddHandler<TVerb>(
            Func<TVerb, Task<int>> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            var builder = new CommandLineAsyncHandlerBuilder(_commandLine, _toMatchList);
            return builder.AddHandler(handler, parser);
        }

        /// <summary>
        /// 开始匹配谓词，如果谓词与任何一个已收集的谓词处理方法匹配，则执行此处理方法，然后返回其处理后的退出代码（没有指定退出代码则返回 0）。
        /// 如果已收集的命令行参数类型中包含不带谓词的参数，那么会成为默认谓词并在没有任何谓词匹配时执行其处理方法。
        /// </summary>
        /// <remarks>
        /// 1. 收集的所有谓词处理方法最多只能有一个不带谓词。
        /// 2. 最多只会有一个谓词处理方法被执行，此方法会返回唯一那个处理方法的退出代码。
        /// </remarks>
        /// <returns>谓词处理方法的退出代码。</returns>
        public int Run()
        {
            var possibleVerb = FindPossibleVerb(_commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(_toMatchList).Run(possibleVerb))
            {
                return exitCode.Result;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return 0;
        }
    }

    /// <summary>
    /// 用于收集命令行的异步谓词处理方法。
    /// </summary>
    public class CommandLineAsyncHandlerBuilder
    {
        /// <summary>
        /// 收集所对应的命令行。
        /// </summary>
        private readonly CommandLine _commandLine;

        /// <summary>
        /// 收集的谓词处理方法。
        /// </summary>
        private readonly List<Func<string, MatchHandleResult<Task<int>>>> _toMatchList
            = new List<Func<string, MatchHandleResult<Task<int>>>>();

        /// <summary>
        /// 创建一个 <see cref="CommandLineAsyncHandlerBuilder"/> 的新实例。
        /// </summary>
        /// <param name="commandLine">命令行参数。</param>
        internal CommandLineAsyncHandlerBuilder(CommandLine commandLine) => _commandLine = commandLine;

        /// <summary>
        /// 创建一个 <see cref="CommandLineAsyncHandlerBuilder"/> 的新实例。
        /// </summary>
        /// <param name="commandLine">命令行参数。</param>
        /// <param name="toMatchList">在同步版本的收集中已收集的谓词处理方法。</param>
        internal CommandLineAsyncHandlerBuilder(CommandLine commandLine,
            List<Func<string, MatchHandleResult<Task<int>>>> toMatchList)
        {
            _commandLine = commandLine;
            _toMatchList = toMatchList;
        }

        /// <summary>
        /// 添加一个谓词处理方法，用于在命令行匹配谓词的时候执行。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public CommandLineAsyncHandlerBuilder AddHandler<TVerb>(
            Action<TVerb> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            _toMatchList.Add(verb => MatchAndHandle(_commandLine, verb, options => Invoke(handler, options), parser));
            return this;
        }

        /// <summary>
        /// 添加一个带有退出代码返回值的谓词处理方法，用于在命令行匹配谓词的时候执行并返回退出代码。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public CommandLineAsyncHandlerBuilder AddHandler<TVerb>(
            Func<TVerb, int> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            _toMatchList.Add(verb => MatchAndHandle(_commandLine, verb, options => Invoke(handler, options), parser));
            return this;
        }

        /// <summary>
        /// 添加一个异步的谓词处理方法，用于在命令行匹配谓词的时候异步执行。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public CommandLineAsyncHandlerBuilder AddHandler<TVerb>(
            Func<TVerb, Task> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            _toMatchList.Add(verb => MatchAndHandle(_commandLine, verb, options => Invoke(handler, options), parser));
            return this;
        }

        /// <summary>
        /// 添加一个带有退出代码返回值的异步谓词处理方法，用于在命令行匹配谓词的时候异步执行并返回退出代码。
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>已收集的谓词处理方法，可以继续收集或者开始异步执行。</returns>
        [Pure]
        public CommandLineAsyncHandlerBuilder AddHandler<TVerb>(
            Func<TVerb, Task<int>> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            _toMatchList.Add(verb => MatchAndHandle(_commandLine, verb, handler, parser));
            return this;
        }

        /// <summary>
        /// 开始匹配谓词，如果谓词与任何一个已收集的谓词处理方法匹配，则执行此处理方法，然后返回其处理后的退出代码（没有指定退出代码则返回 0）。
        /// 如果已收集的命令行参数类型中包含不带谓词的参数，那么会成为默认谓词并在没有任何谓词匹配时执行其处理方法。
        /// 因为收集到的处理方法中包含异步方法，由于不确定会执行哪一个谓词处理方法，所以此方法必须异步等待以取得为此处理方法的退出代码。
        /// </summary>
        /// <remarks>
        /// 1. 收集的所有谓词处理方法最多只能有一个不带谓词。
        /// 2. 最多只会有一个谓词处理方法被执行，此方法会返回唯一那个处理方法的退出代码。
        /// </remarks>
        /// <returns>用于异步等待谓词处理方法退出代码的异步任务。</returns>
        public Task<int> RunAsync()
        {
            var possibleVerb = FindPossibleVerb(_commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(_toMatchList).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }
    }
}