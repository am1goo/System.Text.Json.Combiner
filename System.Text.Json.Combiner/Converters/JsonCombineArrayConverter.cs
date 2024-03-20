namespace System.Text.Json.Combiner.Serialization
{
    public class JsonCombineArrayConverter : JsonCombineBaseConverter<IJsonCombine[]>
    {
        private static readonly Type _interfaceType = typeof(IJsonCombine[]);
        protected override Type interfaceType => _interfaceType;

        public JsonCombineArrayConverter(string cwd) : base(cwd)
        {
        }

        public override IJsonCombine[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return base.Read(ref reader, typeToConvert, options);
        }
    }
}
