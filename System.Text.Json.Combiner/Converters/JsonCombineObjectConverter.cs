namespace System.Text.Json.Combiner.Serialization
{
    public class JsonCombineObjectConverter : JsonCombineBaseConverter<IJsonCombine>
    {
        private static readonly Type _interfaceType = typeof(IJsonCombine);
        protected override Type interfaceType => _interfaceType;

        public JsonCombineObjectConverter(string cwd) : base(cwd)
        {
        }
    }
}
