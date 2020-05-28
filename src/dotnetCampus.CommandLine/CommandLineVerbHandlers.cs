using System;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;

using static dotnetCampus.Cli.Utils.CommandLineHelpers;

// ReSharper disable UnusedVariable

namespace dotnetCampus.Cli
{
    /// <summary>
    /// 包含命令行谓词处理相关的扩展方法。
    /// </summary>
    [Obsolete("此类型中所有方法的 API 设计不利于代码编写，所以此类型废弃。")]
    internal static class CommandLineVerbHandlers
    {
        /// <summary>
        /// 根据谓词处理命令行参数，如果谓词匹配，则执行其处理器；如果不匹配则抛出异常。
        /// 参数类型可以不带谓词，这样，传入的命令行参数中也不应该出现谓词。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        public static void Handle<TVerb>(this CommandLine commandLine,
            Action<TVerb> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler, parser)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return;
            }

            ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行处理，如果谓词匹配一个参数类型，那么就会执行其处理器。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的处理器。</param>
        /// <param name="handler1">命令行参数解析后的处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static void Handle<TVerb0, TVerb1>(this CommandLine commandLine,
            Action<TVerb0> handler0, Action<TVerb1> handler1,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return;
            }

            ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行处理，如果谓词匹配一个参数类型，那么就会执行其处理器。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的处理器。</param>
        /// <param name="handler1">命令行参数解析后的处理器。</param>
        /// <param name="handler2">命令行参数解析后的处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static void Handle<TVerb0, TVerb1, TVerb2>(this CommandLine commandLine,
            Action<TVerb0> handler0, Action<TVerb1> handler1, Action<TVerb2> handler2,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return;
            }

            ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行处理，如果谓词匹配一个参数类型，那么就会执行其处理器。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb3">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的处理器。</param>
        /// <param name="handler1">命令行参数解析后的处理器。</param>
        /// <param name="handler2">命令行参数解析后的处理器。</param>
        /// <param name="handler3">命令行参数解析后的处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <param name="parser3">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static void Handle<TVerb0, TVerb1, TVerb2, TVerb3>(this CommandLine commandLine,
            Action<TVerb0> handler0, Action<TVerb1> handler1,
            Action<TVerb2> handler2, Action<TVerb3> handler3,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null, ICommandLineOptionParser<TVerb3>? parser3 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2),
                verb => MatchAndHandle(commandLine, verb, handler3, parser3)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return;
            }

            ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词处理命令行参数，如果谓词匹配，则执行其处理器并获取其退出代码；如果不匹配则抛出异常。
        /// 参数类型可以不带谓词，这样，传入的命令行参数中也不应该出现谓词。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static int Handle<TVerb>(this CommandLine commandLine,
            Func<TVerb, int> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler, parser)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return exitCode;
            }

            return ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行处理，如果谓词匹配一个参数类型，那么就会执行其处理器并获取其退出代码。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的处理器。</param>
        /// <param name="handler1">命令行参数解析后的处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static int Handle<TVerb0, TVerb1>(this CommandLine commandLine,
            Func<TVerb0, int> handler0, Func<TVerb1, int> handler1,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return exitCode;
            }

            return ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行处理，如果谓词匹配一个参数类型，那么就会执行其处理器并获取其退出代码。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的处理器。</param>
        /// <param name="handler1">命令行参数解析后的处理器。</param>
        /// <param name="handler2">命令行参数解析后的处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static int Handle<TVerb0, TVerb1, TVerb2>(this CommandLine commandLine,
            Func<TVerb0, int> handler0, Func<TVerb1, int> handler1, Func<TVerb2, int> handler2,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return exitCode;
            }

            return ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行处理，如果谓词匹配一个参数类型，那么就会执行其处理器并获取其退出代码。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb3">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的处理器。</param>
        /// <param name="handler1">命令行参数解析后的处理器。</param>
        /// <param name="handler2">命令行参数解析后的处理器。</param>
        /// <param name="handler3">命令行参数解析后的处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <param name="parser3">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static int Handle<TVerb0, TVerb1, TVerb2, TVerb3>(this CommandLine commandLine,
            Func<TVerb0, int> handler0, Func<TVerb1, int> handler1,
            Func<TVerb2, int> handler2, Func<TVerb3, int> handler3,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null, ICommandLineOptionParser<TVerb3>? parser3 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<int>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2),
                verb => MatchAndHandle(commandLine, verb, handler3, parser3)).Run(possibleVerb))
            {
                // 只有能够匹配谓词的处理器执行后，才会在这里得到结果。此代码最多执行一次。
                return exitCode;
            }

            return ThrowIfVerbNotMatched(possibleVerb);
        }

        /// <summary>
        /// 根据谓词异步处理命令行参数，如果谓词匹配，则执行其异步处理器；如果不匹配则抛出异常。
        /// 参数类型可以不带谓词，这样，传入的命令行参数中也不应该出现谓词。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的异步处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task HandleAsync<TVerb>(this CommandLine commandLine,
            Func<TVerb, Task> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler, parser)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行异步处理，如果谓词匹配一个参数类型，那么就会执行其异步处理器。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的异步处理器。</param>
        /// <param name="handler1">命令行参数解析后的异步处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task HandleAsync<TVerb0, TVerb1>(this CommandLine commandLine,
            Func<TVerb0, Task> handler0, Func<TVerb1, Task> handler1,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行异步处理，如果谓词匹配一个参数类型，那么就会执行其异步处理器。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的异步处理器。</param>
        /// <param name="handler1">命令行参数解析后的异步处理器。</param>
        /// <param name="handler2">命令行参数解析后的异步处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task HandleAsync<TVerb0, TVerb1, TVerb2>(this CommandLine commandLine,
            Func<TVerb0, Task> handler0, Func<TVerb1, Task> handler1, Func<TVerb2, Task> handler2,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行异步处理，如果谓词匹配一个参数类型，那么就会执行其异步处理器。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb3">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的异步处理器。</param>
        /// <param name="handler1">命令行参数解析后的异步处理器。</param>
        /// <param name="handler2">命令行参数解析后的异步处理器。</param>
        /// <param name="handler3">命令行参数解析后的异步处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <param name="parser3">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task HandleAsync<TVerb0, TVerb1, TVerb2, TVerb3>(this CommandLine commandLine,
            Func<TVerb0, Task> handler0, Func<TVerb1, Task> handler1,
            Func<TVerb2, Task> handler2, Func<TVerb3, Task> handler3,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null, ICommandLineOptionParser<TVerb3>? parser3 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2),
                verb => MatchAndHandle(commandLine, verb, handler3, parser3)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 根据谓词异步处理命令行参数，如果谓词匹配，则执行其处理器并获取其退出代码；如果不匹配则抛出异常。
        /// 参数类型可以不带谓词，这样，传入的命令行参数中也不应该出现谓词。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler">命令行参数解析后的异步处理器。</param>
        /// <param name="parser">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task<int> HandleAsync<TVerb>(this CommandLine commandLine,
            Func<TVerb, Task<int>> handler, ICommandLineOptionParser<TVerb>? parser = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler, parser)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行异步处理，如果谓词匹配一个参数类型，那么就会执行其处理器并获取其退出代码。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的异步处理器。</param>
        /// <param name="handler1">命令行参数解析后的异步处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task<int> HandleAsync<TVerb0, TVerb1>(this CommandLine commandLine,
            Func<TVerb0, Task<int>> handler0, Func<TVerb1, Task<int>> handler1,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行异步处理，如果谓词匹配一个参数类型，那么就会执行其处理器并获取其退出代码。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的异步处理器。</param>
        /// <param name="handler1">命令行参数解析后的异步处理器。</param>
        /// <param name="handler2">命令行参数解析后的异步处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task<int> HandleAsync<TVerb0, TVerb1, TVerb2>(this CommandLine commandLine,
            Func<TVerb0, Task<int>> handler0, Func<TVerb1, Task<int>> handler1, Func<TVerb2, Task<int>> handler2,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 根据谓词选择命令行参数进行异步处理，如果谓词匹配一个参数类型，那么就会执行其处理器并获取其退出代码。
        /// 这些命令行参数类型最多只能有一个类型不带谓词（对应命令行中没有谓词的情况）。
        /// <para>
        /// 建议传入命令行参数的 <see cref="ICommandLineOptionParser{T}"/>，否则会创建执行效率更低的运行时动态解析器解析。
        /// 不过放心，即便不传入静态解析器，其性能也比主流的命令行解析器性能更好。
        /// </para>
        /// </summary>
        /// <typeparam name="TVerb0">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb1">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb2">命令行参数类型。</typeparam>
        /// <typeparam name="TVerb3">命令行参数类型。</typeparam>
        /// <param name="commandLine">命令行解析后的实例。</param>
        /// <param name="handler0">命令行参数解析后的异步处理器。</param>
        /// <param name="handler1">命令行参数解析后的异步处理器。</param>
        /// <param name="handler2">命令行参数解析后的异步处理器。</param>
        /// <param name="handler3">命令行参数解析后的异步处理器。</param>
        /// <param name="parser0">命令行解析器。</param>
        /// <param name="parser1">命令行解析器。</param>
        /// <param name="parser2">命令行解析器。</param>
        /// <param name="parser3">命令行解析器。</param>
        /// <returns>单个命令行处理器的推出代码。</returns>
        public static Task<int> HandleAsync<TVerb0, TVerb1, TVerb2, TVerb3>(this CommandLine commandLine,
            Func<TVerb0, Task<int>> handler0, Func<TVerb1, Task<int>> handler1,
            Func<TVerb2, Task<int>> handler2, Func<TVerb3, Task<int>> handler3,
            ICommandLineOptionParser<TVerb0>? parser0 = null, ICommandLineOptionParser<TVerb1>? parser1 = null,
            ICommandLineOptionParser<TVerb2>? parser2 = null, ICommandLineOptionParser<TVerb3>? parser3 = null)
        {
            var possibleVerb = FindPossibleVerb(commandLine);
            foreach (var exitCode in new HandleVerbStateMachine<Task<int>>(
                verb => MatchAndHandle(commandLine, verb, handler0, parser0),
                verb => MatchAndHandle(commandLine, verb, handler1, parser1),
                verb => MatchAndHandle(commandLine, verb, handler2, parser2),
                verb => MatchAndHandle(commandLine, verb, handler3, parser3)).Run(possibleVerb))
            {
                return exitCode;
            }

            ThrowIfVerbNotMatchedAsync(possibleVerb);
            return Task.FromResult(0);
        }
    }
}