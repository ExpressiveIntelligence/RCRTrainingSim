using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Step; // It should work, but if this line gives you errors:
// Need to compile Step to get a .dll https://github.com/ianhorswill/Step
// Add the dependency: For Visual Studio 2019 you may not find Project -> Add Reference option. Use Project -> Add Project Reference. Then in dialog window navigate to Browse tab and use Browse to find and attach your dll.

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
            State state = State.Empty;
            Boolean useState = false;

            while (true)
            {
                String onOff = useState ? "off" : "on";
                Console.WriteLine("Enter Step code or type 'q' to exit, 'r' to reload, 's' to turn " + onOff + " stateful interaaction");
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
                    state = State.Empty;
                }
                else if (input.ToLower() == "s")
                {
                    useState = !useState;
                    state = State.Empty;
                    Console.WriteLine("Saving State: " + useState + ". State reset.");
                }
                else
                {
                    try
                    {
                        if (useState)
                        {
                            state = ExecuteWithState(input, module, state);
                        }
                        else
                        {
                            ExecuteStep(input, module);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        static void ExecuteStep(string code, Module module)
        {
            var result = module.ParseAndExecute(code);
            Console.WriteLine("Result: " + result);
        }

        static State ExecuteWithState(string code, Module module, State state)
        {
            (string result, State newState) = module.ParseAndExecute(code, state);
            Console.WriteLine("Result: " + result);
            return newState;
        }

        static Module LoadModule()
        {
            Module module = null;
            while (module == null)
            {
                try
                {
                    module = new Module("REPL");
                    var dir = "..\\..\\..\\..\\..\\StoryAssembler";
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
