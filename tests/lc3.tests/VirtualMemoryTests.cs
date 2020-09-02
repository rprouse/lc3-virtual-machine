using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using LC3;
using NUnit.Framework;

namespace lc3.tests
{
    public class VirtualMemoryTests
    {
        MockConsole _console;
        VirtualMemory _vm;

        [SetUp]
        public void SetUp()
        {
            _console = new MockConsole();
            _vm = new VirtualMemory(_console);
        }

        [Test]
        public void AccessKeyboardStatusVirtualRegisterGetsKeyWhenAvailable()
        {
            _console.KeyAvailable = true;
            ushort value = _vm[VirtualMemory.KBSR];
            value.Should().Be(1 << 15);
            _vm[VirtualMemory.KBDR].Should().Be(MockConsole.DefaultKey);
        }

        [Test]
        public void AccessKeyboardStatusVirtualRegisterSetsStatusToZeroWhenNotAvailable()
        {
            _console.KeyAvailable = false;
            ushort value = _vm[VirtualMemory.KBSR];
            value.Should().Be(0);
            _vm[VirtualMemory.KBDR].Should().Be(0);
        }
    }
}
