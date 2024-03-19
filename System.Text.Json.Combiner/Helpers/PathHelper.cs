using System.IO;

namespace System.Text.Json.Combiner
{
    internal class PathHelper
    {
        public static readonly char pathSeparator = Path.DirectorySeparatorChar;
        public static readonly char oppositeSeparator = pathSeparator == '\\' ? '/' : '\\';

        internal static string FixPath(string str)
        {
            return str.Replace(oppositeSeparator, pathSeparator).TrimEnd(pathSeparator).TrimEnd(oppositeSeparator);
        }
    }
}
