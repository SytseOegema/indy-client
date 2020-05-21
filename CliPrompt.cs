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

        private string consoleInteraction(string definition)
        {
            Console.WriteLine(definition);
            return Console.ReadLine();
        }
    }
}
