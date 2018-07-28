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
        private static readonly Lazy<IReadOnlyDictionary<int, T>> _optionsDictionary = new Lazy<IReadOnlyDictionary<int, T>>(GetAllUniqueOptionsAsDictionary);

        /// <summary>
        /// Defines the optionsNameDictionary
        /// </summary>
        private static readonly Lazy<IReadOnlyDictionary<string, T>> _optionsNameDictionary = new Lazy<IReadOnlyDictionary<string, T>>(GetOptionsAsNameDictionary);

        /// <summary>
        /// Defines the list of all options.
        /// </summary>
        private static readonly Lazy<List<T>> _allOptions = new Lazy<List<T>>(GetAllOptions);

        /// <summary>
        /// Lazy initialized all options sorted by value desc.
        /// </summary>
        private static readonly Lazy<KeyValuePair<int, T>[]> _allOptionsSortedByValueDesc = new Lazy<KeyValuePair<int, T>[]>(GetOptionsSortedByValueDesc);

        /// <summary>
        /// Defines the isFlagsEnum
        /// </summary>
        private static readonly Lazy<bool> _isFlagsEnum = new Lazy<bool>(IsFlagsEnum);

        /// <summary>
        /// Gets all unique values as a dictionary.
        /// </summary>
        /// <returns>The <see cref="Dictionary{int, T}"/></returns>
        private static IReadOnlyDictionary<int, T> GetAllUniqueOptionsAsDictionary() => _allOptions.Value
                .ToDictionary(o => o.Value);

        /// <summary>
        /// Gets all options as a dictionary with name as they key.
        /// </summary>
        /// <returns>The <see cref="Dictionary{string, T}"/></returns>
        private static IReadOnlyDictionary<string, T> GetOptionsAsNameDictionary() => _allOptions.Value
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
        /// Gets the options sorted by value desc.
        /// </summary>
        /// <returns></returns>
        private static KeyValuePair<int, T>[] GetOptionsSortedByValueDesc() => _optionsDictionary.Value
            .OrderByDescending(o => o.Key).ToArray();

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
        public static List<T> AllOptions => _allOptions.Value;

        /// <summary>
        /// Defines the DefaultValue
        /// </summary>
        public static T DefaultValue = _optionsDictionary.Value.GetValueOrDefault(0);

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
        /// <param name="name">The name<see cref="string" /></param>
        /// <param name="val">The val<see cref="int" /></param>
        protected SharpEnum(string name, int val)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Enum values label cannot be null or whitespace.", nameof(name));
            }

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
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T FromValue(int value) => FromValue(value, false);

        /// <summary>
        /// Tries to create enum from int value. Returns true if successful, false if not.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="output">The output.</param>
        /// <returns></returns>
        public static bool TryFromValue(int value, out T output)
        {
            output = FromValue(value, true);
            return output.Value == value;
        }

        /// <summary>
        /// Creates enum from name string value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="caseInsensitive"></param>
        /// <returns></returns>
        public static T Parse(string name, bool caseInsensitive = true) => Parse(name, caseInsensitive, false);

        /// <summary>
        /// Tries to create enum from name string value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="output">The output.</param>
        /// <param name="caseInsensitive">if set to <c>true</c> [case insensitive].</param>
        /// <returns></returns>
        public static bool TryParse(string name, out T output, bool caseInsensitive = true)
        {
            output = Parse(name, caseInsensitive, true);
            return output.Value != DefaultValue.Value || output.Name.Equals(name, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
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
            if (_optionsDictionary.Value.ContainsKey(combinedValue))
            {
                return _optionsDictionary.Value[combinedValue];
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
        /// Creates enum from int value. Will return DefaultValue rather than throw if safe true.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="safe">if set to <c>true</c> [safe].</param>
        /// <returns></returns>
        private static T FromValue(int value, bool safe)
        {
            // Looks for exact match for flag and non-flag enums.
            if (_optionsDictionary.Value.ContainsKey(value))
            {
                return _optionsDictionary.Value[value];
            }

            // Non flag enums should have an exact match.
            // Throw error here because invalid input for non-flag enum.
            if (!_isFlagsEnum.Value)
            {
                if (safe)
                {
                    return DefaultValue;
                }

                throw new ArgumentOutOfRangeException(nameof(value), "Invalid value without SharpFlagsEnum attribute");
            }

            if (value <= 0)
            {
                if (safe || value == 0)
                {
                    return DefaultValue;
                }

                throw new ArgumentOutOfRangeException(nameof(value), "Negative values not allowed for SharpFlagsEnum");
            }

            // Build flag enum.
            // No exact match found, so must iterate through values and detect flags set.
            var names = new List<string>();
            var outputValue = 0;
            foreach (var option in _allOptionsSortedByValueDesc.Value.Where(o => o.Key <= value))
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
                if (safe)
                {
                    return DefaultValue;
                }

                throw new ArgumentOutOfRangeException(nameof(value), "Unmatched input value for enum.");
            }

            // Reverse sort if necessary.
            if (names.Count > 1)
            {
                names.Reverse();
            }

            return (T)Activator.CreateInstance(typeof(T), string.Join(StringSeperator, names), outputValue);
        }

        /// <summary>
        /// Parses the specified name into an enum.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caseInsensitive">if set to <c>true</c> [case insensitive].</param>
        /// <param name="safe">if set to <c>true</c> [safe].</param>
        /// <returns></returns>
        private static T Parse(string name, bool caseInsensitive, bool safe)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (safe)
                {
                    return DefaultValue;
                }

                throw new ArgumentNullException(nameof(name), "Input name is null or empty.");
            }

            var names = name.Split(CharSeperatorAsArray, StringSplitOptions.RemoveEmptyEntries);

            // Non-flag enums should not have multiple values being parsed.
            if (names.Length > 1 && !_isFlagsEnum.Value)
            {
                if (safe)
                {
                    return DefaultValue;
                }

                throw new ArgumentException(nameof(name), "Non-flag enums cannot have multiple values.");
            }

            // Find all the matching values by string comparison using either the dictionary or the list.
            var matchedOptions = new List<T>();
            foreach (var segment in names)
            {
                var token = segment.Trim();

                if (!_optionsNameDictionary.Value.TryGetValue(token, out var match) && caseInsensitive)
                {
                    match = _allOptions.Value.Find(o => o.Name.Equals(token, StringComparison.OrdinalIgnoreCase));
                }

                if (null != match)
                {
                    matchedOptions.Add(match);
                }
            }

            if (matchedOptions.Count == 0)
            {
                if (safe)
                {
                    return DefaultValue;
                }

                throw new ArgumentException(nameof(name), "Input name does not match any enum values.");
            }

            // Get the value of the desired SmartEnum by getting the total value of all matched options.
            var outputValue = matchedOptions.Sum(m => m.Value);

            // Use from value to actually get the output.
            // A bit of a work around because there is no good way to use the OR operator within this function.
            return FromValue(outputValue);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>The <see cref="bool"/></returns>
        public override bool Equals(object obj) => this.Equals(obj as SharpEnum<T>);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Equals(SharpEnum<T> other) => other != null
                   && this.Name == other.Name
                   && this.Value == other.Value;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override int GetHashCode()
        {
            var hashCode = -244751520;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name);
            hashCode = hashCode * -1521134295 + this.Value.GetHashCode();
            return hashCode;
        }
    }
}
