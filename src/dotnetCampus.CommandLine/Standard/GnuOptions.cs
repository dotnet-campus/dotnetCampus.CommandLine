using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;

namespace dotnetCampus.Cli.Standard
{
    internal class GnuOptions : CommandLineOptionParser<GnuOptions>
    {
        private readonly List<CommandLineVerbMatch<Task<int>>>? _matches;

        [Option(nameof(Version))]
        public bool Version { get; private set; }

        [Option(nameof(Help))]
        public bool Help { get; private set; }

        internal GnuOptions(List<CommandLineVerbMatch<Task<int>>>? matches)
        {
            _matches = matches;

            AddMatch(nameof(Version), v => Version = v);
            AddMatch(nameof(Help), v => Help = v);
            SetResult(() => this);
        }

        internal void Run()
        {
            if (Help)
            {
                PrintDetailHelpText();
            }
            else if (Version)
            {
                PrintVersionText();
            }
            else
            {
                PrintHelpText();
            }
        }

        private void PrintHelpText()
        {
            Console.WriteLine("用法（占位符）");
        }

        private void PrintDetailHelpText()
        {
            Console.WriteLine("详细用法（占位符）");
        }

        private static void PrintVersionText()
        {
            var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (string.IsNullOrWhiteSpace(version))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("此程序没有指定版本号，建议开发者在开发时通过项目属性指定版本号，或者设置程序集的 AssemblyInformationalVersionAttribute 特性。");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(version);
            }
        }
    }
}
