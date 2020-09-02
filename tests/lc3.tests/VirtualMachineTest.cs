using LC3;
using FluentAssertions;
using NUnit.Framework;
using LC3.Extensions;

namespace lc3.tests
{
    public class VirtualMachineTest
    {
        MockConsole _console;
        VirtualMachine _vm;

        const string ASM_DIR = @"..\..\..\..\..\asm\";

        [SetUp]
        public void SetUp()
        {
            _console = new MockConsole();
            _vm = new VirtualMachine(_console);
            _vm.Registers[VirtualMachine.PC] = VirtualMachine.PC_START;
        }

        [Test]
        public void Load_WithNoArguments_ReturnsTwo()
        {
            int actual = _vm.Load(new string[0]);
            actual.Should().Be(2);
        }

        [Test]
        public void Load_WithNoArguments_WritesToTheConsole()
        {
            int actual = _vm.Load(new string[0]);
            _console.WriteLineValue.Should().StartWith("lc3");
        }

        [TestCase(0, ConditionFlags.ZRO)]
        [TestCase(1, ConditionFlags.POS)]
        [TestCase(65535, ConditionFlags.NEG)]
        public void UpdateFlags_SetsCondRegister(int value, ConditionFlags expected)
        {
            _vm.Registers[VirtualMachine.R1] = (ushort)value;
            _vm.UpdateFlags(VirtualMachine.R1);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(21, 121, 142)]
        [TestCase(-21, 121, 100)]
        [TestCase(21, -121, -100)]
        [TestCase(-21, -121, -142)]
        public void AddRegister(int r3, int r4, int expected)
        {
            // ADD R2, R3, R4 (R2 = R3 + R4)
            ushort instr = 0x14C4;
            _vm.Registers[VirtualMachine.R3] = (ushort)r3;
            _vm.Registers[VirtualMachine.R4] = (ushort)r4;
            _vm.ADD(instr);
            _vm.Registers[VirtualMachine.R2].Should().Be((ushort)expected);
        }

        [TestCase(21, 1, 22)]
        [TestCase(-21, 1, -20)]
        [TestCase(21, 0x1F, 20)]    // 0x1F is the twos compliment -1 for a 5 bit number
        [TestCase(-21, 0x1F, -22)]
        public void AddImmediate(int r3, int imm5, int expected)
        {
            // ADD R2, R3, #imm5 (R2 = R3 + imm5)
            ushort instr = (ushort)(0x14E0 | (ushort)imm5);
            _vm.Registers[VirtualMachine.R3] = (ushort)r3;
            _vm.ADD(instr);
            _vm.Registers[VirtualMachine.R2].Should().Be((ushort)expected);
        }

        [TestCase(1, 1, ConditionFlags.POS)]
        [TestCase(1, -1, ConditionFlags.ZRO)]
        [TestCase(0, -1, ConditionFlags.NEG)]
        [TestCase(1, 2, ConditionFlags.POS)]
        [TestCase(0, -2, ConditionFlags.NEG)]
        public void AddUpdatesConditionRegister(int r3, int r4, ConditionFlags expected)
        {
            // ADD R2, R3, R4 (R2 = R3 + R4)
            ushort instr = 0x14C4;
            _vm.Registers[VirtualMachine.R3] = (ushort)r3;
            _vm.Registers[VirtualMachine.R4] = (ushort)r4;
            _vm.ADD(instr);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(0x55FF, 0xAAAA, 0x00AA)]
        [TestCase(0xFFFF, 0xFFFF, 0xFFFF)]
        public void AndRegister(int r3, int r4, int expected)
        {
            // AND R2, R3, R4 (R2 = R3 && R4)
            ushort instr = 0x54C4;
            _vm.Registers[VirtualMachine.R3] = (ushort)r3;
            _vm.Registers[VirtualMachine.R4] = (ushort)r4;
            _vm.AND(instr);
            _vm.Registers[VirtualMachine.R2].Should().Be((ushort)expected);
        }

        [TestCase(0x55FF, 0x0A, 0x000A)]
        [TestCase(0xFFFF, 0x1F, 0xFFFF)]    // 0x1F is the two's compliment -1
        public void AndImmediate(int r3, int imm5, int expected)
        {
            // AND R2, R3, #imm5 (R2 = R3 && imm5)
            ushort instr = (ushort)(0x54E0 | (ushort)imm5);
            _vm.Registers[VirtualMachine.R3] = (ushort)r3;
            _vm.AND(instr);
            _vm.Registers[VirtualMachine.R2].Should().Be((ushort)expected);
        }

