using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Xunit;
using Palit.SharpEnums.Converters;
using SharpEnums.Test.Models;

namespace UtilityTests.Converters
{
    public class StringArraySharpEnumConverterTests
    {
        [Fact]
        public void ItSerializesCorrectly()
        {
            var instance = GetTestInstance(TestSharpEnum.PartyTime | TestSharpEnum.Hungry);

            var json = JsonConvert.SerializeObject(instance);
            var jObject = JObject.Parse(json);
            var arr = jObject["whatTimeIsIt"].Values<string>().ToArray();

            Assert.NotNull(jObject);
            Assert.Equal("123", jObject["id"].Value<string>());
            Assert.Equal(2, arr.Length);
            Assert.Equal("partyTime", arr.First());
            Assert.Equal("hungry", arr.Last());
        }

        [Fact]
        public void ItDeserializesCorrectly()
        {
            var json = "{ \"id\": \"123\", \"whatTimeIsIt\": [\"partyTime\", \"hungry\"] }";

            var instance = JsonConvert.DeserializeObject<TestStringArrayEnumConverterClass>(json);

            Assert.NotNull(instance);
            Assert.Equal("123", instance.Id);
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Party));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Time));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.Hungry));
            Assert.True(instance.WhatTimeIsIt.HasFlag(TestSharpEnum.PartyTime | TestSharpEnum.Hungry));
        }

        private TestStringArrayEnumConverterClass GetTestInstance(TestSharpEnum enumValue) => new TestStringArrayEnumConverterClass()
        {
            WhatTimeIsIt = enumValue
        };

        internal class TestStringArrayEnumConverterClass
        {
            [JsonProperty("id")]
            public string Id { get; set; } = "123";

            [JsonProperty("whatTimeIsIt")]
            [JsonConverter(typeof(StringArraySharpEnumConverter<TestSharpEnum>), true)]
            public TestSharpEnum WhatTimeIsIt { get; set; }
        }
    }
}
