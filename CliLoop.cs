using System;
using System.Threading.Tasks;

namespace indyClient
{
    public static class CliLoop
    {
        static PoolController d_pool = new PoolController();
        static DidController d_did = new DidController();
        static WalletController d_wallet = new WalletController(ref d_did);
        static LedgerController d_ledger = new LedgerController(
            ref d_pool, ref d_wallet);
        static Initialize d_initialize = new Initialize();


        public static async Task run()
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
                    case "test":
                        await d_initialize.setupIdentities();
                        break;
                    case "pool connect":
                        Console.WriteLine("Name of the pool:");
                        await d_pool.connect(Console.ReadLine());
                        break;
                    case "reset":
                        Console.WriteLine("Reinitialize genesis transactions?(y/n)");
                        if (ensurer())
                        {
                            await d_initialize.reinitialize();
                        }
                        break;
                    case "wallet create":
                        Console.WriteLine("name of the wallet you would like to create:");
                        await d_wallet.create(Console.ReadLine());
                        break;
                    case "wallet open":
                        Console.WriteLine("name of the wallet you would like to open:");
                        await d_wallet.open(Console.ReadLine());
                        break;
                    case "wallet close":
                        await d_wallet.close();
                        d_did.setOpenWallet(null);
                        break;
                    case "did list":
                        await d_did.list();
                        break;
                    case "did create":
                        Console.WriteLine("Do you want to use a seed to create the did?");
                        await d_did.create(Console.ReadLine());
                        break;
                    case "help":
                        Console.WriteLine("The following commands are available:");
                        Console.WriteLine("exit: to exit the program");
                        Console.WriteLine("reset: to reset the genesis transactions and pool configurations");
                        Console.WriteLine("wallet open");
                        break;
                    default:
                        Console.WriteLine("Input is not recognized try 'help' for more info.");
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
