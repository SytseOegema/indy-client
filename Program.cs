using System;

namespace indyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            cliLoop();
        }

        static void cliLoop()
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
                        if (ensured())
                            Reset.reinitialize();
                        break;
                    case default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        }

        public bool ensurer()
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
                    case default:
                        Console.WriteLine("Specify your choice by typing y/n:");
                        break;
                }
            }
        }
    }
}
