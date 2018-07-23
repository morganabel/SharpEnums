using System;

namespace Palit.SharpEnums.Attributes
{
    /// <summary>
    /// Defines the attribute that enables SharpEnums to act as FlagEnums.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SharpEnumFlagsAttribute : Attribute
    {
    }
}
