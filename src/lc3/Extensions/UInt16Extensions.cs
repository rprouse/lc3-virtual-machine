using System;

namespace LC3.Extensions
{
    public static class UInt16Extensions
    {
        public static ushort SignExtend(this ushort x, ushort bitCount)
        {
            if(((x >> (bitCount - 1)) & 1) == 0x01)
            {
                x |= (ushort)(0xFFFF << bitCount);
            }
            return x;
        }
    }
}
