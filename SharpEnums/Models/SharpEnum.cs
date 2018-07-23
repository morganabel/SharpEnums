using Palit.SharpEnums.Attributes;
using Palit.SharpEnums.Interfaces;
using Palit.SharpEnums.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Palit.SharpEnums.Models
{
    /// <summary>
    /// Defines the <see cref="SharpEnum{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SharpEnum<T> : ISharpEnum, IEquatable<SharpEnum<T>> where T : class, ISharpEnum
    {
        /// <summary>
        /// Defines the CharSeperator
        /// </summary>
        public const char CharSeperator = ',';

        /// <summary>
        /// Defines the StringSeperator
        /// </summary>
        public const string StringSeperator = ", ";

        /// <summary>
        /// Defines the CharSeperatorAsArray
        /// </summary>
        public static readonly char[] CharSeperatorAsArray = new char[] { CharSeperator };

        /// <summary>
        /// Defines the optionsDictionary
        /// </summary>
        private static readonly Lazy<Dictionary<int, T>> optionsDictionary = new Lazy<Dictionary<int, T>>(GetAllUniqueOptionsAsDictionary);

        /// <summary>
        /// Defines the optionsNameDictionary
        /// </summary>
        private static readonly Lazy<Dictionary<string, T>> optionsNameDictionary = new Lazy<Dictionary<string, T>>(GetOptionsAsNameDictionary);

        /// <summary>
        /// Defines the allOptions
        /// </summary>
        private static readonly Lazy<List<T>> allOptions = new Lazy<List<T>>(GetAllOptions);

        /// <summary>
        /// Defines the isFlagsEnum
        /// </summary>
        private static readonly Lazy<bool> isFlagsEnum = new Lazy<bool>(IsFlagsEnum);

        /// <summary>
        /// Gets all unique values as a dictionary.
        /// </summary>
        /// <returns>The <see cref="Dictionary{int, T}"/></returns>
        private static Dictionary<int, T> GetAllUniqueOptionsAsDictionary() => allOptions.Value
                .ToDictionary(o => o.Value);

        /// <summary>
        /// Gets all options as a dictionary with name as they key.
        /// </summary>
        /// <returns>The <see cref="Dictionary{string, T}"/></returns>
        private static Dictionary<string, T> GetOptionsAsNameDictionary() => allOptions.Value
                .ToDictionary(o => o.Name);

        /// <summary>
        /// Gets all options for this enum.
        /// </summary>
        /// <returns>The <see cref="List{T}"/></returns>
        private static List<T> GetAllOptions()
        {
            var t = typeof(T);
            return t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(p => t.IsAssignableFrom(p.FieldType))
                .Select(pi => (T)pi.GetValue(null))
                .ToList();
        }

        /// <summary>
        /// Determines whether [is flags enum].
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        private static bool IsFlagsEnum()
        {
            var t = typeof(T);
            var flagsAttrs = t.GetCustomAttributes(typeof(SharpEnumFlagsAttribute), false);
            return flagsAttrs.Length > 0;
        }

        /// <summary>
        /// Gets the AllOptions
        /// </summary>
        public static List<T> AllOptions => allOptions.Value;

        /// <summary>
        /// Defines the DefaultValue
        /// </summary>
        public static T DefaultValue = optionsDictionary.Value.GetValueOrDefault(0);

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Value
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpEnum{T}"/> class.
        /// </summary>
        protected SharpEnum()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpEnum{T}"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="val">The val<see cref="int"/></param>
        protected SharpEnum(string name, int val)
        {
            Value = val;
            Name = name;
        }

        /// <summary>
        /// Determine if SmartEnum has all flags passed in.
        /// </summary>
        /// <param name="derivedEnum"></param>
        /// <returns></returns>
        public bool HasFlag(T derivedEnum) => IsValueSet(derivedEnum.Value, Value);

        /// <summary>
        /// Creates enum from int value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromValue(int value)
        {
            if (value <= 0)
            {
                return DefaultValue;
            }

            if (optionsDictionary.Value.ContainsKey(value))
            {
                return optionsDictionary.Value[value];
            }

            if (!isFlagsEnum.Value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Invalid value without SharpFlagsEnum attribute");
            }

            var names = new List<string>();
            var outputValue = 0;
            foreach (var option in optionsDictionary.Value.OrderByDescending(o => o.Key))
            {
                if (option.Key <= 0)
                {
                    continue;
                }

                var hasFlag = IsValueSet(option.Key, value);

                if (hasFlag && !IsValueSet(option.Key, outputValue))
                {
                    outputValue |= option.Key;
                    names.Add(option.Value.Name);
                }
            }

            if (outputValue == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            // Reverse sort if necessary.
            if (names.Count > 1)
            {
                names.Reverse();
            }

            return (T)Activator.CreateInstance(typeof(T), string.Join(StringSeperator, names), outputValue);
        }

        /// <summary>
        /// Creates enum from name string value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="caseInsensitive"></param>
        /// <returns></returns>
        public static T Parse(string name, bool caseInsensitive = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var names = name.Split(CharSeperatorAsArray, StringSplitOptions.RemoveEmptyEntries);

            // Non-flag enums should not have multiple values being parsed.
            if (names.Length > 1 && !isFlagsEnum.Value)
            {
                throw new ArgumentException(nameof(name), "Non-flag enums cannot have multiple values.");
            }

            // Find all the matching values by string comparison using either the dictionary or the list.
            var matchedOptions = new List<T>();
            foreach (var segment in names)
            {
                var token = segment.Trim();

                if (!optionsNameDictionary.Value.TryGetValue(token, out var match) && caseInsensitive)
                {
                    match = allOptions.Value.Find(o => o.Name.Equals(token, StringComparison.OrdinalIgnoreCase));
                }

                if (null != match)
                {
                    matchedOptions.Add(match);
                }
            }

            if (matchedOptions.Count == 0)
            {
                throw new ArgumentException(nameof(name));
            }

            // Get the value of the desired SmartEnum by getting the total value of all matched options.
            var outputValue = matchedOptions.Sum(m => m.Value);

            // Use from value to actually get the output.
            // A bit of a work around because there is no good way to use the OR operator within this function.
            return FromValue(outputValue);
        }


        /// <summary>
        /// Combine enum flags with OR bit operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T operator |(SharpEnum<T> a, SharpEnum<T> b)
        {
            if (a.Value == b.Value)
            {
                return (T)Activator.CreateInstance(typeof(T), a.Name, a.Value);
            }

            var combinedValue = a.Value + b.Value;
            if (optionsDictionary.Value.ContainsKey(combinedValue))
            {
                return optionsDictionary.Value[combinedValue];
            }

            var names = (a.Value < b.Value) ? new[] { a.Name, b.Name } : new[] { b.Name, a.Name };
            return (T)Activator.CreateInstance(typeof(T), string.Join(StringSeperator, names), combinedValue);
        }

        /// <summary>
        /// Toggle flags with XOR bit operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T operator ^(SharpEnum<T> a, SharpEnum<T> b)
        {
            var newValue = a.Value ^ b.Value;
            return FromValue(newValue);
        }

        /// <summary>
        /// Keep only mutual flags with AND bit operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T operator &(SharpEnum<T> a, SharpEnum<T> b)
        {
            var newValue = a.Value & b.Value;
            return FromValue(newValue);
        }

        /// <summary>
        /// Implicit operator to convert enum to int value.
        /// </summary>
        /// <param name="smartEnum"></param>
        public static implicit operator int(SharpEnum<T> smartEnum) => smartEnum.Value;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="enum1">The enum1.</param>
        /// <param name="enum2">The enum2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(SharpEnum<T> enum1, SharpEnum<T> enum2) => EqualityComparer<SharpEnum<T>>.Default.Equals(enum1, enum2);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="enum1">The enum1.</param>
        /// <param name="enum2">The enum2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(SharpEnum<T> enum1, SharpEnum<T> enum2) => !(enum1 == enum2);

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString() => Name;

        /// <summary>
        /// Is the input integer a power of two.
        /// </summary>
        /// <param name="x">The x<see cref="int"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private static bool IsPowerOfTwo(int x) => (x & (x - 1)) == 0;

        /// <summary>
        /// Is the value set based on the current value.
        /// </summary>
        /// <param name="input">The input<see cref="int"/></param>
        /// <param name="totalValue">The totalValue<see cref="int"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private static bool IsValueSet(int input, int totalValue) => (totalValue & input) == input;

        /// <summary>
        /// Is a particular bit set.
        /// </summary>
        /// <param name="b">The b<see cref="int"/></param>
        /// <param name="pos">The pos<see cref="int"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private static bool IsBitSet(int b, int pos) => (b & (1 << pos)) != 0;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => this.Equals(obj as SharpEnum<T>);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SharpEnum<T> other) => other != null
                   && this.Name == other.Name
                   && this.Value == other.Value;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -244751520;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name);
            hashCode = hashCode * -1521134295 + this.Value.GetHashCode();
            return hashCode;
        }
    }
}
