using System.Runtime.CompilerServices;

// For unit tests
[assembly:InternalsVisibleTo("lc3.tests")]

namespace LC3
{
    class Program
    {
        static int Main(string[] args)
        {
            var vm = new VirtualMachine();
            var result = vm.Load(args);
            if (result != 0)
                return result;

            vm.Setup();
            vm.Run();

            return 0;
        }
    }
}
