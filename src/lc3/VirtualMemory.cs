using System;

namespace LC3
{
    public class VirtualMemory
    {
        // Memory mapped registers
        internal const ushort KBSR = 0xFE00; // keyboard status
        internal const ushort KBDR = 0xFE02; // keyboard data

        private readonly IConsole _console;

        // 65536 memory locations
        ushort[] _memory { get; } = new ushort[ushort.MaxValue];

        public VirtualMemory(IConsole console)
        {
            _console = console;
        }

        public ushort this[int address]
        {
            get
            {
                if(address == KBSR)
                {
                    if(_console.KeyAvailable)
                    {
                        _memory[KBSR] = (1 << 15);
                        _memory[KBDR] = Convert.ToChar(_console.Read());
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