using System;

namespace LC3
{
    [Flags]
    public enum ConditionFlags : byte
    {
        POS = 0x1,
        ZRO = 0x2,
        NEG = 0x4
    }
}