using System;

namespace LC3
{
    public class VirtualMemory
    {
        // Memory mapped registers
        const ushort KBSR = 0xFE00; // keyboard status
        const ushort KBDR = 0xFE02; // keyboard data

        // 65536 memory locations
        ushort[] _memory { get; } = new ushort[ushort.MaxValue];

        public ushort this[int address]
        {
            get
            {
                if(address == KBSR)
                {
                    if(Console.KeyAvailable)
                    {
                        _memory[KBSR] = (1 << 15);
                        _memory[KBDR] = Convert.ToChar(Console.Read());
                    }
                    else
                    {
                        _memory[KBSR] = 0;
                    }
                }
                return _memory[address];
            }
            set { _memory[address] = value; }
        }
    }
}