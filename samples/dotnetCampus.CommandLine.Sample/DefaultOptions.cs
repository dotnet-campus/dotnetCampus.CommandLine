using System;

using dotnetCampus.Cli.Properties;

namespace dotnetCampus.Cli
{
    internal class DefaultOptions
    {
        [Option(LocalizableDescription = nameof(LocalizableStrings.SamplePropertyDescription))]
        public string? DefaultProperty { get; set; }

        internal void Run()
        {
            Console.WriteLine("默认行为执行……");
        }
    }
}
