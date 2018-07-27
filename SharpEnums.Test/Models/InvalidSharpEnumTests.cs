using Palit.SharpEnums.Models;
using System;
using Xunit;

namespace SharpEnums.Test.Models
{
    public class InvalidSharpEnumTests
    {
        [Fact]
        public void ItShouldThrowOnNullLabel()
        {
            var exception = Record.Exception(() => InvalidLabelEnum.NullLabel);

            Assert.NotNull(exception);
            Assert.IsType<TypeInitializationException>(exception);
        }
    }

    internal class InvalidLabelEnum : SharpEnum<InvalidLabelEnum>
    {
        public static InvalidLabelEnum None = new InvalidLabelEnum(nameof(None), 0);

        public static InvalidLabelEnum NullLabel = new InvalidLabelEnum(null, 0);

        public InvalidLabelEnum(string name, int value) : base(name, value) { }
    }
}
