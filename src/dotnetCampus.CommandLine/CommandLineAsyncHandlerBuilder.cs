using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;

using static dotnetCampus.Cli.Utils.CommandLineHelpers;

namespace dotnetCampus.Cli
{
    /// <summary>
    /// 用于收集命令行的异步谓词处理方法。
    /// </summary>
    public class CommandLineAsyncHandlerBuilder : ICoreCommandLineAsyncHandlerBuilder
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
        internal CommandLineAsyncHandlerBuilder(CommandLine commandLine)
            => _commandLine = commandLine ?? throw new ArgumentNullException(nameof(commandLine));

        /// <summary>
        /// 创建一个 <see cref="CommandLineAsyncHandlerBuilder"/> 的新实例。
        /// </summary>
        /// <param name="commandLine">命令行参数。</param>
        /// <param name="toMatchList">在同步版本的收集中已收集的谓词处理方法。</param>
        internal CommandLineAsyncHandlerBuilder(CommandLine commandLine,
            List<Func<string, MatchHandleResult<Task<int>>>> toMatchList)
        {
            _commandLine = commandLine ?? throw new ArgumentNullException(nameof(commandLine));
            _toMatchList = toMatchList;
        }

        /// <inheritdoc />
        CommandLine ICoreCommandLineAsyncHandlerBuilder.CommandLine => _commandLine;

        /// <inheritdoc />
        List<Func<string, MatchHandleResult<Task<int>>>> ICoreCommandLineAsyncHandlerBuilder.MatchList => _toMatchList;

        /// <inheritdoc />
        void ICoreCommandLineAsyncHandlerBuilder.AddMatch(Func<string, MatchHandleResult<Task<int>>> match) => _toMatchList.Add(match);

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
