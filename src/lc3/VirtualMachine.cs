using System;
using System.IO;
using LC3.Extensions;

namespace LC3
{
    public class VirtualMachine
    {
        // Registers
        internal const byte R0 = 0x0;
        internal const byte R1 = 0x1;
        internal const byte R2 = 0x2;
        internal const byte R3 = 0x3;
        internal const byte R4 = 0x4;
        internal const byte R5 = 0x5;
        internal const byte R6 = 0x6;
        internal const byte R7 = 0x7;
        internal const byte PC = 0x8; // Program Counter
        internal const byte COND = 0x9;
        internal const byte COUNT = 0xA;

        internal const ushort PC_START = 0x3000;

        private readonly IConsole _console;

        internal VirtualMemory Memory { get; }

        // 8 general purpose registers (R0-R7)
        // 1 program counter (PC) register
        // 1 condition flags (COND) register
        internal ushort[] Registers { get; } = new ushort[COUNT];

        public VirtualMachine(IConsole console)
        {
            _console = console;
            Memory = new VirtualMemory(_console);
        }

        public int Load(string[] args)
        {
            if (args.Length == 0)
            {
                _console.WriteLine("lc3 [image-file] ...");
                return 2;
            }

            foreach (string arg in args)
            {
                if (!ReadImage(arg))
                {
                    _console.WriteLine($"Failed to load image: {arg}");
                    return 1;
                }
            }
            return 0;
        }

        internal bool ReadImage(string filename)
        {
            if (!File.Exists(filename))
            {
                _console.WriteLine($"{filename} does not exist");
                return false;
            }

            using (var reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                try
                {
                    ushort origin = reader.ReadUInt16().Swap16();
                    while (true)
                    {
                        Memory[origin++] = reader.ReadUInt16().Swap16();
                    }
                }
                catch (EndOfStreamException) { }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error: {ex.Message}");
                }
            }
            return true;
        }

        public void Run()
        {
            // set the PC to starting position
            // 0x3000 is the default
            Registers[PC] = PC_START;

            bool running = true;
            while (running)
            {
                // FETCH
                ushort instr = Memory[Registers[PC]++];
                Instuctions op = (Instuctions)(instr >> 12);

                switch (op)
                {
                    case Instuctions.ADD:
                        ADD(instr);
                        break;
                    case Instuctions.AND:
                        AND(instr);
                        break;
                    case Instuctions.NOT:
                        NOT(instr);
                        break;
                    case Instuctions.BR:
                        BR(instr);
                        break;
                    case Instuctions.JMP:
                        JMP(instr);
                        break;
                    case Instuctions.JSR:
                        JSR(instr);
                        break;
                    case Instuctions.LD:
                        LD(instr);
                        break;
                    case Instuctions.LDI:
                        LDI(instr);
                        break;
                    case Instuctions.LDR:
                        LDR(instr);
                        break;
                    case Instuctions.LEA:
                        LEA(instr);
                        break;
                    case Instuctions.ST:
                        ST(instr);
                        break;
                    case Instuctions.STI:
                        STI(instr);
                        break;
                    case Instuctions.STR:
                        STR(instr);
                        break;
                    case Instuctions.TRAP:
                        if (TRAP(instr))
                            running = false;
                        break;
                    case Instuctions.RES:
                    case Instuctions.RTI:
                    default:
                        //{ BAD OPCODE, 7}
                        break;
                }
            }
        }

        internal void UpdateFlags(ushort register)
        {
            if (Registers[register] == 0)
            {
                Registers[COND] = (ushort)ConditionFlags.ZRO;
            }
            else if (Registers[register] >> 15 == 0x1)
            {
                Registers[COND] = (ushort)ConditionFlags.NEG;
            }
            else
            {
                Registers[COND] = (ushort)ConditionFlags.POS;
            }
        }

        internal void ADD(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);

            // First operand (SR1)
            ushort sr1 = instr.Bits(8, 6);

            // Immediate or register mode
            ushort imm_flag = instr.Bits(5, 5);

