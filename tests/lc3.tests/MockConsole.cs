using LC3;
using System;
using System.Text;

namespace lc3.tests
{
    public class MockConsole : IConsole
    {
        public const char DefaultKey = 'M';
        public readonly ConsoleKeyInfo DefaultConsoleKeyInfo = new ConsoleKeyInfo(DefaultKey, ConsoleKey.A, true, false, false);

        public bool KeyAvailable => throw new NotImplementedException();

        public bool ReadCalled { get; set; }
        public int Read()
        {
            ReadCalled = true;
            return (int)DefaultKey;
        }

        public bool ReadKeyCalled { get; set; }
        public bool InterceptValue { get; set; }
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            InterceptValue = intercept;
            ReadKeyCalled = true;
            return DefaultConsoleKeyInfo;
        }

        public char WriteCharValue { get; set; }
        public StringBuilder WriteCharBuffer { get; } = new StringBuilder();
        public void Write(char value)
        {
            WriteCharBuffer.Append(value);
            WriteCharValue = value;
        }

        public string WriteStringValue { get; set; }
        public void Write(string value)
        {
            WriteStringValue = value;
        }

        public string WriteLineValue { get; set; }
        public void WriteLine(string value)
        {
            WriteLineValue = value;
        }
    }
}