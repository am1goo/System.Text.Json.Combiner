using System.IO;

namespace System.Text.Json.Combiner
{
    public class FileJsonLoader : IJsonLoader
    {
        public string Load(Uri uri)
        {
            var uriPath = uri.GetFilePath();
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
