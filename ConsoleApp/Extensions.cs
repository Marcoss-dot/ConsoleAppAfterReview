using System;

namespace ConsoleApp
{
    public static class Extensions
    {
        public static string FullTrim(this string s)
        {
            return s.Trim().Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
        }
    }
}
