using System;
using System.Threading.Tasks;

namespace indyClient
{
    public static class CliLoop
    {
        static IpfsFacilitator d_ipfs = new IpfsFacilitator();
        static CliPrompt d_prompt = new CliPrompt();
        static PoolController d_pool = new PoolController("sandbox");
        static WalletController d_wallet = new WalletController();
        static LedgerController d_ledger = new LedgerController(
            ref d_pool, ref d_wallet);
        static Initialize d_initialize = new Initialize(
            ref d_wallet, ref d_ledger);
        static SetupFacilitator d_setup = new SetupFacilitator(
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
            var res = "";
            while (true)
            {
                setInputLine();
                var input = Console.ReadLine();
                input = input.Trim();
                switch (input)
                {
                    case "exit":
                        d_prompt.exitMessage();
                        return;
                    case "pool connect":
                        await d_pool.connect(d_prompt.poolName());
                        break;
                    case "wallet open":
                        res = await d_wallet.open(
                            d_prompt.issuerWalletName(),
                            d_prompt.walletMasterKey());
                        break;
                    case "wallet create":
                        await d_wallet.create(d_prompt.issuerWalletName());
                        break;
                    case "wallet close":
                        res = await d_wallet.close();
                        break;
                    case "wallet list":
                        IOFacilitator temp = new IOFacilitator();
                        temp.listDirectories("/wallet");
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
                        res = await d_ledger.createSchema(
                            d_prompt.schemaName(),
                            d_prompt.schemaVersion(),
                            d_prompt.schemaAttributes());
                        break;
                    case "schema get":
                        res = await d_ledger.getSchema(
                            d_prompt.submitterDid(),
                            d_prompt.schemaId());
                            break;
                    case "schema list":
                        res = await d_wallet.listSchemas();
                        break;
                    case "master secret create":
                        res = await d_wallet.createMasterSecret(
                            d_prompt.secretId());
                        break;
                    case "credential definition list":
                        res = await d_wallet.listCredDefs();
                        break;
                    case "credential definition create":
                        res = await d_ledger.createCredDef(
                            d_prompt.schemaJson(),
                            d_prompt.credDefTag());
                        break;
                    case "credential offer create":
                        res = await d_wallet.createCredentialOffer(
                            d_prompt.credDefId());
                        break;
                    case "credential request create":
                        res = await d_wallet.createCredentialRequest(
                            d_prompt.credOfferJson(),
                            d_prompt.credDefJson(),
                            d_prompt.secretId());
                        break;
                    case "credential create":
                        CredDefFacilitator credFac =
                            new CredDefFacilitator();

                        res = await d_wallet.createCredential(
                            d_prompt.credOfferJson(),
                            d_prompt.credReqJson(),
                            credFac.generateCredValueJson(
                                d_prompt.schemaAttributes(),
                                d_prompt.credValues())
                            );
                        break;
                    case "credential store":
                        res = await d_wallet.storeCredential(
                            d_prompt.credReqMetaJson(),
                            d_prompt.credJson(),
                            d_prompt.credDefJson());
                        break;

                    case "test":
                        await d_ipfs.test();
                        break;
                    case "test upload":
                        await d_ipfs.addFile(d_prompt.walletPath());
                        break;
                    case "test download":
                        await d_ipfs.getFile(d_prompt.walletPath());




                    case "wallet export":
                        res = await d_wallet.walletExport(
                            d_prompt.walletPath(),
                            d_prompt.walletExportKey());
                        break;
                    case "wallet import":
                        res = await d_wallet.walletImport(
                            d_prompt.walletIdentifier(),
                            d_prompt.walletPath(),
                            d_prompt.walletMasterKey(),
                            d_prompt.walletExportKey());
                        break;
                    case "wallet record add":
                        res = await d_wallet.addRecord(
                            d_prompt.recordType(),
                            d_prompt.recordId(),
                            d_prompt.recordValue(),
                            d_prompt.recordTagsJson());
                        break;
                    case "wallet get record":
                        res = await d_wallet.getRecord(
                            d_prompt.recordType(),
                            d_prompt.walletQuery(),
                            d_prompt.walletOptions()
                        );
                        break;
                    case "EHR environment setup":
                        if(ensurer("Are you sure you want to setup the environment?"))
                            await d_setup.setupEHREnvironment();
                        break;
                    case "help":
                        d_prompt.helpOptions();
                        break;
                    default:
                        d_prompt.inputUnrecognized();
                        break;
                }
                if (res != "")
                    Console.WriteLine(res);

                res = "";
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

        static bool ensurer(string question)
        {
            Console.WriteLine(question);
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
