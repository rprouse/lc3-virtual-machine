using System;
using System.Collections.Generic;
using LC3.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace lc3.tests.Extensions
{
    public class UInt16ExtensionTests
    {
        [TestCase(0x1F, 5, 65535)]
        [TestCase(0x0F, 5, 15)]
        [TestCase(0xFF, 8, 65535)]
        [TestCase(0xFFFF, 16, 65535)]
        public void TestSignExtend(int x, int byteCount, int expected)
        {
            ushort actual = ((ushort)x).SignExtend((ushort)byteCount);
            actual.Should().Be((ushort)expected);
        }

        [TestCase(0xFFFF, 2, 3)]
        [TestCase(0xFFFF, 8, 255)]
        [TestCase(0xFFFF, 10, 1023)]
        [TestCase(0xFFFF, 16, 65535)]
        [TestCase(0xFFC0, 2, 3)]
        [TestCase(0xFFC0, 8, 255)]
        [TestCase(0xFFC0, 10, 1023)]
        [TestCase(0xFFC0, 16, 65472)]
        public void TestMsb(int x, int bits, int expected)
        {
            ushort actual = ((ushort)x).MSB((ushort)bits);
            actual.Should().Be((ushort)expected);
        }

        [TestCase(0xFFFF, 2, 3)]
        [TestCase(0xFFFF, 8, 255)]
        [TestCase(0xFFFF, 10, 1023)]
        [TestCase(0xFFFF, 16, 65535)]
        [TestCase(0x002A, 2, 2)]
        [TestCase(0x002A, 8, 42)]
        [TestCase(0x002A, 10, 42)]
        [TestCase(0x002A, 16, 42)]
        [TestCase(0x14E1, 5, 1)]
        public void TestLsb(int x, int bits, int expected)
        {
            ushort actual = ((ushort)x).LSB((ushort)bits);
            actual.Should().Be((ushort)expected);
        }

        [TestCase(0x14C4, 8, 6, 3)]
        [TestCase(0x14C4, 11, 9, 2)]
        [TestCase(0x14C4, 2, 0, 4)]
        [TestCase(0x14C4, 2, 2, 1)]
        [TestCase(0x14C4, 15, 12, 1)]
        public void TestBits(int x, int msb, int lsb, int expected)
        {
            ushort actual = ((ushort)x).Bits((ushort)msb, (ushort)lsb);
            actual.Should().Be((ushort)expected);
        }

        [TestCase(0xFF00, 0x00FF)]
        [TestCase(0x00FF, 0xFF00)]
        [TestCase(0x0FF0, 0xF00F)]
        public void TestSwap16(int x, int expected)
        {
            ushort actual = ((ushort)x).Swap16();
            actual.Should().Be((ushort)expected);
        }
    }
}
