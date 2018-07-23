using Newtonsoft.Json;
using Palit.SharpEnums.Interfaces;
using Palit.SharpEnums.Models;
using System;

namespace Palit.SharpEnums.Converters
{
    /// <summary>
    /// Defines the <see cref="IntSharpEnumConverter{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntSharpEnumConverter<T> : JsonConverter where T : class, ISharpEnum
    {
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
                if (reader.TokenType == JsonToken.Integer)
                {
                    var intValue = Convert.ToInt32(reader.Value);

                    return SharpEnum<T>.FromValue(intValue);
                }

                if (reader.TokenType == JsonToken.String)
                {
                    return SharpEnum<T>.Parse(reader.Value.ToString(), true);
                }
            }
            catch (Exception)
            {
                throw new JsonSerializationException("");
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing smart enum.");
        }

        /// <summary>
        /// Writes json output.
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

            var output = e.Value;
            writer.WriteValue(output);
        }
    }
}
