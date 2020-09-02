using LC3;
using FluentAssertions;
using NUnit.Framework;
using LC3.Extensions;

namespace lc3.tests
{
    public class VirtualMachineTest
    {
        VirtualMachine _vm;

        [SetUp]
        public void SetUp()
        {
            _vm = new VirtualMachine();
            _vm.Registers[VirtualMachine.PC] = VirtualMachine.PC_START;
        }

        [Test]
        public void Load_WithNoArguments_ReturnsTwo()
        {
            int actual = _vm.Load(new string[0]);
            actual.Should().Be(2);
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
    }
}