        [TestCase(0x55FF, 0xAAAA, ConditionFlags.POS)]
        [TestCase(0xFF00, 0x00FF, ConditionFlags.ZRO)]
        [TestCase(0xFFFF, 0xFFFF, ConditionFlags.NEG)]
        public void AndUpdatesConditionRegister(int r3, int r4, ConditionFlags expected)
        {
            // AND R2, R3, R4 (R2 = R3 && R4)
            ushort instr = 0x54C4;
            _vm.Registers[VirtualMachine.R3] = (ushort)r3;
            _vm.Registers[VirtualMachine.R4] = (ushort)r4;
            _vm.AND(instr);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(0b_0000_0010_0000_0011, ConditionFlags.POS)]
        [TestCase(0b_0000_0100_0000_0011, ConditionFlags.ZRO)]
        [TestCase(0b_0000_1000_0000_0011, ConditionFlags.NEG)]
        public void BrUpdatesPCOffset(int instr, ConditionFlags flags)
        {
            _vm.Registers[VirtualMachine.COND] = (ushort)flags;
            _vm.BR((ushort)instr);
            _vm.Registers[VirtualMachine.PC].Should().Be(VirtualMachine.PC_START + 0B_0011);
        }

        [Test]
        public void JmpUpdatesPCOffset()
        {
            // JMP R2
            ushort instr = 0B_1100_0000_1000_0000;
            _vm.Registers[VirtualMachine.R2] = 0x7777;
            _vm.JMP(instr);
            _vm.Registers[VirtualMachine.PC].Should().Be(0x7777);
        }

        [Test]
        public void RetUpdatesPCOffset()
        {
            // RET
            ushort instr = 0B_1100_0001_1100_0000;
            _vm.Registers[VirtualMachine.R7] = 0x7777;
            _vm.JMP(instr);
            _vm.Registers[VirtualMachine.PC].Should().Be(0x7777);
        }

        [Test]
        public void JsrSavesTheProgramCounterToRegister7()
        {
            // JSR LABEL
            ushort instr = 0B_0100_1000_0000_0001;
            _vm.JSR(instr);
            _vm.Registers[VirtualMachine.R7].Should().Be(VirtualMachine.PC_START);
        }

        [TestCase(0B_0100_1000_0000_0001, VirtualMachine.PC_START + 1)]  // Positive Jump
        [TestCase(0B_0100_1111_1111_1111, VirtualMachine.PC_START - 1)]  // Negative Jump
        public void JsrUpdatesTheProgramCounter(int instr, int expected)
        {
            _vm.JSR((ushort)instr);
            _vm.Registers[VirtualMachine.PC].Should().Be((ushort)expected);
        }

        [Test]
        public void JsrrUpdatesTheProgramCounter()
        {
            // JSR R3
            ushort instr = 0B_0100_0000_1100_0000;
            _vm.Registers[VirtualMachine.R3] = 0x7777;
            _vm.JSR(instr);
            _vm.Registers[VirtualMachine.PC].Should().Be(0x7777);
        }

        [TestCase(0B_0010_1000_0000_0001, 1)] // Positive Jump (LD R4, #1)
        [TestCase(0B_0010_1001_1111_1111, -1)] // Negative Jump (LD R4, #-1)
        public void LdLoadsFromProgramCounterOffset(int instr, int dir)
        {
            _vm.Memory[VirtualMachine.PC_START + dir] = 0x7777;
            _vm.LD((ushort)instr);
            _vm.Registers[VirtualMachine.R4].Should().Be(0x7777);
        }

        [TestCase(0x0001, ConditionFlags.POS)]
        [TestCase(0x0000, ConditionFlags.ZRO)]
        [TestCase(0xFFFF, ConditionFlags.NEG)]
        public void LdUpdatesConditionRegisters(int addr, ConditionFlags expected)
        {
            ushort instr = 0B_0010_1000_0000_0001;
            _vm.Memory[VirtualMachine.PC_START + 1] = (ushort)addr;
            _vm.LD(instr);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(-1, 0x01FF)]   // Minus one in twos compliment 9-bit number
        [TestCase(0, 0x0000)]
        [TestCase(1, 0x0001)]
        public void LdiLoadFromMemory(int offset, int bOffset)
        {
            // LDI R4, bOffset
            ushort instr = (ushort)(0xA800 | bOffset);
            _vm.Memory[VirtualMachine.PC_START+offset] = 0x7000;
            _vm.Memory[0x7000] = 42;
            _vm.LDI(instr);
            _vm.Registers[VirtualMachine.R4].Should().Be(42);
        }

