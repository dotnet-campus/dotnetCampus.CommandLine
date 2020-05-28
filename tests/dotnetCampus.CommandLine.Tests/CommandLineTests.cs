using System.Linq;
using System.Threading.Tasks;

using dotnetCampus.Cli.Tests.Fakes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

using static dotnetCampus.Cli.Tests.Fakes.CommandLineArgs;

namespace dotnetCampus.Cli.Tests
{
    [TestClass]
    public class CommandLineTests
    {
        [ContractTestCase]
        public void ParseAs()
        {
            "命令行中没有参数，正确完成解析。".Test(() =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(NoArgs);
                var options = commandLine.As(new OptionsParser());

                // Assert
                Assert.AreEqual(null, options.FilePath);
                Assert.AreEqual(false, options.IsFromCloud);
                Assert.AreEqual(false, options.IsIwb);
                Assert.AreEqual(null, options.StartupMode);
                Assert.AreEqual(false, options.IsSilence);
                Assert.AreEqual(null, options.Placement);
                Assert.AreEqual(null, options.StartupSession);
            });

            "使用 {0} 风格的命令行，正确完成解析。".Test((string name, string[] args) =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(args, protocolName: UrlProtocol);
                var options = commandLine.As(new OptionsParser());

                // Assert
                Assert.AreEqual(FileValue, options.FilePath);
                Assert.AreEqual(CloudValue, options.IsFromCloud);
                Assert.AreEqual(IwbValue, options.IsIwb);
                Assert.AreEqual(ModeValue, options.StartupMode);
                Assert.AreEqual(SilenceValue, options.IsSilence);
                Assert.AreEqual(PlacementValue, options.Placement);
                Assert.AreEqual(StartupSessionValue, options.StartupSession);
            }).WithArguments(
                ("Windows", WindowsStyleArgs),
                ("Cmd", CmdStyleArgs),
                ("Linux", LinuxStyleArgs),
                ("Url", UrlArgs));