            if (imm_flag == 1)
            {
                ushort imm5 = instr.LSB(5).SignExtend(5);
                Registers[dr] = (ushort)(Registers[sr1] + imm5);
            }
            else
            {
                ushort sr2 = instr.LSB(3);
                Registers[dr] = (ushort)(Registers[sr1] + Registers[sr2]);
            }
            UpdateFlags(dr);
        }

        internal void AND(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);

            // First operand (SR1)
            ushort sr1 = instr.Bits(8, 6);

            // Immediate or register mode
            ushort imm_flag = instr.Bits(5, 5);

            if (imm_flag == 1)
            {
                ushort imm5 = instr.LSB(5).SignExtend(5);
                Registers[dr] = (ushort)(Registers[sr1] & imm5);
            }
            else
            {
                ushort sr2 = instr.LSB(3);
                Registers[dr] = (ushort)(Registers[sr1] & Registers[sr2]);
            }
            UpdateFlags(dr);
        }

        internal void BR(ushort instr)
        {
            ushort nzp = instr.Bits(11, 9);
            ushort pcOffset9 = instr.LSB(9).SignExtend(9);
            if (nzp == Registers[COND])
            {
                Registers[PC] += pcOffset9;
            }
        }

        // JMP and RET
        internal void JMP(ushort instr)
        {
            ushort baseR = instr.Bits(8, 6);
            Registers[PC] = Registers[baseR];
        }

        // JSR and JSRR
        internal void JSR(ushort instr)
        {
            Registers[R7] = Registers[PC];
            ushort flag = instr.Bits(11, 11);
            if (flag == 0)
            {
                ushort baseR = instr.Bits(8, 6);
                Registers[PC] = Registers[baseR];
            }
            else
            {
                ushort pcOffset11 = instr.LSB(11).SignExtend(11);
                Registers[PC] += pcOffset11;
            }
        }

        internal void LD(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);
            ushort pcOffset9 = instr.LSB(9).SignExtend(9);
            ushort addr = (ushort)(Registers[PC] + pcOffset9);
            Registers[dr] = Memory[addr];
            UpdateFlags(dr);
        }

        internal void LDI(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);
            ushort pcOffset9 = instr.LSB(9).SignExtend(9);
            ushort addr = (ushort)(Registers[PC] + pcOffset9);
            Registers[dr] = Memory[Memory[addr]];
            UpdateFlags(dr);
        }

        internal void LDR(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);
            ushort baseR = instr.Bits(8, 6);
            ushort pcOffset6 = instr.LSB(6).SignExtend(6);
            ushort addr = (ushort)(Registers[baseR] + pcOffset6);
            Registers[dr] = Memory[addr];
            UpdateFlags(dr);
        }

        internal void LEA(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);
            ushort pcOffset9 = instr.LSB(9).SignExtend(9);
            Registers[dr] = (ushort)(Registers[PC] + pcOffset9);
            UpdateFlags(dr);
        }

        internal void NOT(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);
            ushort sr = instr.Bits(8, 6);
            Registers[dr] = (ushort)~Registers[sr];
            UpdateFlags(dr);
        }

        internal void ST(ushort instr)
        {
            ushort sr = instr.Bits(11, 9);
            ushort pcOffset9 = instr.LSB(9).SignExtend(9);
            ushort addr = (ushort)(Registers[PC] + pcOffset9);
            Memory[addr] = Registers[sr];
        }

        internal void STI(ushort instr)
        {
            ushort sr = instr.Bits(11, 9);
            ushort pcOffset9 = instr.LSB(9).SignExtend(9);
            ushort addr = (ushort)(Registers[PC] + pcOffset9);
            Memory[Memory[addr]] = Registers[sr];
        }

        internal void STR(ushort instr)
        {
            ushort sr = instr.Bits(11, 9);
            ushort baseR = instr.Bits(8, 6);
            ushort pcOffset6 = instr.LSB(6).SignExtend(6);
            ushort addr = (ushort)(Registers[baseR] + pcOffset6);
            Memory[addr] = Registers[sr];
        }

        internal bool TRAP(ushort instr)
        {
            TrapVector trap = (TrapVector)instr.LSB(8);
            switch (trap)
            {
                case TrapVector.GETC:
                    TRAP_GETC();
                    break;
                case TrapVector.OUT:
                    TRAP_OUT();
                    break;
                case TrapVector.PUTS:
                    TRAP_PUTS();
                    break;
                case TrapVector.IN:
                    TRAP_IN();
                    break;
                case TrapVector.PUTSP:
                    TRAP_PUTSP();
                    break;
                case TrapVector.HALT:
                    // Halt execution and print a message on the console
                    _console.WriteLine("HALT AND CATCH FIRE");
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Read a single character from the keyboard. The character is not echoed onto the
        /// console. Its ASCII code is copied into R0. The high eight bits of R0 are cleared
        /// </summary>
        private void TRAP_GETC()
        {
            ConsoleKeyInfo key = _console.ReadKey(true);
            Registers[R0] = key.KeyChar;
        }

        /// <summary>
        /// Write a character in R0[7:0] to the console display.
        /// </summary>
        private void TRAP_OUT()
        {
            _console.Write((char)Registers[R0]);
        }

        /// <summary>
        /// Write a string of ASCII characters to the console display. The characters are contained
        /// in consecutive memory locations, one character per memory location, starting with
        /// the address specified in R0. Writing terminates with the occurrence of x0000 in a
        /// memory location
        /// </summary>
        private void TRAP_PUTS()
        {
            ushort ptr = Registers[R0];
            while (Memory[ptr] != 0x0000)
            {
                _console.Write((char)Memory[ptr++]);
            }
        }

        /// <summary>
        /// Print a prompt on the screen and read a single character from the keyboard. The
        /// character is echoed onto the console monitor, and its ASCII code is copied into R0.
        /// The high eight bits of R0 are cleared.
        /// </summary>
        private void TRAP_IN()
        {
            _console.Write("Enter a character: ");
            ConsoleKeyInfo key = _console.ReadKey(false);
            Registers[R0] = key.KeyChar;
        }

        /// <summary>
        /// Write a string of ASCII characters to the console. The characters are contained in
        /// consecutive memory locations, two characters per memory location, starting with the
        /// address specified in R0. The ASCII code contained in bits [7:0] of a memory location
        /// is written to the console first. Then the ASCII code contained in bits [15:8] of that
        /// memory location is written to the console. (A character string consisting of an odd
        /// number of characters to be written will have x00 in bits [15:8] of the memory
        /// location containing the last character to be written.) Writing terminates with the
        /// occurrence of x0000 in a memory location.
        /// </summary>
        private void TRAP_PUTSP()
        {
            ushort ptr = Registers[R0];
            while (Memory[ptr] != 0x0000)
            {
                char c1 = (char)Memory[ptr].LSB(8);
                _console.Write(c1);

                char c2 = (char)Memory[ptr++].MSB(8);
                if (c2 != 0x00)
                    _console.Write(c2);
            }
        }
    }
}