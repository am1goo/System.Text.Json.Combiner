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
            if (!TryGetObjectAsPath(ref reader, _cwd, out var uri))
            {
                var o = JsonCombiner.CreateOptions(options, null);
                o.Converters.RemoveIfNeed(typeof(JsonCombineConverter));
                return (IJsonCombine)JsonSerializer.Deserialize(ref backup, typeToConvert, o);
            }

            switch (uri.Scheme)
            {
                case "file":
                    var uriPath = uri.GetFilePath();
                    var fi = new FileInfo(uriPath);
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

                default:
                    return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, IJsonCombine value, JsonSerializerOptions options)
        {
        }

        private static bool TryGetObjectAsPath(ref Utf8JsonReader reader, string cwd, out Uri result)
        {
            result = null;
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
                                var path = reader.GetString();
                                if (TryParsePath(path, cwd, out var uri))
                                {
                                    result = uri;
                                }
                            }
                        }
                        break;

                    case JsonTokenType.EndObject:
                        canRead = false;
                        break;
                }
            }
            return result != null;
        }

        private static bool TryParsePath(string path, string cwd, out Uri result)
        {
            if (!path.StartsWith("file://"))
                path = $"file://{path}";

            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri))
            {
                result = default;
                return false;
            }

            var uriPath = uri.GetFilePath();
            if (!Path.IsPathRooted(uriPath))
                uriPath = Path.Combine(cwd, uriPath);

            result = new Uri($"{uri.Scheme}://{uriPath}");
            return true;
        }
    }
}
