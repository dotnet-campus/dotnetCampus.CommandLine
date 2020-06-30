using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;

namespace dotnetCampus.Cli
{
    public interface ICommandLineHandlerBuilder
    {
    }

    public interface ICommandLineAsyncHandlerBuilder : ICommandLineHandlerBuilder
    {
    }

    internal interface ICoreCommandLineHandlerBuilder : ICommandLineHandlerBuilder
    {
        CommandLine CommandLine { get; }

        List<Func<string, MatchHandleResult<Task<int>>>> MatchList { get; }

        void AddMatch(Func<string, MatchHandleResult<Task<int>>> match);
    }

    internal interface ICoreCommandLineAsyncHandlerBuilder : ICommandLineAsyncHandlerBuilder
    {
        CommandLine CommandLine { get; }

        List<Func<string, MatchHandleResult<Task<int>>>> MatchList { get; }

        void AddMatch(Func<string, MatchHandleResult<Task<int>>> match);
    }
}
