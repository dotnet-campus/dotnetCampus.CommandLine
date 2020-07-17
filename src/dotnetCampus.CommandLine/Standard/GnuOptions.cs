using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

using dotnetCampus.Cli.Properties;
using dotnetCampus.Cli.StateMachine;
using dotnetCampus.Cli.Utils;

namespace dotnetCampus.Cli.Standard
{
    internal class GnuOptions : CommandLineOptionParser<GnuOptions>
    {
        private readonly ResourceManager? _resourceManager;

        [Option(nameof(Version), LocalizableDescription = nameof(LocalizableStrings.VersionOptionDescription))]
        public bool Version { get; private set; }

        [Option('h', nameof(Help), LocalizableDescription = nameof(LocalizableStrings.HelpOptionDescription))]
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
            var selfAssembly = typeof(GnuOptions).Assembly;
            var verbInfoList = matches
                .Select(x => x.VerbType.GetCustomAttribute<VerbAttribute>())
                .OfType<VerbAttribute>()
                .Select(x => new { Name = x.VerbName, Description = GetLocalizedDescription(x, _resourceManager) })
                .ToList();
            var mergedOptionInfoList = matches
                .Where(x => !x.VerbType.IsDefined(typeof(VerbAttribute)))
                .SelectMany(x => x.VerbType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.IsDefined(typeof(OptionAttribute)))
                    .Select(x => new { Type = x.DeclaringType, Property = x, Attribute = x.GetCustomAttribute<OptionAttribute>() })
                    .Select(x => new { x.Attribute!.ShortName, LongName = x.Attribute.LongName ?? x.Property.Name, x.Attribute, x.Type })
                    .Select(x => new
                    {
                        Name = x.ShortName is null ? $"--{NamingHelper.MakeKebabCase(x.LongName)}" : $"-{x.ShortName}|--{NamingHelper.MakeKebabCase(x.LongName)}",
                        Description = GetLocalizedDescription(x.Attribute, x.Type!.Assembly == selfAssembly ? LocalizableStrings.ResourceManager : _resourceManager)
                    })
                ).ToList();

            var maxVerbTextLength = verbInfoList.Count == 0 ? 0 : verbInfoList.Max(x => x.Name.Length);
            var maxOptionTextLength = mergedOptionInfoList.Count == 0 ? 0 : mergedOptionInfoList.Max(x => x.Name.Length);
            var columnLength = Math.Max(maxOptionTextLength, maxVerbTextLength);
            columnLength = Math.Max(12, columnLength);

            Console.Write(LocalizableStrings.UsageHeader);
            Console.WriteLine("[options]");

            Console.WriteLine();

            if (mergedOptionInfoList.Count > 0)
            {
                Console.WriteLine(LocalizableStrings.OptionsHeader);
            }
            foreach (var x in mergedOptionInfoList)
            {
                Console.Write(GetColumnString(x.Name, columnLength));
                Console.WriteLine(x.Description);
            }

            Console.WriteLine();

            if (verbInfoList.Count > 0)
            {
                Console.WriteLine(LocalizableStrings.CommandHeader);
            }
            foreach (var x in verbInfoList)
            {
                Console.Write(GetColumnString(x.Name, columnLength));
                Console.WriteLine(x.Description);
            }
        }

        private void PrintDetailHelpText(IReadOnlyList<CommandLineVerbMatch<Task<int>>> matches)
        {
            PrintHelpText(matches);
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

        private static string GetColumnString(string originalString, int columnLength)
            => $"  {originalString.PadRight(columnLength, ' ')}  ";

        private static string GetLocalizedDescription(CommandLineAttribute attribute, ResourceManager? resourceManager)
            => resourceManager != null && !string.IsNullOrWhiteSpace(attribute.LocalizableDescription)
                ? resourceManager.GetString(attribute.LocalizableDescription, CultureInfo.CurrentUICulture) ?? ""
                : attribute.Description ?? "";
    }
}
