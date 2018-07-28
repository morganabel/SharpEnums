using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Palit.SharpEnums.Interfaces;
using Palit.SharpEnums.Models;
using Palit.SharpEnums.Utilities;
using System;
using System.Linq;

namespace Palit.SharpEnums.Converters
{
    /// <summary>
    /// Defines the <see cref="StringArraySharpEnumConverter{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringArraySharpEnumConverter<T> : JsonConverter where T : class, ISharpEnum
    {
        /// <summary>
        /// Gets or sets a value indicating whether the written enum text should be camel case.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool CamelCaseText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [safe convert].
        /// Safe convert uses TryParse and TryFromValue and will not throw errors on unmatched input.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [safe convert]; otherwise, <c>false</c>.
        /// </value>
        public bool SafeConvert { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringArraySharpEnumConverter{T}"/> class.
        /// </summary>
        /// <param name="camelCaseText">if set to <c>true</c> [camel case text].</param>
        public StringArraySharpEnumConverter(bool camelCaseText) => CamelCaseText = camelCaseText;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringArraySharpEnumConverter{T}"/> class.
        /// </summary>
        /// <param name="camelCaseText">if set to <c>true</c> [camel case text].</param>
        /// <param name="safeConvert">if set to <c>true</c> [safe convert].</param>
        public StringArraySharpEnumConverter(bool camelCaseText, bool safeConvert) : this(camelCaseText) => SafeConvert = safeConvert;

        /// <summary>
        /// Determines if this converter is applicable.
        /// </summary>
        /// <param name="objectType">The objectType<see cref="Type"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public override bool CanConvert(Type objectType) => objectType is ISharpEnum;

        /// <summary>
        /// Reads json input.
        /// </summary>
        /// <param name="reader">The reader<see cref="JsonReader"/></param>
        /// <param name="objectType">The objectType<see cref="Type"/></param>
        /// <param name="existingValue">The existingValue<see cref="object"/></param>
        /// <param name="serializer">The serializer<see cref="JsonSerializer"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Handle null?

            try
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    var jTokenType = JTokenType.None;

                    var array = JArray.Load(reader);
                    if (array.Any())
                    {
                        jTokenType = array.First.Type;
                    }

                    if (jTokenType == JTokenType.String)
                    {
                        var enumText = string.Join(SharpEnum<T>.StringSeperator, array.Values<string>());

                        if (SafeConvert)
                        {
                            SharpEnum<T>.TryParse(enumText, out var output);
                            return output;
                        }

                        return SharpEnum<T>.Parse(enumText);
                    }
                }

                if (reader.TokenType == JsonToken.String)
                {
                    if (SafeConvert)
                    {
                        SharpEnum<T>.TryParse(reader.Value.ToString(), out var output, true);
                        return output;
                    }

                    return SharpEnum<T>.Parse(reader.Value.ToString(), true);
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    var intValue = Convert.ToInt32(reader.Value);

                    if (SafeConvert)
                    {
                        SharpEnum<T>.TryFromValue(intValue, out var output);
                        return output;
                    }

                    return SharpEnum<T>.FromValue(intValue);
                }
            }
            catch (Exception)
            {
                throw new JsonSerializationException("");
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing smart enum.");
        }

        /// <summary>
        /// Write json output.
        /// </summary>
        /// <param name="writer">The writer<see cref="JsonWriter"/></param>
        /// <param name="value">The value<see cref="object"/></param>
        /// <param name="serializer">The serializer<see cref="JsonSerializer"/></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var e = value as ISharpEnum;

            writer.WriteStartArray();

            var output = (CamelCaseText) ? e.Name.ToCamelCase<T>() : e.Name;
            foreach (var item in output.Split(SharpEnum<T>.CharSeperatorAsArray, StringSplitOptions.RemoveEmptyEntries))
            {
                writer.WriteValue(item.Trim());
            }

            writer.WriteEndArray();
        }
    }
}
