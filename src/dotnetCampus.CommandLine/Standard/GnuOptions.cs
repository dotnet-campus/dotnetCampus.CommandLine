//using System;

//namespace dotnetCampus.Cli.Standard
//{
//    public class GnuOptions : CommandLineOptionParser<GnuOptions>
//    {
//        [Option(nameof(Version))]
//        public bool Version { get; private set; }

//        [Option(nameof(Help))]
//        public bool Help { get; private set; }

//        internal GnuOptions()
//        {
//            AddMatch(nameof(Version), v => Version = v);
//            AddMatch(nameof(Help), v => Help = v);
//            SetResult(() => this);
//        }

//        internal void Run()
//        {
//            if (Version)
//            {
//            }
//            Console.WriteLine();
//        }
//    }
//}
