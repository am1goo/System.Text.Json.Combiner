using System.IO;
using System.Text.Json.Combiner.Serialization;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Combiner
{
    public class JsonCombiner
    {
        public static T Deserialize<T>(string path, JsonSerializerOptions options = null)
        {
            var fi = new FileInfo(path);
            using (var fs = fi.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();

                    var cwd = fi.DirectoryName;
                    var c = new JsonCombineConverter(cwd);
                    var o = CreateOptions(options, c);
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
                var c = new JsonCombineConverter(cwd);
                var o = CreateOptions(options, c);
                return await JsonSerializer.DeserializeAsync<T>(fs, o, cancellationToken);
            }
        }

        public static JsonSerializerOptions CreateOptions(JsonSerializerOptions options, JsonConverter converter)
        {
            JsonSerializerOptions result;
            if (options == null)
                result = new JsonSerializerOptions();
            else
                result = new JsonSerializerOptions(options);

            result.Converters.AddIfNeed(converter);
            return result;
        }
    }
}
