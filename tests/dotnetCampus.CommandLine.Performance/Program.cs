using System.Reflection;
using BenchmarkDotNet.Running;

namespace dotnetCampus.CommandLine.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
