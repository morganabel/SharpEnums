using Palit.SharpEnums.Models;
using Palit.SharpEnums.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SharpEnums.Test.Attributes
{
    public class SharpEnumFlagsAttributesTests
    {
        [Fact]
        public void ItShouldThrowWhenNoFlagsFromFlagValue()
        {
            var invalidValue = 3;

            var exception = Record.Exception(() => NoFlagsEnum.FromValue(invalidValue));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public void ItShouldWorkWhenFlagsEnumFromFlagValue()
        {
            var validValue = 3;

            var redBlackEnum = FlagsEnum.FromValue(validValue);

            Assert.True(redBlackEnum.HasFlag(FlagsEnum.Red));
            Assert.True(redBlackEnum.HasFlag(FlagsEnum.Black));
        }

        [Fact]
        public void ItShouldThrowWhenParseNoFlagsFromFlagString()
        {
            var invalidString = "red, black";

            var exception = Record.Exception(() => NoFlagsEnum.Parse(invalidString));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void ItShouldWorkWhenParseFlagsEnumFromFlagString()
        {
            var validString = "red, black";

            var redBlackEnum = FlagsEnum.Parse(validString);

            Assert.True(redBlackEnum.HasFlag(FlagsEnum.Red));
            Assert.True(redBlackEnum.HasFlag(FlagsEnum.Black));
        }
    }

    internal class NoFlagsEnum : SharpEnum<NoFlagsEnum>
    {
        public static NoFlagsEnum None = new NoFlagsEnum(nameof(None), 0);

        public static NoFlagsEnum Black = new NoFlagsEnum(nameof(Black), 1);

        public static NoFlagsEnum Red = new NoFlagsEnum(nameof(Red), 2);

        public static NoFlagsEnum Green = new NoFlagsEnum(nameof(Green), 4);

        public static NoFlagsEnum Blue = new NoFlagsEnum(nameof(Blue), 8);

        public NoFlagsEnum(string name, int value) : base(name, value) { }
    }

    [SharpEnumFlags]
    internal class FlagsEnum : SharpEnum<FlagsEnum>
    {
        public static FlagsEnum None = new FlagsEnum(nameof(None), 0);

        public static FlagsEnum Black = new FlagsEnum(nameof(Black), 1);

        public static FlagsEnum Red = new FlagsEnum(nameof(Red), 2);

        public static FlagsEnum Green = new FlagsEnum(nameof(Green), 4);

        public static FlagsEnum Blue = new FlagsEnum(nameof(Blue), 8);

        public FlagsEnum(string name, int value) : base(name, value) { }
    }
}
