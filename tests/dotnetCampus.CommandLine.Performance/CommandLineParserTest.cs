using BenchmarkDotNet.Attributes;
using CommandLine;
using dotnetCampus.Cli.Performance.Fakes;
using dotnetCampus.Cli.Tests.Fakes;
using static dotnetCampus.Cli.Tests.Fakes.CommandLineArgs;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace dotnetCampus.Cli.Performance
{
    public class CommandLineParserTest
    {
        [Benchmark]
        public void ParseNoArgs()
        {
            var commandLine = CommandLine.Parse(NoArgs, protocolName: "walterlv");
            commandLine.As(new OptionsParser());
        }

        [Benchmark]
        public void ParseNoArgsAuto()
        {
            var commandLine = CommandLine.Parse(NoArgs, protocolName: "walterlv");
            commandLine.As<Options>();
        }

        [Benchmark(Baseline = true)]
        public void ParseWindows()
        {
            var commandLine = CommandLine.Parse(WindowsStyleArgs, protocolName: "walterlv");
            commandLine.As(new OptionsParser());
        }

        [Benchmark]
        public void ParseWindowsAuto()
        {
            var commandLine = CommandLine.Parse(WindowsStyleArgs, protocolName: "walterlv");
            commandLine.As<Options>();
        }

        [Benchmark]
        public void ParseWindowsRuntime()
        {
            var commandLine = CommandLine.Parse(WindowsStyleArgs, protocolName: "walterlv");
            commandLine.As<RuntimeOptions>();
        }

        [Benchmark]
        public void ParseWindowsImmutableRuntime()
        {
            var commandLine = CommandLine.Parse(WindowsStyleArgs, protocolName: "walterlv");
            commandLine.As<RuntimeImmutableOptions>();
        }

        [Benchmark]
        public void HandleVerbs()
        {
            var commandLine = CommandLine.Parse(EditVerbArgs);
            commandLine.AddHandler(options => 0, new SelfWrittenEditOptionsParser())
                .AddHandler(options => 0, new SelfWrittenPrintOptionsParser()).Run();
        }

        [Benchmark]
        public void HandleVerbsRuntime()
        {
            var commandLine = CommandLine.Parse(EditVerbArgs);
            commandLine.AddHandler<EditOptions>(options => 0)
                .AddHandler<PrintOptions>(options => 0).Run();
        }

        [Benchmark]
        public void ParseCmd()
        {
            var commandLine = CommandLine.Parse(CmdStyleArgs, protocolName: "walterlv");
            commandLine.As(new OptionsParser());
        }

        [Benchmark]
        public void ParseCmdAuto()
        {
            var commandLine = CommandLine.Parse(CmdStyleArgs, protocolName: "walterlv");
            commandLine.As<Options>();
        }

        [Benchmark]
        public void ParseLinux()
        {
            var commandLine = CommandLine.Parse(LinuxStyleArgs, protocolName: "walterlv");
            commandLine.As(new OptionsParser());
        }

        [Benchmark]
        public void ParseLinuxAuto()
        {
            var commandLine = CommandLine.Parse(LinuxStyleArgs, protocolName: "walterlv");
            commandLine.As<Options>();
        }

        [Benchmark]
        public void ParseUrl()
        {
            var commandLine = CommandLine.Parse(UrlArgs, protocolName: "walterlv");
            commandLine.As(new OptionsParser());
        }

        [Benchmark]
        public void ParseUrlAuto()
        {
            var commandLine = CommandLine.Parse(UrlArgs, protocolName: "walterlv");
            commandLine.As<Options>();
        }

        [Benchmark]
        public void CommandLineParser()
        {
            Parser.Default.ParseArguments<ComparedOptions>(LinuxStyleArgs).WithParsed(options => { });
        }
    }
}
