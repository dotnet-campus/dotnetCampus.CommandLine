//using System.Diagnostics.Contracts;

//namespace dotnetCampus.Cli.Standard
//{
//    /// <summary>
//    /// 为命令行提供 GNU 标准支持。
//    /// </summary>
//    public static class GnuExtensions
//    {
//        /// <summary>
//        /// 添加支持 GNU 标准的命令行通用参数。
//        /// </summary>
//        /// <param name="commandLine">构造器模式。</param>
//        /// <returns>构造器模式。</returns>
//        [Pure]
//        public static CommandLineHandlerBuilder AddGnuStandardHandlers(
//            this ICommandLineHandlerBuilder commandLine)
//        {
//            return commandLine.AddHandler(o => o.Run(), new GnuOptions());
//        }

//        /// <summary>
//        /// 添加支持 GNU 标准的命令行通用参数。
//        /// </summary>
//        /// <param name="commandLine">构造器模式。</param>
//        /// <returns>构造器模式。</returns>
//        [Pure]
//        public static CommandLineAsyncHandlerBuilder AddGnuStandardHandlers(
//            this ICommandLineAsyncHandlerBuilder commandLine)
//        {
//            return commandLine.AddHandler(o => o.Run(), new GnuOptions());
//        }
//    }
//}
