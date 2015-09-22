using System;

namespace TypeAlias
{
    // When alias is specified explicitly, the alias will be used.
    // When alias left empty, the alias will be calculated by 31-bits djb2 hash algorithm.
    // The range of hash value is [0x100, 0x7FFFFFFF]

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TypeAliasAttribute : Attribute
    {
        public int Alias { get; private set; }

        public TypeAliasAttribute()
        {
        }

        public TypeAliasAttribute(int alias)
        {
            Alias = alias;
        }
    }
}
