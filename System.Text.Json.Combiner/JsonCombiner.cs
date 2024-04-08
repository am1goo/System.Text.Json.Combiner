using System.Collections.Generic;
using System.IO;
using System.Text.Json.Combiner.Serialization;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Combiner
{
    public class JsonCombiner
    {
        private static Dictionary<string, IJsonLoader> _loaders = new Dictionary<string, IJsonLoader>
        {
            { "file",   new FileJsonLoader() },
            { "http",   new HttpJsonLoader() },
            { "https",  new HttpJsonLoader() },
        };
        public static IReadOnlyDictionary<string, IJsonLoader> loaders => _loaders;

        public static T Deserialize<T>(string path, JsonSerializerOptions options = null)
        {
            var fi = new FileInfo(path);
            using (var fs = fi.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();

                    var cwd = fi.DirectoryName;
                    var c1 = new JsonCombineObjectConverter(cwd);
                    var c2 = new JsonCombineArrayConverter(cwd);
                    var o = CreateOptions(options, c1, c2);
                    return JsonSerializer.Deserialize<T>(json, o);
                }
            }
        }

        public static async ValueTask<T> DeserializeAsync<T>(string path, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            var fi = new FileInfo(path);
            using (var fs = fi.OpenRead())
            {
                var cwd = fi.DirectoryName;
                var c1 = new JsonCombineObjectConverter(cwd);
                var c2 = new JsonCombineArrayConverter(cwd);
                var o = CreateOptions(options, c1, c2);
                return await JsonSerializer.DeserializeAsync<T>(fs, o, cancellationToken);
            }
        }

        public static JsonSerializerOptions CreateOptions(JsonSerializerOptions options, params JsonConverter[] converters)
        {
            JsonSerializerOptions result;
            if (options == null)
                result = new JsonSerializerOptions();
            else
                result = new JsonSerializerOptions(options);

            result.Converters.AddIfNeed(converters);
            return result;
        }

        public static bool RegisterLoader<T>(string scheme, T loader, bool @override = false) where T : IJsonLoader
        {
            if (string.IsNullOrWhiteSpace(scheme))
                return false;

            if (loader == null)
                return false;

            if (_loaders.ContainsKey(scheme))
            {
                if (@override)
                    _loaders[scheme] = loader;
                else
                    return false;
            }

            _loaders.Add(scheme, loader);
            return true;
        }
    }
}
