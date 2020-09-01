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
    }
}
