using Palit.SharpEnums.Interfaces;
using Palit.SharpEnums.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Palit.SharpEnums.Utilities
{
    /// <summary>
    /// Defines the <see cref="ExtensionMethods" />
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension method for converting string to camel case based on SmartEnum names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">The str<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string ToCamelCase<T>(this string str) where T : class, ISharpEnum
        {
            if (str.Length == 0)
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToLowerInvariant();
            }

            var parts = str.Split(SharpEnum<T>.CharSeperatorAsArray, StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder((parts.Length * 2) - 1);
            for (var i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(SharpEnum<T>.StringSeperator);
                }

                var trimmed = parts[i].Trim();
                sb.Append(char.ToLowerInvariant(trimmed[0])).Append(trimmed.Substring(1));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Extension method for getting value or default from IDictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">The dictionary<see cref="IDictionary{TKey, TValue}"/></param>
        /// <param name="key">The key<see cref="TKey"/></param>
        /// <param name="defaultValue">The defaultValue<see cref="TValue"/></param>
        /// <returns>The <see cref="TValue"/></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
