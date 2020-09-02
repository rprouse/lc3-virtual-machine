using System;

namespace LC3
{
    public interface IConsole
    {
        bool KeyAvailable { get; }

        int Read();

        ConsoleKeyInfo ReadKey(bool intercept);

        void Write(char value);

        void Write(string value);

        void WriteLine(string value);
    }

    /// <summary>
    /// Wraps System.Console so that it can be mocked out for testing
    /// </summary>
    public class VirtualConsole : IConsole
    {
        public bool KeyAvailable => Console.KeyAvailable;

        public int Read() => Console.Read();

        public ConsoleKeyInfo ReadKey(bool intercept) =>
            Console.ReadKey(intercept);

        public void Write(char value) =>
            Console.Write(value);

        public void Write(string value) =>
            Console.Write(value);

        public void WriteLine(string value) =>
            Console.WriteLine(value);
    }
}
