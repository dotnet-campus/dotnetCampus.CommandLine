using dotnetCampus.Cli.Properties;

namespace dotnetCampus.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parse(args, LocalizableStrings.ResourceManager)
                .AddHandler<SampleOptions>(o => o.Run())
                .Run();
        }
    }
}
