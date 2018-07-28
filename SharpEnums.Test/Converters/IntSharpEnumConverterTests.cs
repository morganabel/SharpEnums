using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Palit.SharpEnums.Converters;
using SharpEnums.Test.Models;
using Xunit;

namespace SharpEnums.Test.Converters
{
    public class IntSharpEnumConverterTests
    {
        [Fact]
        public void ItSerializesCorrectly()
        {
            var instance = GetTestInstance(TestSharpEnum.PartyTime | TestSharpEnum.Hungry);

            var json = JsonConvert.SerializeObject(instance);
            var jObject = JObject.Parse(json);

            Assert.NotNull(jObject);
            Assert.Equal("123", jObject["id"].Value<string>());
            Assert.Equal(11, jObject["whatTimeIsIt"].Value<int>());
        }

        [Fact]
        public void ItDeserializesCorrectly()
        {
            var json = "{ \"id\": \"123\", \"whatTimeIsIt\": 11 }";

            var instance = JsonConvert.DeserializeObject<TestIntEnumConverterClass>(json);

            Assert.NotNull(instance);
            Assert.Equal("123", instance.Id);
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Party));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Time));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Hungry));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.PartyTime | TestSharpEnum.Hungry));
        }

        [Fact]
        public void ItThrowsOnUnsafeDeserializeInvalidValue()
        {
            var json = $"{{ \"id\": \"123\", \"whatTimeIsIt\": {TestSharpEnum.InvalidValue} }}";

            var exception = Record.Exception(() => JsonConvert.DeserializeObject<TestIntEnumConverterClass>(json));

            Assert.NotNull(exception);
            Assert.IsType<JsonSerializationException>(exception);
        }

        [Fact]
        public void ItShouldReturnDefaultValueOnSafeDeserializeInvalidValue()
        {
            var json = $"{{ \"id\": \"123\", \"whatTimeIsIt\": {TestSharpEnum.InvalidValue} }}";

            var instance = JsonConvert.DeserializeObject<TestIntEnumConvertSafeClass>(json);

            Assert.NotNull(instance);
            Assert.StrictEqual(TestSharpEnum.DefaultValue, instance.WhatTimeIsIt);
        }

        private TestIntEnumConverterClass GetTestInstance(TestSharpEnum enumValue) => new TestIntEnumConverterClass()
        {
            WhatTimeIsIt = enumValue
        };
    }

    internal class TestIntEnumConverterClass
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "123";

        [JsonProperty("whatTimeIsIt")]
        [JsonConverter(typeof(IntSharpEnumConverter<TestSharpEnum>))]
        public TestSharpEnum WhatTimeIsIt { get; set; }
    }

    internal class TestIntEnumConvertSafeClass
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "123";

        [JsonProperty("whatTimeIsIt")]
        [JsonConverter(typeof(IntSharpEnumConverter<TestSharpEnum>), true)]
        public TestSharpEnum WhatTimeIsIt { get; set; }
    }
}
