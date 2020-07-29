#if NETCOREAPP3_0 || NETCOREAPP3_1 || NETCOREAPP5_1 || NET5_0 || NET6_0
#else
namespace System
{
    internal static class NET45Compatibility
    {
        public static int IndexOf(this string @string, char value, StringComparison stringComparison)
        {
            return @string.IndexOf(value.ToString(), stringComparison);
        }
    }
}
#endif
