using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;

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
            while (1)
            {
                Console.ReadLine(input);
                switch (input)
                {
                    case "exit":
                        Console.WriteLine("Exit program!");
                        return;
                    case "reset":
                        Console.WriteLine("Reinitialize genesis transactions?(y/n)")
                        if (ensured())
                            Reset.reinitialize();
                        break;
                    case default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        }

        static bool ensurer()
        {
            string ensurer = "";
            while(1)
            {
                Console.ReadLine(ensurer);
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
