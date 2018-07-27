using Palit.SharpEnums.Attributes;
using Palit.SharpEnums.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SharpEnums.Test.Models
{
    public class SharpEnumTests
    {
        [Fact]
        public void ItExists()
        {
            var test = TestSharpEnum.Party;
        }

        [Fact]
        public void ItHasDefaultValue()
        {
            var defaultValue = TestSharpEnum.DefaultValue;

            Assert.Equal(nameof(TestSharpEnum.None), defaultValue.Name);
            Assert.Equal(0, defaultValue.Value);
        }

        [Fact]
        public void ItShouldListAllOptions()
        {
            var allOptions = TestSharpEnum.AllOptions;

            Assert.Equal(9, allOptions.Count);
            Assert.Equal(new List<TestSharpEnum>
            {
                TestSharpEnum.None,
                TestSharpEnum.Party,
                TestSharpEnum.Time,
                TestSharpEnum.Sleepy,
                TestSharpEnum.Hungry,
                TestSharpEnum.PartyTime,
                TestSharpEnum.SleepyTime,
                TestSharpEnum.HungryTime,
                TestSharpEnum.All
            }, allOptions);
        }

        [Fact]
        public void OnFromValueInvalidValueThrowsOutOfRangeException()
        {
            var expectedValue = TestSharpEnum.InvalidValue;

            var exception = Record.Exception(() => TestSharpEnum.FromValue(expectedValue));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1 << 0)]
        [InlineData(1 << 1)]
        [InlineData(1 << 2)]
        [InlineData(3)]
        [InlineData(int.MaxValue)]
        public void ValidFromValuesDoNotThrowError(int value)
        {
            var exception = Record.Exception(() => TestSharpEnum.FromValue(value));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-3)]
        public void FromValueHandlesNegativeValuesWithoutFlags(int value)
        {
            var newEnum = NegativeEnum.FromValue(value);

            Assert.Equal(value, newEnum.Value);
            Assert.NotStrictEqual(NegativeEnum.DefaultValue, newEnum);
        }

        [Theory]
        [ClassData(typeof(SharpEnumFromValueTheoryGenerator))]
        public void FromValueLoadsCorrectEnum(int value, string expectedName, TestSharpEnum hasEnumFlags)
        {
            var newEnum = TestSharpEnum.FromValue(value);

            Assert.Equal(value, newEnum.Value);
            Assert.Equal(expectedName, newEnum.Name);
            Assert.True(newEnum.HasFlag(hasEnumFlags));
        }

        [Fact]
        public void OnFromNameNullThrowsArgumentNullException()
        {
            string enumStringNameNull = null;    

            var nullException = Record.Exception(() => TestSharpEnum.Parse(enumStringNameNull));       

            Assert.NotNull(nullException);
            Assert.IsType<ArgumentNullException>(nullException);
        }

        [Fact]
        public void OnFromNameEmptyThrowsArgumentNullException()
        {
            var enumStringNameEmpty = "";

            var emptyException = Record.Exception(() => TestSharpEnum.Parse(enumStringNameEmpty));

            Assert.NotNull(emptyException);
            Assert.IsType<ArgumentNullException>(emptyException);
        }

        [Fact]
        public void OnFromNameInvalidThrowsArgumentException()
        {
            var invalidName = "InvalidName";

            var exception = Record.Exception(() => TestSharpEnum.Parse(invalidName));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Theory]
        [InlineData("None", "None", 0)]
        [InlineData("Sleepy", "Sleepy", 1 << 2)]
        [InlineData("sleepy", "Sleepy", 1 << 2)]
        [InlineData("PartyTime", "PartyTime", 3)]
        [InlineData("party, time", "PartyTime", 3)]
        [InlineData("Sleepy, party", "Party, Sleepy", (1 << 2) + (1 << 0))]
        [InlineData("Sleepy, party", "Sleepy", 1 << 2, false)]
        public void ValidFromNameLoadsCorrectEnum(string name, string expectedName, int expectedValue, bool caseInsensitive = true)
        {
            var parsedEnum = TestSharpEnum.Parse(name, caseInsensitive);

            Assert.Equal(expectedName, parsedEnum.Name);
            Assert.Equal(expectedValue, parsedEnum.Value);
        }

        [Fact]
        public void ItShouldReturnTrueWhenEqual()
        {
            var a = TestSharpEnum.Party;
            var b = TestSharpEnum.Party;

            Assert.True(a == b);
        }

        [Fact]
        public void ItShouldReturnFalseWhenNotEqual()
        {
            var a = TestSharpEnum.Party;
            var b = TestSharpEnum.Time;

            Assert.False(a == b);
        }

        [Fact]
        public void OrOperatorWorks()
        {
            var combined = TestSharpEnum.Party | TestSharpEnum.Time;

            Assert.Equal(TestSharpEnum.PartyTime.Name, combined.Name);
            Assert.True(combined.HasFlag(TestSharpEnum.Party));
            Assert.True(combined.HasFlag(TestSharpEnum.Time));
            Assert.True(combined.HasFlag(TestSharpEnum.PartyTime));
        }

        [Theory]
        [ClassData(typeof(SharpEnumOrOperatorCorrectNameOrder))]
        public void OrOperatorNameCorrectOrderAndCorrectValue(TestSharpEnum a, TestSharpEnum b, string name)
        {
            var testSharpEnum = a | b;

            Assert.Equal(name, testSharpEnum.Name);
            Assert.Equal(a.Value + b.Value, testSharpEnum.Value);
        }

        [Fact]
        public void XorOperatorWorks()
        {
            var combined = TestSharpEnum.PartyTime ^ TestSharpEnum.Time;

            Assert.Equal(TestSharpEnum.Party.Name, combined.Name);
            Assert.True(combined.HasFlag(TestSharpEnum.Party));
            Assert.False(combined.HasFlag(TestSharpEnum.Time));
        }

        [Fact]
        public void AndOperatorWorks()
        {
            var combined = TestSharpEnum.PartyTime & TestSharpEnum.Time;

            Assert.Equal(TestSharpEnum.Time.Name, combined.Name);
            Assert.True(combined.HasFlag(TestSharpEnum.Time));
            Assert.False(combined.HasFlag(TestSharpEnum.Party));
        }

        [Fact]
        public void ItDoesNotHaveFlag()
        {
            var item = TestSharpEnum.PartyTime;

            var hasFlag = item.HasFlag(TestSharpEnum.Sleepy);

            Assert.False(hasFlag);
        }

        [Fact]
        public void ItHasFlag()
        {
            var item = TestSharpEnum.PartyTime;

            var hasPartyFlag = item.HasFlag(TestSharpEnum.Party);
            var hasTimeFlag = item.HasFlag(TestSharpEnum.Time);
            var hasPartyTimeFlag = item.HasFlag(TestSharpEnum.PartyTime);

            Assert.True(hasPartyFlag);
            Assert.True(hasTimeFlag);
            Assert.True(hasPartyTimeFlag);
        }
    }

    internal class NegativeEnum : SharpEnum<NegativeEnum>
    {
        public static NegativeEnum None = new NegativeEnum(nameof(None), 0);

        public static NegativeEnum One = new NegativeEnum(nameof(One), -1);

        public static NegativeEnum Two = new NegativeEnum(nameof(Two), -2);

        public static NegativeEnum Three = new NegativeEnum(nameof(Three), -3);

        public NegativeEnum(string name, int val) : base(name, val) { }
    }

    [SharpEnumFlags]
    public class TestSharpEnum : SharpEnum<TestSharpEnum>
    {
        /// <summary>
        /// Power of two value that is completely invalid. Will not contain any flags.
        /// </summary>
        public const int InvalidValue = 1 << 6;

        public static TestSharpEnum None = new TestSharpEnum(nameof(None), 0);

        public static TestSharpEnum Party = new TestSharpEnum(nameof(Party), 1 << 0);

        public static TestSharpEnum Time = new TestSharpEnum(nameof(Time), 1 << 1);

        public static TestSharpEnum Sleepy = new TestSharpEnum(nameof(Sleepy), 1 << 2);

        public static TestSharpEnum Hungry = new TestSharpEnum(nameof(Hungry), 1 << 3);

        public static TestSharpEnum PartyTime = new TestSharpEnum(nameof(PartyTime), Party.Value + Time.Value);

        public static TestSharpEnum SleepyTime = new TestSharpEnum(nameof(SleepyTime), Sleepy.Value + Time.Value);

        public static TestSharpEnum HungryTime = new TestSharpEnum(nameof(HungryTime), Hungry.Value + Time.Value);

        public static TestSharpEnum All = new TestSharpEnum(nameof(All), Party.Value + Time.Value + Sleepy.Value + Hungry.Value);

        public TestSharpEnum(string name, int val) : base(name, val) { }
    }

    public class SharpEnumOrOperatorCorrectNameOrder : IEnumerable<object[]>
    {
        private readonly List<object[]> parameters;

        public SharpEnumOrOperatorCorrectNameOrder()
        {
            parameters = new List<object[]>();

            parameters.Add(
                new object[]
                {
                    TestSharpEnum.Party, TestSharpEnum.Time, TestSharpEnum.PartyTime.Name
                }
            );

            parameters.Add(
                new object[]
                {
                    TestSharpEnum.Time, TestSharpEnum.Party, TestSharpEnum.PartyTime.Name
                }
            );

            parameters.Add(
                new object[]
                {
                    TestSharpEnum.Party, TestSharpEnum.Time, TestSharpEnum.PartyTime.Name
                }
            );

            parameters.Add(
                new object[]
                {
                    TestSharpEnum.PartyTime, TestSharpEnum.Sleepy, String.Join(", ", TestSharpEnum.PartyTime, TestSharpEnum.Sleepy)
                }
            );


        }

        public IEnumerator<object[]> GetEnumerator() => ((IEnumerable<object[]>)parameters).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<object[]>)parameters).GetEnumerator();
    }

    public class SharpEnumFromValueTheoryGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> parameters;

        public SharpEnumFromValueTheoryGenerator()
        {
            parameters = new List<object[]>();

            parameters.Add(
                new object[]
                {
                    0, nameof(TestSharpEnum.None), TestSharpEnum.None
                }
            );

            parameters.Add(
                new object[]
                {
                    1 << 0, nameof(TestSharpEnum.Party), TestSharpEnum.Party
                }
            );

            parameters.Add(
                new object[]
                {
                    TestSharpEnum.PartyTime.Value, nameof(TestSharpEnum.PartyTime), TestSharpEnum.PartyTime
                }
            );

            var combined = TestSharpEnum.Party | TestSharpEnum.Time;
            parameters.Add(
                new object[]
                {
                    combined.Value, nameof(TestSharpEnum.PartyTime), TestSharpEnum.PartyTime
                }
            );

            var combinedNotDefined = TestSharpEnum.Sleepy | TestSharpEnum.Time;
            parameters.Add(
                new object[]
                {
                    combinedNotDefined.Value, combinedNotDefined.Name, TestSharpEnum.Sleepy | TestSharpEnum.Time
                }
            );
        }

        public IEnumerator<object[]> GetEnumerator() => parameters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
