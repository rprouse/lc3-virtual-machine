using LC3;
using FluentAssertions;
using NUnit.Framework;

namespace lc3.tests
{
    public class VirtualMachineTest
    {
        VirtualMachine _vm;

        [SetUp]
        public void SetUp()
        {
            _vm = new VirtualMachine();
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
            _vm.Add(instr);
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
            _vm.Add(instr);
            _vm.Registers[VirtualMachine.R2].Should().Be((ushort)expected);
        }
    }
}