using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace System.Text.Json.Combiner.Serialization
{
    public abstract class JsonCombineBaseConverter<T> : JsonConverter<T>
    {
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
            o.Converters.RemoveIfNeed(typeof(JsonCombineObjectConverter));
            o.Converters.RemoveIfNeed(typeof(JsonCombineArrayConverter));

            var backup = reader;
            if (TryGetObjectAsPath(ref reader, out var uri))
            {
                if (!JsonCombiner.loaders.TryGetValue(uri.Scheme, out var loader))
                    throw new Exception($"unsupported scheme {uri.Scheme}");

                var ctx = new JsonLoaderContext
                {
                    cwd = _cwd,
                };
                var json = loader.Load(uri, ctx);
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

        private static bool TryGetObjectAsPath(ref Utf8JsonReader reader, out Uri result)
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
                            result = CreateUri(path);
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
                                    result = CreateUri(path);
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

        private static Uri CreateUri(string parsed)
        {
            if (Uri.TryCreate(parsed, UriKind.Absolute, out var result))
            {
                return result;
            }
            else
            {
                return new Uri($"file://{parsed}");
            }
        }
    }
}
