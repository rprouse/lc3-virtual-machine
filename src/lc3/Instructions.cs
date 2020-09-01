namespace LC3
{
    public enum Instuctions : byte
    {
        BR = 0x0,     // branch
        ADD = 0x1,    // add
        LD = 0x2,     // load
        ST = 0x3,     // store
        JSR = 0x4,    // jump register
        AND = 0x5,    // bitwise and
        LDR = 0x6,    // load register
        STR = 0x7,    // store register
        RTI = 0x8,    // unused
        NOT = 0x9,    // bitwise not
        LDI = 0xA,    // load indirect
        STI = 0xB,    // store indirect
        JMP = 0xC,    // jump
        RES = 0xD,    // reserved (unused)
        LEA = 0xE,    // load effective address
        TRAP = 0xF    // execute trap
    }
}