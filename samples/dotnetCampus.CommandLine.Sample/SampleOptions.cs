using System;

using dotnetCampus.Cli.Properties;

namespace dotnetCampus.Cli
{
    internal class SampleOptions
    {
        [Option(LocalizableDescription = nameof(LocalizableStrings.SamplePropertyDescription))]
        public string? SampleProperty { get; set; }

        internal void Run()
        {

        }
    }
}
