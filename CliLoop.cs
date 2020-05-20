using System;
using System.Threading.Tasks;

namespace indyClient
{
    public static class CliLoop
    {
        static CliPrompt d_prompt = new CliPrompt();
        static PoolController d_pool = new PoolController("sandbox");
        static DidController d_did = new DidController();
        static WalletController d_wallet = new WalletController(ref d_did);
        static LedgerController d_ledger = new LedgerController(
            ref d_pool, ref d_did, ref d_wallet);
        static Initialize d_initialize = new Initialize(
            ref d_did, ref d_wallet, ref d_ledger);

        public static async Task start()
        {
            Console.WriteLine("Connecting to pool " + d_pool.getIdentifier()
                + ".");
            Console.WriteLine("DIKKE DUISTER NIET VERGETEN DIT WEER AAN TE ZETTEN");
            // await d_pool.connect(d_pool.getIdentifier());
            await run();
        }

        public static async Task run()
        {
            while (true)
            {
                setInputLine();
                var input = Console.ReadLine();
                switch (input)
                {
                    case "exit":
                        Console.WriteLine("Bye Bye!");
                        return;
                    case "pool connect":
                        Console.WriteLine("Name of the pool:");
                        await d_pool.connect(Console.ReadLine());
                        break;
                    case "wallet setup":
                        await d_ledger.initializeWallet(
                            d_prompt.issuerWalletName(),
                            d_prompt.signerWalletName(),
                            d_prompt.issuerRole()
                        );
                        break;
                    case "wallet create":
                        Console.WriteLine("name of the wallet you would like to create:");
                        await d_wallet.create(Console.ReadLine());
                        break;
                    case "wallet open":
                        Console.WriteLine("name of the wallet you would like to open:");
                        var res = await d_wallet.open(Console.ReadLine());
                        Console.WriteLine(res);
                        break;
                    case "wallet close":
                        res = await d_wallet.close();
                        Console.WriteLine(res);
                        break;
                    case "did list":
                        await d_did.list();
                        break;
                    case "did create":
                        Console.WriteLine("Seed for did(press ENTER if you dont require a seed):");
                        await d_did.create(Console.ReadLine());
                        break;
                    case "schema create":
                        await d_ledger.createSchemaCLI();
                        break;
                    case "help":
                        helpOptions();
                        break;
                    case "reset genesis":
                        Console.WriteLine("Reinitialize genesis transactions?(y/n)");
                        if (ensurer())
                        {
                            await d_initialize.reinitialize();
                        }
                        break;
                    case "create genesis wallets":
                        await d_initialize.createGenesisWallets();
                        break;
                    default:
                        Console.WriteLine("Input is not recognized try 'help' for more info.");
                        break;
                }
            }
        }

        static void setInputLine()
        {
            if (d_pool.isOpen())
                Console.Write(d_pool.getIdentifier() + "|");

            if (d_wallet.isOpen())
                Console.Write(d_wallet.getIdentifier());

          Console.Write("> ");

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

        static void helpOptions()
        {
            string options;
            options = "pool connect: connect to an identity pool.\n";
            options += "wallet setup: publish new wallet with certified did.\n";
            options += "wallet create: ";
            options += "wallet open: ";
            options += "wallet close: ";
            options += "did list: ";
            options += "did create: ";
            options += "create genesis wallets: ";
            options += "reset genesis: ";
            Console.WriteLine(options);
        }
    }
}
