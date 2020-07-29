#pragma warning disable CA1303 // 请不要将文本作为本地化参数传递

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Cli.StateMachine;
using dotnetCampus.Cli.Utils;

namespace dotnetCampus.Cli.Standard
{
    internal class GnuOptions : CommandLineOptionParser<GnuOptions>
    {
        private readonly ResourceManager? _resourceManager;

        [NotNull]
        private LocalizableStrings? _localizableStrings;

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
            _localizableStrings = new LocalizableStrings();
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
                        Description = x.Type!.Assembly == selfAssembly
                            ? GetLocalizedDescription(x.Attribute, _localizableStrings)
                            : GetLocalizedDescription(x.Attribute, _resourceManager),
                    })
                ).ToList();

            var maxVerbTextLength = verbInfoList.Count == 0 ? 0 : verbInfoList.Max(x => x.Name.Length);
            var maxOptionTextLength = mergedOptionInfoList.Count == 0 ? 0 : mergedOptionInfoList.Max(x => x.Name.Length);
            var columnLength = Math.Max(maxOptionTextLength, maxVerbTextLength);
            columnLength = Math.Max(12, columnLength);

            Console.Write(_localizableStrings.UsageHeader);
            if (verbInfoList.Count > 0)
            {
                Console.WriteLine("[options] [command] [command-options] [arguments]");
            }
            else
            {
                Console.WriteLine("[options] [arguments]");
            }

            Console.WriteLine();

            if (mergedOptionInfoList.Count > 0)
            {
                Console.WriteLine(_localizableStrings.OptionsHeader);
            }
            foreach (var x in mergedOptionInfoList)
            {
                Console.Write(GetColumnString(x.Name, columnLength));
                Console.WriteLine(x.Description);
            }

            Console.WriteLine();

            if (verbInfoList.Count > 0)
            {
                Console.WriteLine(_localizableStrings.CommandHeader);
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

        private static string GetLocalizedDescription(CommandLineAttribute attribute, LocalizableStrings resourceManager)
            => attribute.LocalizableDescription != null && !string.IsNullOrWhiteSpace(attribute.LocalizableDescription)
                ? resourceManager.GetString(attribute.LocalizableDescription, CultureInfo.CurrentUICulture) ?? ""
                : attribute.Description ?? "";

        /// <summary>
        /// 为了避免引用此 NuGet 包带来大量的本地化附属库的引用，我们不能使用会生成额外程序集的本地化方案。
        /// </summary>
        private class LocalizableStrings
        {
            private Dictionary<string, string> _defaultDictionary = new Dictionary<string, string>
            {
                { "", "" },
            };

            internal string VersionOptionDescription => GetString(Thread.CurrentThread.CurrentUICulture);
            internal Dictionary<string, string> _versionOptionDescription = new Dictionary<string, string>
            {
                {  "", "Display version." },
                { "zh-CN", "显示版本。" },
            };

            internal string HelpOptionDescription => GetString(Thread.CurrentThread.CurrentUICulture);
            internal Dictionary<string, string> _helpOptionDescription = new Dictionary<string, string>
            {
                {  "", "Display help." },
                { "zh-CN", "显示使用说明。" },
            };

            internal string UsageHeader => GetString(Thread.CurrentThread.CurrentUICulture);
            internal Dictionary<string, string> _usageHeader = new Dictionary<string, string>
            {
                {  "", "Usage: " },
                { "zh-CN", "使用：" },
            };

            internal string OptionsHeader => GetString(Thread.CurrentThread.CurrentUICulture);
            internal Dictionary<string, string> _optionsHeader = new Dictionary<string, string>
            {
                {  "", "Options: " },
                { "zh-CN", "选项：" },
            };

            internal string CommandHeader => GetString(Thread.CurrentThread.CurrentUICulture);
            internal Dictionary<string, string> _commandHeader = new Dictionary<string, string>
            {
                {  "", "Commands: " },
                { "zh-CN", "命令：" },
            };

            private string GetString(CultureInfo culture, [CallerMemberName] string? propertyName = null)
                => GetString(propertyName!, culture) ?? "";

            internal string? GetString(string name, CultureInfo currentUICulture)
            {
                var dictionary = name switch
                {
                    nameof(VersionOptionDescription) => _versionOptionDescription,
                    nameof(HelpOptionDescription) => _helpOptionDescription,
                    nameof(UsageHeader) => _usageHeader,
                    nameof(OptionsHeader) => _optionsHeader,
                    nameof(CommandHeader) => _commandHeader,
                    _ => _defaultDictionary,
                };
                return dictionary.TryGetValue(currentUICulture.Name, out var text)
                    ? text
                    : dictionary[""];
            }
        }
    }
}
