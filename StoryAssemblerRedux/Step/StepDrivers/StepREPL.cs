using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Step;

namespace StepDrivers
{
    internal class StepREPL
    {
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
            };

            Module module = LoadModule();
      
            while (true)
            {
                Console.WriteLine("Enter Step code or type 'q' to exit, 'r' to reload:");
                string input = Console.ReadLine();
                if (input == null)
                {
                    // do nothing
                }
                else if (input.ToLower() == "q")
                {
                    break;
                }
                else if (input.ToLower() == "r")
                {
                    module = LoadModule();
                }
                try
                {
                    ExecuteStep(input, module);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

            static void ExecuteStep(string input, Module module)
            {
                var result = module.ParseAndExecute(input);
                Console.WriteLine("Result");
                Console.WriteLine(result);
            }
            
            static Module LoadModule()
            {
                Module module = null;
                while (module == null) {
                    try
                    {
                        module = new Module("REPL");
                        var dir = "\\\\Mac\\code\\retl\\RCR\\StoryAssemblerRedux\\StoryAssembler\\";
                        module.LoadDirectory(dir);
                        Console.WriteLine("Loaded " + dir);
                        return module;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press any character to reload.");
                        Console.ReadLine();
                    }
                }
                return module;
            }
    }
}
