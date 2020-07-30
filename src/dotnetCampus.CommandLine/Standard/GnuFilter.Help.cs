﻿#pragma warning disable CA1303 // 请不要将文本作为本地化参数传递

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

using dotnetCampus.Cli.Utils;

namespace dotnetCampus.Cli.Standard
{
    partial class GnuFilter
    {
        private void PrintHelpText(IReadOnlyList<Type> relatedTypes)
        {
            var selfAssembly = typeof(GnuFilter).Assembly;
            var verbInfoList = relatedTypes
                .Select(x => x.GetCustomAttribute<VerbAttribute>())
                .OfType<VerbAttribute>()
                .Select(x => new { Name = x.VerbName, Description = GetLocalizedDescription(x, _resourceManager) })
                .ToList();
            var mergedOptionInfoList = relatedTypes
                .Where(x => !x.IsDefined(typeof(VerbAttribute)))
                .SelectMany(x => x.GetProperties(BindingFlags.Instance | BindingFlags.Public)
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

        private void PrintVerbHelpText(Type verbType)
        {
            Console.WriteLine("--------------------------");
        }

        private void PrintUnknownVerbHelpText(string? verb)
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is null)
            {
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, _localizableStrings.UnknownCommandFormat, verb));
            }
            else
            {
                var name = Path.GetFileNameWithoutExtension(assembly.Location);
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, _localizableStrings.UnknownCommandFormat, name, verb));
            }
        }

        private void PrintDetailHelpText(IReadOnlyList<Type> relatedTypes)
        {
            PrintHelpText(relatedTypes);
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
    }
}
