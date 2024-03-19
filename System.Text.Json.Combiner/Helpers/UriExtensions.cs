namespace System.Text.Json.Combiner
{
    internal static class UriExtensions
    {
        internal static string GetFilePath(this Uri uri)
        {
            return PathHelper.FixPath($"{uri.Host}{uri.PathAndQuery}");
        }
    }
}
