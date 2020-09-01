using System;
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

        // 65536 memory locations
        internal ushort[] Memory { get; } = new ushort[ushort.MaxValue];

        // 8 general purpose registers (R0-R7)
        // 1 program counter (PC) register
        // 1 condition flags (COND) register
        internal ushort[] Registers { get; } = new ushort[COUNT];

        public int Load(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("lc3 [image-file] ...");
                return 2;
            }

            foreach (string arg in args)
            {
                if (!ReadImage(arg))
                {
                    Console.WriteLine($"Failed to load image: {arg}");
                    return 1;
                }
            }
            return 0;
        }

        internal bool ReadImage(string filename)
        {
            return false;
        }

        public void Setup()
        {

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
                        Add(instr);
                        break;
                    case Instuctions.AND:
                        //{AND, 7}
                        break;
                    case Instuctions.NOT:
                        //{ NOT, 7}
                        break;
                    case Instuctions.BR:
                        //{ BR, 7}
                        break;
                    case Instuctions.JMP:
                        //{ JMP, 7}
                        break;
                    case Instuctions.JSR:
                        //{ JSR, 7}
                        break;
                    case Instuctions.LD:
                        //{ LD, 7}
                        break;
                    case Instuctions.LDI:
                        //{ LDI, 6}
                        break;
                    case Instuctions.LDR:
                        //{ LDR, 7}
                        break;
                    case Instuctions.LEA:
                        //{ LEA, 7}
                        break;
                    case Instuctions.ST:
                        //{ ST, 7}
                        break;
                    case Instuctions.STI:
                        //{ STI, 7}
                        break;
                    case Instuctions.STR:
                        //{ STR, 7}
                        break;
                    case Instuctions.TRAP:
                        //{ TRAP, 8}
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
            if(Registers[register] == 0)
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

        internal void Add(ushort instr)
        {
            // Destination register (DR)
            ushort r0 = (ushort)((instr >> 9) & 0x7);

            // First operand (SR1)
            ushort r1 = (ushort)((instr >> 6) & 0x7);

            // Immediate or register mode
            ushort imm_flag = (ushort)((instr >> 5) & 0x1);

            if(imm_flag == 1)
            {
                ushort imm5 = (ushort)(instr & 0x1F);
                ushort x = imm5.SignExtend(5);
                Registers[r0] = (ushort)(Registers[r1] + x);
            }
            else
            {
                ushort r2 = (ushort)(instr & 0x7);
                Registers[r0] = (ushort)(Registers[r1] + Registers[r2]);
            }
            UpdateFlags(r0);
        }
    }
}