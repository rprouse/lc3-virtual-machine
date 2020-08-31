using System;

namespace LC3
{
    public class VirtualMachine
    {
        // Registers
        const byte R0 = 0x0;
        const byte R1 = 0x1;
        const byte R2 = 0x2;
        const byte R3 = 0x3;
        const byte R4 = 0x4;
        const byte R5 = 0x5;
        const byte R6 = 0x6;
        const byte R7 = 0x7;
        const byte PC = 0x8; // Program Counter
        const byte COND = 0x9;
        const byte COUNT = 0xA;

        const UInt16 PC_START = 0x3000;

        // 65536 memory locations
        public UInt16[] Memory { get; } = new UInt16[UInt16.MaxValue];

        // 8 general purpose registers (R0-R7)
        // 1 program counter (PC) register
        // 1 condition flags (COND) register
        public UInt16[] Registers { get; } = new UInt16[COUNT];

        public int Load(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("lc3 [image-file] ...");
                return 2;
            }

            for (int i = 1; i < args.Length; i++)
            {
                if (!ReadImage(args[i]))
                {
                    Console.WriteLine($"Failed to load image: {args[i]}");
                    return 1;
                }
            }
            return 0;
        }

        private bool ReadImage(string filename)
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
                UInt16 instr = Memory[Registers[PC]++];
                Instuctions op = (Instuctions)(instr >> 12);

                switch (op)
                {
                    case Instuctions.ADD:
                        //{ADD, 6}
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
    }
}