using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace System.Text.Json.Combiner.Serialization
{
    public class JsonCombineConverter : JsonConverter<IJsonCombine>
    {
        private static readonly Type _interfaceType = typeof(IJsonCombine);

        private string _cwd;

        public JsonCombineConverter(string cwd)
        {
            this._cwd = cwd;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return _interfaceType.IsAssignableFrom(typeToConvert);
        }

        public override IJsonCombine Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var backup = reader;
            if (!TryGetObjectAsPath(ref reader, _cwd, out var path))
            {
                var o = JsonCombiner.CreateOptions(options, null);
                o.Converters.RemoveIfNeed(typeof(JsonCombineConverter));
                return (IJsonCombine)JsonSerializer.Deserialize(ref backup, typeToConvert, o);
            }

            var fi = new FileInfo(path);
            using (var fs = fi.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();

                    var cwd = fi.DirectoryName;
                    var c = new JsonCombineConverter(cwd);
                    var o = JsonCombiner.CreateOptions(options, c);
                    var obj = (IJsonCombine)JsonSerializer.Deserialize(json, typeToConvert, o);
                    return obj;
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, IJsonCombine value, JsonSerializerOptions options)
        {
        }

        private static bool TryGetObjectAsPath(ref Utf8JsonReader reader, string cwd, out string path)
        {
            path = null;
            var canRead = true;
            while (canRead && reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        var propertyName = reader.GetString();
                        if (string.Equals(propertyName, "path", StringComparison.InvariantCultureIgnoreCase) || 
                            string.Equals(propertyName, "include", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (reader.Read())
                            {
                                path = reader.GetString();
                                path = FixPath(path, cwd);
                            }
                        }
                        break;

                    case JsonTokenType.EndObject:
                        canRead = false;
                        break;
                }
            }
            return !string.IsNullOrWhiteSpace(path);
        }

        private static string FixPath(string path, string cwd)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Combine(cwd, path);

            var pathSeparator = Path.DirectorySeparatorChar;
            var oppositeSeparator = pathSeparator == '\\' ? '/' : '\\';
            return path.Replace(oppositeSeparator, pathSeparator);
        }
    }
}