            "使用运行时解析器解析至可变类型，正确完成解析。".Test(() =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(WindowsStyleArgs);
                var options = commandLine.As<RuntimeOptions>();

                // Assert
                Assert.AreEqual(FileValue, options.FilePath);
                Assert.AreEqual(CloudValue, options.IsFromCloud);
                Assert.AreEqual(IwbValue, options.IsIwb);
                Assert.AreEqual(ModeValue, options.StartupMode);
                Assert.AreEqual(SilenceValue, options.IsSilence);
                Assert.AreEqual(PlacementValue, options.Placement);
                Assert.AreEqual(StartupSessionValue, options.StartupSession);
            });

            "使用运行时解析器解析至不可变类型，正确完成解析。".Test(() =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(WindowsStyleArgs);
                var options = commandLine.As<RuntimeImmutableOptions>();

                // Assert
                Assert.AreEqual(FileValue, options.FilePath);
                Assert.AreEqual(CloudValue, options.IsFromCloud);
                Assert.AreEqual(IwbValue, options.IsIwb);
                Assert.AreEqual(ModeValue, options.StartupMode);
                Assert.AreEqual(SilenceValue, options.IsSilence);
                Assert.AreEqual(PlacementValue, options.Placement);
                Assert.AreEqual(StartupSessionValue, options.StartupSession);
            });
        }

        [ContractTestCase]
        public void ParseAsAmbiguously()
        {
            "命令行传入开关参数，或者传入带有 true/false 值的参数，可以赋值给 bool 类型。".Test((string[] args) =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(args);
                var options = commandLine.As(new AmbiguousOptionsParser());

                // Assert
                Assert.AreEqual(true, options.Boolean);
            }).WithArguments(
                new[] { "--boolean" },
                new[] { "--boolean", "true" }
                );

            "命令行传入带有 true/false 值的参数，可以赋值给 string 类型。".Test(() =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(new[] { "--string-boolean", "true" });
                var options = commandLine.As(new AmbiguousOptionsParser());

                // Assert
                Assert.AreEqual("true", options.StringBoolean);
            });

            "命令行传入带有多个值的参数，可以赋值给 string 类型。".Test(() =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(new[] { "--string-array", "a", "b" });
                var options = commandLine.As(new AmbiguousOptionsParser());

                // Assert
                Assert.AreEqual("a b", options.StringArray);
            });

            "命令行传入带有多个值的参数，可以赋值给 string 集合类型。".Test(() =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(new[]
                {
                    "--array", "a", "b",
                    "--list", "a", "b",
                    "--read-only-list", "a", "b",
                    "--enumerable", "a", "b",
                });
                var options = commandLine.As<CollectionOptions>();

                // Assert
                CollectionAssert.AreEqual(new[] { "a", "b" }, options.Array);
                CollectionAssert.AreEqual(new[] { "a", "b" }, options.List.ToArray());
                CollectionAssert.AreEqual(new[] { "a", "b" }, options.ReadOnlyList.ToArray());
                CollectionAssert.AreEqual(new[] { "a", "b" }, options.Enumerable.ToArray());
            });

            "命令行传入带有多个值的参数，可以赋值给未内置的 string 集合类型。".Test(() =>
            {
                // Arrange & Action
                var commandLine = CommandLine.Parse(new[]
                {
                    "--collection", "a", "b",
                });
                var options = commandLine.As<CollectionOptions>();

                // Assert
                CollectionAssert.AreEqual(new[] { "a", "b" }, options.Collection.ToArray());
            });
        }

        [ContractTestCase]
        public void Handle()
        {
            const string expectedFilePath = @"C:\Users\lvyi\Test.txt";

            "处理带有谓词的命令行参数，可以正确根据谓词选择处理函数。".Test((string[] args, int expectedExitCode) =>
            {
                // Arrange
                var commandLine = CommandLine.Parse(args);

                // Action
                var exitCode = commandLine.Handle<EditOptions, PrintOptions>(
                    options => 0,
                    options => 1);

                // Assert
                Assert.AreEqual(expectedExitCode, exitCode);
            }).WithArguments(
                // 不区分大小写。
                (new[] { "Edit", expectedFilePath }, 0),
                (new[] { "edit", expectedFilePath }, 0),
                (new[] { "Print", expectedFilePath }, 1));

            "处理带有谓词的命令行参数，可以正确解析出含谓词的命令行参数。".Test((string[] args) =>
            {
                // Arrange
                var commandLine = CommandLine.Parse(args);

                // Action
                string filePath = null;
                commandLine.Handle<EditOptions, PrintOptions, ShareOptions>(
                    options => filePath = options.FilePath,
                    options => filePath = options.FilePath,
                    options => { });

                // Assert
                Assert.AreEqual(expectedFilePath, filePath);
            }).WithArguments(
                // 不区分大小写。
                new[] { "Edit", expectedFilePath },
                new[] { "Print", expectedFilePath });

            "处理带有默认谓词的命令行参数，可以在没有谓词的情况下解析。".Test((string[] args) =>
            {
                // Arrange
                var commandLine = CommandLine.Parse(args);

                // Action
                string filePath = null;
                commandLine.Handle<Options, PrintOptions>(
                    options => filePath = options.FilePath,
                    options => filePath = options.FilePath);

                // Assert
                Assert.AreEqual(expectedFilePath, filePath);
            }).WithArguments(
                // 不区分大小写。
                new[] { expectedFilePath },
                new[] { "Print", expectedFilePath });
        }

        [ContractTestCase]
        public void HandleAsync()
        {
            const string expectedFilePath = @"C:\Users\lvyi\Test.txt";

            "处理带有谓词的命令行参数，可以正确根据谓词选择处理函数。".Test(async (string[] args, int expectedExitCode) =>
            {
                // Arrange
                var commandLine = CommandLine.Parse(args);

                // Action
                var exitCode = await commandLine.HandleAsync<EditOptions, PrintOptions>(
                    async options =>
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                        return 1;
                    },
                    async options =>
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                        return 2;
                    });

                // Assert
                Assert.AreEqual(expectedExitCode, exitCode);
            }).WithArguments(
                // 不区分大小写。
                (new[] { "Edit", expectedFilePath }, 1),
                (new[] { "edit", expectedFilePath }, 1),
                (new[] { "Print", expectedFilePath }, 2));

            "处理带有谓词的命令行参数，可以正确解析出含谓词的命令行参数。".Test(async (string[] args) =>
            {
                // Arrange
                var commandLine = CommandLine.Parse(args);

                // Action
                string filePath = null;
                await commandLine.HandleAsync<EditOptions, PrintOptions, ShareOptions>(
                    async options =>
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                        filePath = options.FilePath;
                    },
                    async options =>
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                        filePath = options.FilePath;
                    },
#pragma warning disable 1998
                    async options => { });
#pragma warning restore 1998

                // Assert
                Assert.AreEqual(expectedFilePath, filePath);
            }).WithArguments(
                // 不区分大小写。
                new[] { "Edit", expectedFilePath },
                new[] { "Print", expectedFilePath });

            "处理带有默认谓词的命令行参数，可以在没有谓词的情况下解析。".Test(async (string[] args) =>
            {
                // Arrange
                var commandLine = CommandLine.Parse(args);

                // Action
                string filePath = null;
                await commandLine.HandleAsync<Options, PrintOptions>(
                    async options =>
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                        filePath = options.FilePath;
                    },
                    async options =>
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                        filePath = options.FilePath;
                    });

                // Assert
                Assert.AreEqual(expectedFilePath, filePath);
            }).WithArguments(
                // 不区分大小写。
                new[] { expectedFilePath },
                new[] { "Print", expectedFilePath });
        }

        [ContractTestCase]
        public void AddHandlerAndRun()
        {
            "添加一组参数处理器，然后执行最匹配的处理器。".Test(() =>
            {
            });
        }
    }
}
