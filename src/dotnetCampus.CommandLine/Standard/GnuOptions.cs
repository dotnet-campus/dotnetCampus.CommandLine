using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;

namespace dotnetCampus.Cli.Standard
{
    public class GnuOptions : CommandLineOptionParser<GnuOptions>
    {
        [Option(nameof(Version))]
        public bool Version { get; private set; }

        [Option(nameof(Help))]
        public bool Help { get; private set; }

        internal GnuOptions(List<CommandLineVerbMatch<Task<int>>>? matches)
        {
            AddMatch(nameof(Version), v => Version = v);
            AddMatch(nameof(Help), v => Help = v);
            SetResult(() => this);
        }

        internal void Run()
        {
            if (Help)
            {
                PrintHelpText();
            }
            else if (Version)
            {
                PrintVersionText();
            }
        }

        private void PrintHelpText()
        {

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
