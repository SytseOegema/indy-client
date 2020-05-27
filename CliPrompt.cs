using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hyperledger.Indy.PoolApi;


namespace indyClient
{
    class CliPrompt
    {
        public void helpOptions()
        {
            string options;
            options = "pool connect:: connect to an identity pool.\n";
            options += "wallet create:: create new wallet\n";
            options += "wallet open:: open existing wallet\n";
            options += "wallet close:: close opened wallet\n";
            options += "wallet list:: list wallets on this device\n";
            options += "did create:: create new did in opened wallet\n";
            options += "did activate:: activate a did to use for transactions\n";
            options += "did list:: list dids in opened wallet\n";
            options += "ledger send initial nym:: send the initial nym request to create a new identity.\n";
            options += "                          new identities can only be created by Trustees ,Stewards and Endorsers.\n";
            options += "schema create:: create a new schema\n";
            options += "schema list:: list al schema in this wallet\n";
            options += "schema get:: get a schema\n";
            options += "credential definition create:: create a credential definition\n";
            options += "credential definition list:: list all credential definitions in this wallet\n";



            options += "create genesis wallets:: creates wallets for Trustee1, Steward1, Steward2\n";
            options += "reset genesis:: does not work :)\n";
            options += "exit:: quit program\n";
            Console.WriteLine(options);
        }

        public void exitMessage()
        {
          Console.WriteLine("Bye bye!");
        }

        public void inputUnrecognized()
        {
            Console.WriteLine("The command is not recognized.");
            Console.WriteLine("Try 'help' for a list of available commands.");
        }



        public string secretId()
        {
            return consoleInteraction("The link secret identifier/name:");
        }

        public string credValues()
        {
            return consoleInteraction("The values of the credential: [\"name-value\", \"age-value\"]");
        }

        public string credReqMetaJson()
        {
            return consoleInteraction("The credential request meta JSON:");
        }

        public string credReqJson()
        {
            return consoleInteraction("The credential request JSON:");
        }

        public string credOfferJson()
        {
            return consoleInteraction("The credential offer JSON:");
        }

        public string credDefJson()
        {
            return consoleInteraction("The credential definition JSON:");
        }

        public string credDefId()
        {
            return consoleInteraction("credential definition id:");
        }

        public string credDefTag()
        {
            return consoleInteraction("credential definition tag(TAG1):");
        }

        public string credJson()
        {
            return consoleInteraction("credential json:");
        }

        public string schemaJson()
        {
            return consoleInteraction("The schema Json:");
        }

        public string submitterDid()
        {
            return consoleInteraction("The did of the submitter:");
        }

        public string schemaId()
        {
            return consoleInteraction("The schema id:");
        }

        public string schemaAttributes()
        {
            return consoleInteraction("The attributes of the schema: [\"name\", \"age\"]");
        }

        public string schemaVersion()
        {
            return consoleInteraction("The version of the schema(x.x.x):");
        }

        public string schemaName()
        {
            return consoleInteraction("The name of the schema:");
        }

        public string nymRole()
        {
            return consoleInteraction("Specify role(TRUSTEE, STEWARD, ENDORER) for the NYM request:");
        }

        public string nymAlias()
        {
            return consoleInteraction("Specify alias for the NYM request:");
        }

        public string nymVerkey()
        {
            return consoleInteraction("Specify verkey for the NYM request:");
        }

        public string nymDid()
        {
            return consoleInteraction("Specify did for the NYM request:");
        }

        public string myDid()
        {
            return consoleInteraction("Specify the did you want to use:");
        }

        public string didMetaDataJson()
        {
            return consoleInteraction("Specify did in metaData JSON:");
        }

        public string recordTagsJson()
        {
            return consoleInteraction("Records Tags in JSON:");
        }

        public string recordValue()
        {
            return consoleInteraction("Record Value:");
        }

        public string recordId()
        {
            return consoleInteraction("Record ID:");
        }

        public string recordType()
        {
            return consoleInteraction("Record Type:");
        }

        public string walletQuery()
        {
            return consoleInteraction("Wallet Query in JSON");
        }

        public string walletOptions()
        {
            return consoleInteraction("Wallet query options in JSON");
        }

        public string didSeed()
        {
            return consoleInteraction("Specify did seed(may be empty):");
        }

        public string poolName()
        {
            return consoleInteraction("Name of the pool:");
        }

        public string issuerWalletName()
        {
            return consoleInteraction("Wallet name of the issuer:");
        }

        public string signerWalletName()
        {
            return consoleInteraction("Wallet name of the signer:");
        }

        public string issuerRole()
        {
            return consoleInteraction("What role has the issuer:");
        }


        // public string walletCredentialsJson()
        // {
        //     var input = "{";
        //     Console.WriteLine("Schema id (optional):");
        //     input += "\"schema_id\": \"" + Console.ReadLine() + "\",";
        //     Console.WriteLine("Issuer did (optional):");
        //     input += "\"schema_issuer_did\": \"" + Console.ReadLine() + "\",";
        //     Console.WriteLine("Schema name (optional):");
        //     input += "\"schema_name\": \"" + Console.ReadLine() + "\",";
        //     Console.WriteLine("Schema version (optional):");
        //     input += "\"schema_version\": \"" + Console.ReadLine() + "\",";
        //     Console.WriteLine("Issuer did (optional):");
        //     input += "\"issuer_did\": \"" + Console.ReadLine() + "\",";
        //     Console.WriteLine("Credential definition id (optional):");
        //     input += "\"cred_def_id\": \"" + Console.ReadLine() + "\"";
        //     input += "}";
        //     return input;
        // }

        private string consoleInteraction(string definition)
        {
            Console.WriteLine(definition);
            return Console.ReadLine();
        }
    }
}
