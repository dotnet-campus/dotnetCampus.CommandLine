using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

using dotnetCampus.Cli.Properties;
using dotnetCampus.Cli.StateMachine;

namespace dotnetCampus.Cli.Standard
{
    internal class GnuOptions : CommandLineOptionParser<GnuOptions>
    {
        private readonly ResourceManager? _resourceManager;

        [Option(nameof(Version))]
        public bool Version { get; private set; }

        [Option('h', nameof(Help))]
        public bool Help { get; private set; }

        internal GnuOptions(CommandLine? commandLine)
        {
            _resourceManager = commandLine?.ResourceManager;

            AddMatch(nameof(Version), v => Version = v);
            AddMatch(nameof(Help), v => Help = v);
            SetResult(() => this);
        }

        internal void Run(IReadOnlyList<CommandLineVerbMatch<Task<int>>>? matches)
        {
            matches ??= new List<CommandLineVerbMatch<Task<int>>>();

            if (Help)
            {
                PrintDetailHelpText(matches);
            }
            else if (Version)
            {
                PrintVersionText();
            }
            else
            {
                PrintHelpText(matches);
            }
        }

        private void PrintHelpText(IReadOnlyList<CommandLineVerbMatch<Task<int>>> matches)
        {
            var helpOptionText = "-h|--help";
            var maxOptionTextLength = helpOptionText.Length;
            var verbs = matches.Select(x => x.VerbType.GetCustomAttribute<VerbAttribute>()).OfType<VerbAttribute>().ToList();
            var maxVerbTextLength = verbs.Count == 0 ? 0 : verbs.Max(x => x.VerbName.Length);
            var columnLength = Math.Max(maxOptionTextLength, maxVerbTextLength);

            Console.Write(LocalizableStrings.UsageHeader);
            Console.WriteLine("[options]");
            Console.WriteLine();

            Console.WriteLine(LocalizableStrings.OptionsHeader);
            Console.Write(GetColumnString(helpOptionText, columnLength));
            Console.WriteLine(LocalizableStrings.HelpOptionDescription);
            Console.WriteLine();

            if (verbs.Count > 0)
            {
                Console.WriteLine(LocalizableStrings.CommandHeader);
            }
            foreach (var verbAttribute in verbs)
            {
                Console.Write(GetColumnString(verbAttribute.VerbName, columnLength));
                Console.WriteLine(GetLocalizedDescription(verbAttribute));
            }
        }

        private static void PrintDetailHelpText(IReadOnlyList<CommandLineVerbMatch<Task<int>>> matches)
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

        private string GetColumnString(string originalString, int columnLength)
            => $"  {originalString.PadRight(columnLength, ' ')}  ";

        private string GetLocalizedDescription(CommandLineAttribute attribute)
            => _resourceManager != null && !string.IsNullOrWhiteSpace(attribute.LocalizableDescription)
                ? _resourceManager.GetString(attribute.LocalizableDescription, CultureInfo.CurrentUICulture) ?? ""
                : attribute.Description ?? "";
    }
}