        [TestCase(-1, ConditionFlags.NEG)]
        [TestCase(0, ConditionFlags.ZRO)]
        [TestCase(1, ConditionFlags.POS)]
        public void LdiUpdatesConditionalRegisters(int x, ConditionFlags expected)
        {
            // LDI R4, 1
            ushort instr = 0xA801;
            _vm.Memory[VirtualMachine.PC_START + 1] = 0x7000;
            _vm.Memory[0x7000] = ((ushort)x).SignExtend(16);
            _vm.LDI(instr);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(0B_0110_1000_1000_0001, 1)] // LDR R4, R2, #1
        [TestCase(0B_0110_1000_1011_1111, -1)] // LDR R4, R2, #-1
        public void LdrLoadsBasePlusOffset(int instr, int dir)
        {
            _vm.Registers[VirtualMachine.R2] = 0x7000;
            _vm.Memory[0x7000 + dir] = 0x7777;
            _vm.LDR((ushort)instr);
            _vm.Registers[VirtualMachine.R4].Should().Be(0x7777);
        }

        [TestCase(0x0001, ConditionFlags.POS)]
        [TestCase(0x0000, ConditionFlags.ZRO)]
        [TestCase(0xFFFF, ConditionFlags.NEG)]
        public void LdrUpdatesConditionRegisters(int value, ConditionFlags expected)
        {
            ushort instr = 0B_0110_1000_1000_0001;
            _vm.Registers[VirtualMachine.R2] = 0x7000;
            _vm.Memory[0x7001] = (ushort)value;
            _vm.LDR(instr);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(0B_1110_1000_0000_0001, VirtualMachine.PC_START + 1)]
        [TestCase(0B_1110_1001_1111_1111, VirtualMachine.PC_START - 1)]
        public void LeaLoadsEffectiveAddress(int instr, int expected)
        {
            _vm.LEA((ushort)instr);
            _vm.Registers[VirtualMachine.R4].Should().Be((ushort)expected);
        }

        [TestCase(0B_1110_1000_0000_0001, ConditionFlags.POS)]
        [TestCase(0B_1110_1000_0000_0000, ConditionFlags.ZRO)]
        [TestCase(0B_1110_1001_1111_1111, ConditionFlags.NEG)]
        public void LeaUpdatesConditionRegisters(int instr, ConditionFlags expected)
        {
            _vm.Registers[VirtualMachine.PC] = 0;
            _vm.LEA((ushort)instr);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(0xFFFF, 0x0000)]
        [TestCase(0x0000, 0xFFFF)]
        [TestCase(0xAA55, 0x55AA)]
        public void NotInvertsTheBits(int r2, int expected)
        {
            // NOT R4, R2
            ushort instr = 0B_1001_1000_1011_1111;
            _vm.Registers[VirtualMachine.R2] = (ushort)r2;
            _vm.NOT(instr);
            _vm.Registers[VirtualMachine.R4].Should().Be((ushort)expected);
        }

        [TestCase(0xFFF0, ConditionFlags.POS)]
        [TestCase(0xFFFF, ConditionFlags.ZRO)]
        [TestCase(0x0000, ConditionFlags.NEG)]
        public void NotUpdatesConditionRegisters(int r2, ConditionFlags expected)
        {
            // NOT R4, R2
            ushort instr = 0B_1001_1000_1011_1111;
            _vm.Registers[VirtualMachine.R2] = (ushort)r2;
            _vm.NOT(instr);
            _vm.Registers[VirtualMachine.COND].Should().Be((ushort)expected);
        }

        [TestCase(0B_0011_1000_0000_0001, 1)]
        [TestCase(0B_0011_1001_1111_1111, -1)]
        public void StStoresToMemory(int instr, int dir)
        {
            _vm.Registers[VirtualMachine.R4] = 0x7777;
            _vm.ST((ushort)instr);
            _vm.Memory[VirtualMachine.PC_START + dir].Should().Be(0x7777);
        }

        [TestCase(0B_1011_1000_0000_0001, 1)]
        [TestCase(0B_1011_1001_1111_1111, -1)]
        public void StiStoresIndirectlyToMemory(int instr, int dir)
        {
            _vm.Memory[(ushort)(VirtualMachine.PC_START + dir)] = 0x8000;
            _vm.Registers[VirtualMachine.R4] = 0x7777;
            _vm.STI((ushort)instr);
            _vm.Memory[0x8000].Should().Be(0x7777);
        }

