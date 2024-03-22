using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace System.Text.Json.Combiner.Serialization
{
    public abstract class JsonCombineBaseConverter<T> : JsonConverter<T>
    {
        private static Dictionary<string, IJsonLoader> _loaders = new Dictionary<string, IJsonLoader>
        {
            { "file", new FileJsonLoader() },
            { "http", new HttpJsonLoader() },
            { "https", new HttpJsonLoader() },
        };

        protected abstract Type interfaceType { get; }

        private string _cwd;
        protected string cwd => _cwd;

        public JsonCombineBaseConverter(string cwd)
        {
            this._cwd = cwd;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return interfaceType.IsAssignableFrom(typeToConvert);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var o = JsonCombiner.CreateOptions(options);
            o.Converters.RemoveIfNeed(typeof(JsonCombineConverter));
            o.Converters.RemoveIfNeed(typeof(JsonCombineArrayConverter));

            var backup = reader;
            if (TryGetObjectAsPath(ref reader, _cwd, out var uri))
            {
                if (!_loaders.TryGetValue(uri.Scheme, out var loader))
                    throw new Exception($"unsupported scheme {uri.Scheme}");
                
                var json = loader.Load(uri);
                var obj = (T)JsonSerializer.Deserialize(json, typeToConvert, o);
                return obj;
            }
            else
            {
                var obj = (T)JsonSerializer.Deserialize(ref backup, typeToConvert, o);
                return obj;
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
        }

        private static bool TryGetObjectAsPath(ref Utf8JsonReader reader, string cwd, out Uri result)
        {
            result = null;
            var canRead = true;
            do
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        {
                            var path = reader.GetString();
                            if (TryParsePath(path, cwd, out var uri))
                            {
                                result = uri;
                            }
                            canRead = false;
                            break;
                        }

                    case JsonTokenType.PropertyName:
                        {
                            var propertyName = reader.GetString();
                            if (string.Equals(propertyName, "include", StringComparison.InvariantCultureIgnoreCase))
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
                        }

                    case JsonTokenType.EndObject:
                        canRead = false;
                        break;
                }
            }
            while (canRead && reader.Read());

            return result != null;
        }

        private static bool TryParsePath(string parsed, string cwd, out Uri result)
        {
            if (Uri.TryCreate(parsed, UriKind.Absolute, out result))
            {
                switch (result.Scheme)
                {
                    case "file":
                        return TryParseFileUri(result, cwd, out result);
                    default:
                        return true;
                }
            }
            else
            {
                return TryParseFileUri(parsed, cwd, out result);
            }
        }

        private static bool TryParseFileUri(string path, string cwd, out Uri result)
        {
            if (!path.StartsWith("file://"))
                path = $"file://{path}";

            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri))
            {
                result = default;
                return false;
            }

            return TryParseFileUri(uri, cwd, out result);
        }

        private static bool TryParseFileUri(Uri uri, string cwd, out Uri result)
        {
            var uriPath = uri.GetFilePath();
            if (!Path.IsPathRooted(uriPath))
                uriPath = Path.Combine(cwd, uriPath);

            result = new Uri($"{uri.Scheme}://{uriPath}");
            return true;
        }
    }
}
