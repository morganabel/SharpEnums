using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Palit.SharpEnums.Converters;
using SharpEnums.Test.Models;
using Xunit;

namespace SharpEnums.Test.Converters
{
    public class StringSharpEnumConverterTests
    {
        [Fact]
        public void ItSerializesCorrectly()
        {
            var instance = GetTestInstance(TestSharpEnum.PartyTime | TestSharpEnum.Hungry);

            var json = JsonConvert.SerializeObject(instance);
            var jObject = JObject.Parse(json);

            Assert.NotNull(jObject);
            Assert.Equal("123", jObject["id"].Value<string>());
            Assert.Equal("partyTime, hungry", jObject["whatTimeIsIt"].Value<string>());
        }

        [Fact]
        public void ItDeserializesCorrectly()
        {
            var json = "{ \"id\": \"123\", \"whatTimeIsIt\": \"partyTime, hungry\" }";

            var instance = JsonConvert.DeserializeObject<TestEnumConverterClass>(json);

            Assert.NotNull(instance);
            Assert.Equal("123", instance.Id);
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Party));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Time));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Hungry));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.PartyTime | TestSharpEnum.Hungry));
        }

        [Fact]
        public void ItDeserializesCorrectlyFromInt()
        {
            var json = "{ \"id\": \"123\", \"whatTimeIsIt\": 11 }";

            var instance = JsonConvert.DeserializeObject<TestEnumConverterClass>(json);

            Assert.NotNull(instance);
            Assert.Equal("123", instance.Id);
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Party));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Time));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Hungry));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.PartyTime | TestSharpEnum.Hungry));
        }

        private TestEnumConverterClass GetTestInstance(TestSharpEnum enumValue) => new TestEnumConverterClass()
        {
            WhatTimeIsIt = enumValue
        };
    }

    internal class TestEnumConverterClass
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "123";

        [JsonProperty("whatTimeIsIt")]
        [JsonConverter(typeof(StringSharpEnumConverter<TestSharpEnum>), true)]
        public TestSharpEnum WhatTimeIsIt { get; set; }
    }
}
