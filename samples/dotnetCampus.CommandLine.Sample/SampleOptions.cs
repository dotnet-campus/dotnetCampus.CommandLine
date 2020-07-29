using dotnetCampus.Cli.Properties;

namespace dotnetCampus.Cli
{
    [Verb("sample", LocalizableDescription = nameof(LocalizableStrings.SampleVerbDescription))]
    internal class SampleOptions
    {
        [Option(LocalizableDescription = nameof(LocalizableStrings.SamplePropertyDescription))]
        public string? SampleProperty { get; set; }

        internal void Run()
        {
        }
    }
}
