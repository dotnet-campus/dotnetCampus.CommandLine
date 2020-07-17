using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;

namespace dotnetCampus.Cli
{
    /// <summary>
    /// 对 <see cref="CommandLine"/> 和 <see cref="CommandLineHandlerBuilder"/> 提供抽象。
    /// </summary>
    public interface ICommandLineHandlerBuilder
    {
    }

    /// <summary>
    /// 对 <see cref="CommandLineAsyncHandlerBuilder"/> 提供抽象。
    /// </summary>
    public interface ICommandLineAsyncHandlerBuilder : ICommandLineHandlerBuilder
    {
    }

    /// <summary>
    /// 为 <see cref="CommandLineHandlerBuilder"/> 提供仅限内部使用的方法。
    /// </summary>
    internal interface ICoreCommandLineHandlerBuilder : ICommandLineHandlerBuilder
    {
        CommandLine CommandLine { get; }

        List<CommandLineVerbMatch<Task<int>>> Matches { get; }

        void AddMatch<TVerb>(Func<string?, MatchHandleResult<Task<int>>> match);
    }

    /// <summary>
    /// 为 <see cref="CommandLineAsyncHandlerBuilder"/> 提供仅限内部使用的方法。
    /// </summary>
    internal interface ICoreCommandLineAsyncHandlerBuilder : ICommandLineAsyncHandlerBuilder
    {
        CommandLine CommandLine { get; }

        List<CommandLineVerbMatch<Task<int>>> Matches { get; }

        void AddMatch<TVerb>(Func<string?, MatchHandleResult<Task<int>>> match);
    }
}
