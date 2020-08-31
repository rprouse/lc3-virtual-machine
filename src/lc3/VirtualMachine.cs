using System;

namespace LC3
{
    public class VirtualMachine
    {
        // 65536 memory locations
        public UInt16[] Memory { get; } = new UInt16[UInt16.MaxValue];

        // 8 general purpose registers (R0-R7)
        // 1 program counter (PC) register
        // 1 condition flags (COND) register
        public UInt16[] Registers { get; } = new UInt16[(int)LC3.Registers.COUNT];
    }
}