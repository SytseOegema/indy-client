using System;
using System.Threading.Tasks;

namespace indyClient
{
    public static class CliLoop
    {
        static CliPrompt d_prompt = new CliPrompt();
        static PoolController d_pool = new PoolController("sandbox");
        static WalletController d_wallet = new WalletController();
        static LedgerController d_ledger = new LedgerController(
            ref d_pool, ref d_wallet);
        static Initialize d_initialize = new Initialize(
            ref d_wallet, ref d_ledger);

        public static async Task start()
        {
            Console.WriteLine("Connecting to pool " + d_pool.getIdentifier()
                + ".");
            Console.WriteLine("DIKKE DUISTER NIET VERGETEN DIT WEER AAN TE ZETTEN");
            // await d_pool.connect(d_pool.getIdentifier());

            Console.WriteLine("wallet open");
            var res = await d_wallet.open(
                d_prompt.issuerWalletName());
            Console.WriteLine(res);

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
                        d_prompt.exitMessage();
                        return;
                    case "pool connect":
                        Console.WriteLine("Name of the pool:");
                        await d_pool.connect(Console.ReadLine());
                        break;
                    case "wallet open":
                        var res = await d_wallet.open(
                            d_prompt.issuerWalletName());
                        Console.WriteLine(res);
                        break;
                    case "wallet close":
                        res = await d_wallet.close();
                        Console.WriteLine(res);
                        break;
                    case "did list":
                        await d_wallet.listDids();
                        break;
                    case "did create":
                        await d_wallet.createDid(d_prompt.didSeed(),
                            d_prompt.didMetaDataJson());
                        break;
                    case "did activate":
                        d_wallet.setActiveDid(d_prompt.myDid());
                        break;
                    case "ledger send initial nym":
                        await d_ledger.sendNymRequest(
                            d_prompt.nymDid(),
                            d_prompt.nymVerkey(),
                            d_prompt.nymAlias(),
                            d_prompt.nymRole());
                        break;
                    case "schema create":
                        var  schemaJson = await d_ledger.createSchema(
                            d_prompt.schemaName(),
                            d_prompt.schemaVersion(),
                            d_prompt.schemaAttributes());
                        Console.WriteLine("schema Json:" + schemaJson);
                        break;
                    case "schema get":
                        var schemaJson = await d_ledger.getSchema(
                            d_prompt.submitterDid(),
                            d_prompt.schemaId());
                            Console.WriteLine(schemaJson);
                            break;

                            
                    case "wallet record add":
                        Console.WriteLine(
                            await d_wallet.addRecord(
                                d_prompt.recordType(),
                                d_prompt.recordId(),
                                d_prompt.recordValue(),
                                d_prompt.recordTagsJson()
                            )
                        );
                        break;
                    case "wallet credential get":
                        await d_wallet.getCredentials(
                            d_prompt.walletQuery());
                        break;
                    case "wallet get record":
                        var record = await d_wallet.getRecord(
                            d_prompt.recordType(),
                            d_prompt.walletQuery(),
                            d_prompt.walletOptions()
                        );
                        Console.WriteLine(record);
                        break;
                    // case "wallet setup":
                    //     await d_ledger.initializeWallet(
                    //         d_prompt.issuerWalletName(),
                    //         d_prompt.signerWalletName(),
                    //         d_prompt.issuerRole());
                    //     break;
                    case "wallet create":
                        await d_wallet.create(d_prompt.issuerWalletName());
                        break;
                    case "help":
                        d_prompt.helpOptions();
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
                        d_prompt.inputUnrecognized();
                        break;
                }
            }
        }

        static void setInputLine()
        {
            if (d_pool.isOpen())
                Console.Write(d_pool.getIdentifier() + "|");

            if (d_wallet.isOpen())
                Console.Write(d_wallet.getIdentifier() + "|");

          Console.Write(d_wallet.getActiveDid());

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


    }
}
