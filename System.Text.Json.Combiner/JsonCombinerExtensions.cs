using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace System.Text.Json.Combiner
{
    public static class JsonCombinerExtensions
    {
        public static void RemoveIfNeed(this IList<JsonConverter> list, JsonConverter converter)
        {
            var converterType = converter.GetType();
            RemoveIfNeed(list, converterType);
        }

        public static void RemoveIfNeed(this IList<JsonConverter> list, Type type)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (type.IsAssignableFrom(list[i].GetType()))
                {
                    list.RemoveAt(i);
                    --i;
                }
            }
        }

        public static void AddIfNeed(this IList<JsonConverter> list, params JsonConverter[] converters)
        {
            if (converters == null)
                return;

            foreach (var converter in converters)
            {
                AddIfNeed(list, converter);
            }
        }

        public static void AddIfNeed(this IList<JsonConverter> list, JsonConverter converter)
        {
            if (converter == null)
                return;

            if (Contains(list, converter.GetType()))
                return;

            list.Add(converter);
        }

        public static bool Contains(this IList<JsonConverter> list, Type type)
        {
            foreach (var c in list)
            {
                if (c.GetType().Equals(type))
                    return true;
            }
            return false;
        }
    }
}