        [TestCase(0B_0111_1000_1000_0001, 1)]
        [TestCase(0B_0111_1000_1011_1111, -1)]
        public void StrStoresBasePlusOffset(int instr, int dir)
        {
            _vm.Registers[VirtualMachine.R2] = 0x8000;
            _vm.Registers[VirtualMachine.R4] = 0x7777;
            _vm.STR((ushort)instr);
            _vm.Memory[0x8000 + dir].Should().Be(0x7777);
        }

        [Test]
        public void TrapGetCGetsACharacterNotEchoedToTerminal()
        {
            ushort instr = 0xF020;
            bool result = _vm.TRAP(instr);
            result.Should().BeFalse();
            _console.ReadKeyCalled.Should().BeTrue();
            _console.InterceptValue.Should().BeTrue();
            _vm.Registers[VirtualMachine.R0].Should().Be(MockConsole.DefaultKey);
        }

        [Test]
        public void TrapOutWritesACharacterFromRegister0()
        {
            ushort instr = 0xF021;
            _vm.Registers[VirtualMachine.R0] = 'b';
            bool result = _vm.TRAP(instr);
            result.Should().BeFalse();
            _console.WriteCharValue.Should().Be('b');
        }

        [Test]
        public void TrapPutSOutputsAWordString()
        {
            ushort instr = 0xF022;
            ushort addr = 0x7000;
            _vm.Registers[VirtualMachine.R0] = addr;
            _vm.Memory[addr++] = 'H';
            _vm.Memory[addr++] = 'e';
            _vm.Memory[addr++] = 'l';
            _vm.Memory[addr++] = 'l';
            _vm.Memory[addr++] = 'o';
            _vm.Memory[addr++] = ' ';
            _vm.Memory[addr++] = 'W';
            _vm.Memory[addr++] = 'o';
            _vm.Memory[addr++] = 'r';
            _vm.Memory[addr++] = 'l';
            _vm.Memory[addr++] = 'd';
            _vm.Memory[addr++] = 0x00;
            bool result = _vm.TRAP(instr);
            result.Should().BeFalse();
            _console.WriteCharBuffer.ToString().Should().Be("Hello World");
        }

        [Test]
        public void TrapInGetsCharacterEchoedToTerminal()
        {
            ushort instr = 0xF023;
            bool result = _vm.TRAP(instr);
            result.Should().BeFalse();
            _console.ReadKeyCalled.Should().BeTrue();
            _console.InterceptValue.Should().BeFalse();
            _vm.Registers[VirtualMachine.R0].Should().Be(MockConsole.DefaultKey);
        }

        [Test]
        public void TrapPutSPOutputsAnOddLengthByteString()
        {
            ushort instr = 0xF024;
            ushort addr = 0x7000;
            _vm.Registers[VirtualMachine.R0] = addr;
            _vm.Memory[addr++] = 'H' | ('e' << 8);
            _vm.Memory[addr++] = 'l' | ('l' << 8);
            _vm.Memory[addr++] = 'o' | (' ' << 8);
            _vm.Memory[addr++] = 'W' | ('o' << 8);
            _vm.Memory[addr++] = 'r' | ('l' << 8);
            _vm.Memory[addr++] = 'd';
            _vm.Memory[addr++] = 0x00;
            bool result = _vm.TRAP(instr);
            result.Should().BeFalse();
            _console.WriteCharBuffer.ToString().Should().Be("Hello World");
        }

        [Test]
        public void TrapPutSPOutputsAnEvenLengthByteString()
        {
            ushort instr = 0xF024;
            ushort addr = 0x7000;
            _vm.Registers[VirtualMachine.R0] = addr;
            _vm.Memory[addr++] = 'H' | ('e' << 8);
            _vm.Memory[addr++] = 'l' | ('l' << 8);
            _vm.Memory[addr++] = 'o' | (' ' << 8);
            _vm.Memory[addr++] = 'W' | ('o' << 8);
            _vm.Memory[addr++] = 'r' | ('l' << 8);
            _vm.Memory[addr++] = 'd' | ('!' << 8);
            _vm.Memory[addr++] = 0x00;
            bool result = _vm.TRAP(instr);
            result.Should().BeFalse();
            _console.WriteCharBuffer.ToString().Should().Be("Hello World!");
        }

        [Test]
        public void TrapHaltHaltsTheProgram()
        {
            ushort instr = 0xF025;
            bool result = _vm.TRAP(instr);
            result.Should().BeTrue();
            _console.WriteLineValue.Should().Contain("HALT");
        }
    }
}