using System.IO;

namespace System.Text.Json.Combiner
{
    public class FileJsonLoader : IJsonLoader
    {
        public string Load(Uri uri, JsonLoaderContext ctx)
        {
            var uriPath = uri.GetFilePath();
            if (!Path.IsPathRooted(uriPath))
                uriPath = Path.Combine(ctx.cwd, uriPath);

            var fi = new FileInfo(uriPath);
            using (var fs = fi.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
