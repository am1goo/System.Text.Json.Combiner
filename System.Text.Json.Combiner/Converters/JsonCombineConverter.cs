namespace System.Text.Json.Combiner.Serialization
{
    public class JsonCombineConverter : JsonCombineBaseConverter<IJsonCombine>
    {
        private static readonly Type _interfaceType = typeof(IJsonCombine);
        protected override Type interfaceType => _interfaceType;

        public JsonCombineConverter(string cwd) : base(cwd)
        {
        }
    }
}
