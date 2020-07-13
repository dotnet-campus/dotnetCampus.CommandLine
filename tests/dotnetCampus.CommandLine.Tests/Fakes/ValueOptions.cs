using System.Collections.Generic;

namespace dotnetCampus.Cli.Tests.Fakes
{
    public class ValueOptions
    {
        [Option('f', nameof(Foo))]
        public string Foo { get; set; }

        [Value(0)]
        public long LongValue { get; set; }

        [Value(1, 2)]
        public IReadOnlyList<string> Values { get; set; }

        [Value(2)]
        public int Int32Value { get; set; }
    }

    public class UnlimitedValueOptions
    {
        [Option('s', nameof(Section))]
        public string Section { get; set; }

        [Value(0)]
        public int Count { get; set; }

        [Value(1, int.MaxValue)]
        public IEnumerable<string> Args { get; set; }
    }
}
