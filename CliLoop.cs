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
            ref d_pool, ref d_did, ref d_wallet);
        static Initialize d_initialize = new Initialize(
            ref d_did, ref d_wallet, ref d_ledger);


        public static async Task run()
        {
            string input = "pool connect";
            while (true)
            {
                Console.Write("> ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "exit":
                        Console.WriteLine("Exit program!");
                        return;
                    case "pool connect":
                        Console.WriteLine("Name of the pool:");
                        await d_pool.connect(Console.ReadLine());
                        break;
                    case "wallet setup":
                            WalletSetupCLI();
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
                        Console.WriteLine("Seed for did(press ENTER if you dont require a seed):");
                        await d_did.create(Console.ReadLine());
                        break;
                    case "schema create":
                        SchemaCreateCLI();
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

        private void WalletSetupCLI()
        {
          Console.WriteLine("Setup a new wallet with a first did.");
          Console.WriteLine("Name of the new wallet:");
          string name = Console.ReadLine();
          Console.WriteLine("Name of the Trustee that signs the NYM request:");
          string trusteeName = Console.ReadLine();
          Console.WriteLine("The Role of the ID of the new wallet(TRUSTEE, STEWARD, ENDORSER, IDENTITY_OWNER):")
          await d_initialize.setupIdentity(name, trusteeName,
              Console.ReadLine());
        }

        private void SchemaCreateCLI()
        {
            Console.WriteLine("Name of the schema:")
            string name = Console.ReadLine();
            Console.WriteLine("Version of the schema: (x.x.x)")
            string version = Console.ReadLine();
            Console.WriteLine("Attributes of the schema: ["/name/", /"age/"]")
            string attributes = Console.ReadLine();
            Console.WriteLine(name + version + attributes)
        }
    }
}
