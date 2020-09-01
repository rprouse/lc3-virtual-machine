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
                        TRAP(instr);
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

        internal void ADD(ushort instr)
        {
            // Destination register (DR)
            ushort dr = instr.Bits(11, 9);

            // First operand (SR1)
            ushort sr1 = instr.Bits(8, 6);

            // Immediate or register mode
            ushort imm_flag = instr.Bits(5, 5);

            if(imm_flag == 1)
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
            if(nzp == Registers[COND])
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
            if(flag == 0)
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
            Registers[dr] = Memory[Registers[PC] + pcOffset9];
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
            Registers[dr] = Memory[Registers[baseR] + pcOffset6];
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
        }

        internal void STI(ushort instr)
        {
        }

        internal void STR(ushort instr)
        {
        }

        internal void TRAP(ushort instr)
        {
        }
    }
}