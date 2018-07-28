# SharpEnums
Enums have issues. This library provides a base class `SharpEnum` to create strongly typed alternatives to enums.


## Current Limitations
Keep in mind that there are several limitations to this library right now:
* Enum values must be `int`
* The `,` character is reserved as a seperator

## Quick Start
SharpEnums are defined by inheriting from the SharpEnum abstract class:
```csharp
// Allow Flags
[SharpEnumFlags]
public class TestSharpEnum : SharpEnum<TestSharpEnum>
{
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

// No Flags Allowed
public class TypicalEnum : SharpEnum<TypicalEnum>
{
    public static TypicalEnum None = new TypicalEnum(nameof(None), 0);

    public static TypicalEnum Black = new TypicalEnum(nameof(Black), 1);

    public static TypicalEnum Red = new TypicalEnum(nameof(Red), 2);

    public static TypicalEnum Orange = new TypicalEnum(nameof(Orange), 3);

    public static TypicalEnum Green = new TypicalEnum(nameof(Green), 4);

    public TypicalEnum(string name, int value) : base(name, value) { }
}
```

## Serialization

By default SharpEnums will not serialize as expected with JSON.net. SharpEnums includes 3 different JSON.net custom converters to help with this problem:
* IntSharpEnumConverter
* StringSharpEnumConverter
* StringArraySharpEnumConverter

One way to use these converters is with attributes:
```csharp
// Will serialize to int value i.e. "WhatTimeIsIt": 11
[JsonConverter(typeof(IntSharpEnumConverter<TestSharpEnum>))]
public TestSharpEnum WhatTimeIsIt { get; set; }

// Will serialize to string i.e. "WhatTimeIsIt": "partyTime, hungry"
// Second parameter is optional camel-casing.
[JsonConverter(typeof(StringSharpEnumConverter<TestSharpEnum>), true)]
public TestSharpEnum WhatTimeIsIt { get; set; }

// Will serialize to string array i.e. "WhatTimeIsIt": ["partyTime", "hungry"]
// Second parameter is optional camel-casing.
[JsonConverter(typeof(StringArraySharpEnumConverter<TestSharpEnum>), true)]
public TestSharpEnum WhatTimeIsIt { get; set; }
```

## Safe Deserialization
Each serializer also supports "safe" conversion. By default, invalid values will throw a `JsonSerializationException` on deserialization. By enabling safe conversion, invalid values will be deserialized to the default value rather than throw an error.

Safe deserialization is supported by each converter:
```csharp
// Second parameter is safe conversion boolean.
[JsonConverter(typeof(IntSharpEnumConverter<TestSharpEnum>), true)]
public TestSharpEnum WhatTimeIsIt { get; set; }

// Third parameter is safe conversion boolean.
[JsonConverter(typeof(StringSharpEnumConverter<TestSharpEnum>), true, true)]
public TestSharpEnum WhatTimeIsIt { get; set; }

// Third parameter is safe conversion boolean.
[JsonConverter(typeof(StringArraySharpEnumConverter<TestSharpEnum>), true, true)]
public TestSharpEnum WhatTimeIsIt { get; set; }
```

## Using SharpEnums

### Flags Enums
SharpEnums fully supports flag enums. With normal enums, you would use the `[Flags]` attribute. With SharpEnums, the same behavior works using the `SharpEnumFlags` attribute:
```csharp
[SharpEnumFlags]
public class TestSharpEnum : SharpEnum<TestSharpEnum>
```

### List all options
```csharp
var allOptions = TestSmartEnum.AllOptions;
```

### Default value
```csharp
var defaultValue = TestSmartEnum.DefaultValue;
```

### Has flag
```csharp
if (enumInstance.HasFlag(TestSmartEnum.Party)) {
}
```

### From value
```csharp
var enumInstance = TestSmartEnum.FromValue(11);
```

### Try from value
```csharp
var success = TestSharpEnum.TryFromValue(11, out var enumInstance);
```

### Parse
```csharp
var enumInstance = TestSmartEnum.Parse("partyTime", caseInsensitive: true);
```

### Try parse
```csharp
var success = TestSmartEnum.Parse("partyTime", out var enumInstance, caseInsensitive: true);
```

### Operations
All normal enum operations are supported:
```csharp
var orCombined = TestSharpEnum.Party | TestSharpEnum.Time;

var xorCombined = TestSharpEnum.PartyTime ^ TestSharpEnum.Time;

var andCombined = TestSharpEnum.PartyTime & TestSharpEnum.Time;
```

## License

MIT