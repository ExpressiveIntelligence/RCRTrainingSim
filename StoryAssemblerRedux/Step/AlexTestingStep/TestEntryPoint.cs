using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Step;

namespace AlexTestingStep
{
    internal class TestEntryPoint
    {
        static void Main(string[] args)
        {
            Module module = new Module("Run");
            Console.WriteLine(module);

            module.LoadDirectory("\\\\Mac\\code\\retl\\RCR\\StoryAssemblerRedux\\StoryAssembler\\");
            Console.WriteLine("Loaded");
            var result = module.Call("RunStoryWrap");
            
            Console.WriteLine("Result");
            Console.WriteLine(result);
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
