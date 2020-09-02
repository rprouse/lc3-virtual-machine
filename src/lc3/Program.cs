using System.Runtime.CompilerServices;

// For unit tests
[assembly:InternalsVisibleTo("lc3.tests")]

namespace LC3
{
    class Program
    {
        static int Main(string[] args)
        {
            var vm = new VirtualMachine(new VirtualConsole());
            var result = vm.Load(args);
            if (result != 0)
                return result;

            vm.Run();

            return 0;
        }
    }
}
