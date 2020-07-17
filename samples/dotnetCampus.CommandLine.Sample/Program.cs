using dotnetCampus.Cli.Properties;
using dotnetCampus.Cli.Standard;

namespace dotnetCampus.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parse(args, LocalizableStrings.ResourceManager)
                .AddStandardHandlers()
                .AddHandler<SampleOptions>(o => o.Run())
                .Run();
        }
    }
}
