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
            options += "wallet setup:: publish new wallet with certified did.\n";
            options += "wallet create:: create new wallet\n";
            options += "wallet open:: open existing wallet\n";
            options += "wallet close:: close opened wallet\n";
            options += "did list:: list dids in opened wallet\n";
            options += "did create:: create new did in opened wallet\n";
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


        public string nymRole()
        {
            return consoleInteraction("Specify role(TRUSTEE, STEWARD, ENDORER) for the NYM request:");
        }

        public string nymAlias()
        {
            return consoleInteraction("Specify alias for the NYM request:");
        }

        public string nymDid()
        {
            return consoleInteraction("Specify did for the NYM request:");
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
