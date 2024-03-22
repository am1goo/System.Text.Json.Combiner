using NUnit.Framework;
using System.IO;
using System.Text.Json.Combiner;

namespace System.Text.Json.CombinerTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestDeserialization()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true,
            };
            var path = Path.Combine(Environment.CurrentDirectory, "Example\\text_object.json");
            var obj = JsonCombiner.Deserialize<TestObject>(path, options);

            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.param1, "param1");
            Assert.AreEqual(obj.param2, 2);
            Assert.AreEqual(obj.param3, 3.3f);

            var inner1 = obj.inner1;
            Assert.IsNotNull(inner1);
            Assert.AreEqual(inner1.arg1, "arg1");
            Assert.AreEqual(inner1.arg2, 22);
            Assert.AreEqual(inner1.arg3, 33.33f);

            var inner2 = obj.inner2;
            Assert.IsNotNull(inner2);
            Assert.AreEqual(inner2.arg1, "arg11");
            Assert.AreEqual(inner2.arg2, 222);
            Assert.AreEqual(inner2.arg3, 333.333f);

            var innerArray = obj.innerArray;
            Assert.IsNotNull(innerArray);
            Assert.AreEqual(innerArray.Length, 2);
        }

        public class TestObject
        {
            public string param1;
            public int param2;
            public float param3;
            public InnerObject inner1;
            public InnerObject inner2;
            public InnerObject[] innerArray;
            public InnerObject extern1;

            public class InnerObject : IJsonCombine
            {
                public IntentObject intent;
                public string arg1;
                public int arg2;
                public float arg3;
            }

            public class IntentObject : IJsonCombine
            {
                public string x;
                public int y;
                public float z;
            }
        }
    }
}