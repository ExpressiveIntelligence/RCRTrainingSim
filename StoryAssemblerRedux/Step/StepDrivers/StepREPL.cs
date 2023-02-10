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
                Module module = new Module("Run");
                Console.WriteLine(module);

                module.LoadDirectory("\\\\Mac\\code\\retl\\RCR\\StoryAssemblerRedux\\StoryAssembler\\");
                Console.WriteLine("Loaded");

                while (true)
                        {
                            Console.WriteLine("Enter Step code or type 'q' to exit:");
                            string input = Console.ReadLine();
                            if (input.ToLower() == "q")
                            {
                                break;
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
                    Console.ReadLine();
            }
    }

}
