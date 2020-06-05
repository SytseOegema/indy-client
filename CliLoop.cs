using System;
using System.Threading.Tasks;

namespace indyClient
{
    public static class CliLoop
    {
        static CliPrompt d_prompt = new CliPrompt();
        static PoolController d_pool = new PoolController();
        static WalletController d_wallet = new WalletController();
        static LedgerController d_ledger = new LedgerController(
            ref d_pool, ref d_wallet);
        static Initialize d_initialize = new Initialize(
            ref d_wallet, ref d_ledger);
        static SetupFacilitator d_setup = new SetupFacilitator(
            ref d_wallet, ref d_ledger);
        static DoctorProofFacilitator d_docProof = new DoctorProofFacilitator(
            ref d_wallet);


        public static async Task start()
        {
            IOFacilitator io = new IOFacilitator();

            if (io.directoryExists(io.getHomePath(), "wallet_export"))
            {
                Console.WriteLine("Welcome back to the indy doctor emergency client!");
            }
            else
            {
                string command = "mkdir " + io.getWalletExportPathAbs();
                ShellFacilitator.Bash(command);
                Console.WriteLine("Welcome to the indy doctor emergency client!");
                Console.WriteLine("You can setup the environment using the command:");
                Console.WriteLine("> EHR environment setup");
                Console.WriteLine("However you have to connect to a pool first using:");
                Console.WriteLine("pool connect");
                Console.WriteLine("> Use the command `help` to list all available commands");
            }
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
                try
                {
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
                            requiredWalletCheck();
                            await d_wallet.listDids();
                            break;
                        case "did create":
                            requiredWalletCheck();
                            await d_wallet.createDid(d_prompt.didSeed(),
                            d_prompt.didMetaDataJson());
                            break;
                        case "did activate":
                            requiredWalletCheck();
                            d_wallet.setActiveDid(d_prompt.myDid());
                            break;
                        case "ledger send initial nym":
                            requiredWalletCheck();
                            requiredDidCheck();
                            requiredPoolCheck();
                            await d_ledger.sendNymRequest(
                            d_prompt.nymDid(),
                            d_prompt.nymVerkey(),
                            d_prompt.nymAlias(),
                            d_prompt.nymRole());
                            break;
                        case "schema create":
                            requiredWalletCheck();
                            requiredDidCheck();
                            requiredPoolCheck();
                            res = await d_ledger.createSchema(
                            d_prompt.schemaName(),
                            d_prompt.schemaVersion(),
                            d_prompt.schemaAttributes());
                            break;
                        case "schema get":
                            requiredWalletCheck();
                            requiredDidCheck();
                            requiredPoolCheck();
                            res = await d_ledger.getSchema(
                            d_prompt.submitterDid(),
                            d_prompt.schemaId());
                            break;
                        case "schema list":
                            requiredWalletCheck();
                            res = await d_wallet.listSchemas();
                            break;
                        case "master secret create":
                            requiredWalletCheck();
                            res = await d_wallet.createMasterSecret(
                            d_prompt.secretId());
                            break;
                        case "credential definition list":
                            requiredWalletCheck();
                            requiredDidCheck();
                            res = await d_wallet.listCredDefs();
                            break;
                        case "credential definition create":
                            requiredWalletCheck();
                            requiredDidCheck();
                            requiredPoolCheck();
                            res = await d_ledger.createCredDef(
                            d_prompt.schemaJson(),
                            d_prompt.credDefTag());
                            break;
                        case "credential offer create":
                            requiredWalletCheck();
                            res = await d_wallet.createCredentialOffer(
                            d_prompt.credDefId());
                            break;
                        case "credential request create":
                            requiredWalletCheck();
                            requiredDidCheck();
                            res = await d_wallet.createCredentialRequest(
                            d_prompt.credOfferJson(),
                            d_prompt.credDefJson(),
                            d_prompt.secretId());
                            break;
                        case "credential create":
                            requiredWalletCheck();
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
                            requiredWalletCheck();
                            res = await d_wallet.storeCredential(
                            d_prompt.credReqMetaJson(),
                            d_prompt.credJson(),
                            d_prompt.credDefJson());
                            break;
                        case "doctor proof request":
                            res = d_docProof.getProofRequest();
                            break;
                        case "doctor proof create":
                            requiredWalletCheck();
                            res = await d_docProof.createDoctorProof();
                            break;


                        case "wallet export local":
                            requiredWalletCheck();
                            res = await d_wallet.walletExportLocal(
                            d_prompt.walletPath(),
                            d_prompt.walletExportKey());
                            break;
                        case "wallet export ipfs":
                            requiredWalletCheck();
                            res = await d_wallet.walletExportIpfs(
                            d_prompt.walletExportKey(),
                            d_prompt.walletMasterKey());
                            break;
                        case "wallet import local":
                            res = await d_wallet.walletImportLocal(
                            d_prompt.walletIdentifier(),
                            d_prompt.walletPath(),
                            d_prompt.walletMasterKey(),
                            d_prompt.walletExportKey());
                            break;
                        case "wallet import ipfs":
                            res = await d_wallet.walletImportIpfs(
                            d_prompt.walletIdentifier(),
                            d_prompt.walletConfigPath());
                            break;
                        case "wallet record add":
                            requiredWalletCheck();
                            res = await d_wallet.addRecord(
                            d_prompt.recordType(),
                            d_prompt.recordId(),
                            d_prompt.recordValue(),
                            d_prompt.recordTagsJson());
                            break;
                        case "wallet get record":
                            requiredWalletCheck();
                            res = await d_wallet.getRecord(
                            d_prompt.recordType(),
                            d_prompt.walletQuery(),
                            d_prompt.walletOptions()
                            );
                            break;
                        case "EHR environment setup":
                            requiredPoolCheck();
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
                }
                catch (Exception e)
                {
                    res = e.Message;
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

        static void requiredWalletCheck()
        {
            if (!d_wallet.isOpen())
                throw new Exception("Attention:   A wallet has to be opened in order to execute this command.");
        }

        static void requiredDidCheck()
        {
            if (!d_wallet.hasActiveDid())
                throw new Exception("Attention:   A DID has to be activated in order to execute this command.");
        }

        static void requiredPoolCheck()
        {
            if (!d_pool.isOpen())
                throw new Exception("Attention:   A connection to a pool is required in order to execute this command.");
        }

    }
}
