using System;

namespace indyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            cliLoop();
        }

        static async Task cliLoop()
        {
            string input = "";
            while (true)
            {
                Console.Write("> ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "exit":
                        Console.WriteLine("Exit program!");
                        return;
                    case "reset":
                        Console.WriteLine("Reinitialize genesis transactions?(y/n)");
                        if (ensurer())
                        {
                            Reset reset = new Reset();
                            reset.reinitialize();
                        }
                        break;
                    case "help":
                        Console.WriteLine("The following commands are available:");
                        Console.WriteLine("exit: to exit the program");
                        Console.WriteLine("reset: to reset the genesis transactions and pool configurations");
                        break;
                    default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        }

        static bool ensurer()
        {
            string ensurer = "";
            while(true)
            {
                ensurer = Console.ReadLine();
                switch (ensurer)
                {
                    case "y":
                        return true;
                    case "yes":
                        return true;
                    case "n":
                        return false;
                    case "no":
                        return false;
                    default:
                        Console.WriteLine("Specify your choice by typing y/n:");
                        break;
                }
            }
        }
    }
}
