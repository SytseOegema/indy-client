using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            options += "wallet create:: create new wallet.\n";
            options += "wallet open:: open existing wallet.\n";
            options += "wallet close:: close opened wallet.\n";
            options += "wallet list:: list wallets on this device.\n";
            options += "wallet record add:: adds local record to wallet.\n";
            options += "wallet record get:: gets local records from wallet.\n";
            options += "wallet export local:: export wallet to a file on your system.\n";
            options += "wallet export ipfs:: export wallet to IPFS.\n";
            options += "wallet import local:: import wallet from a file on your system.\n";
            options += "wallet import ipfs:: export wallet from IPFS.\n";
            options += "did create:: create new did in opened wallet.\n";
            options += "did activate:: activate a did to use for transactions.\n";
            options += "did list:: list dids in opened wallet.\n";
            options += "ledger send initial nym:: send the initial nym request to create a new identity.\n";
            options += "                       :: new identities can only be created by Trustees ,Stewards and Endorsers.\n";
            options += "schema create:: create a new schema.\n";
            options += "schema list:: list al schema in this wallet.\n";
            options += "schema get:: get a schema.\n";
            options += "credential definition create:: create a credential definition.\n";
            options += "credential definition list:: list all credential definitions in this wallet.\n";
            options += "credential offer create:: issuer creates credential offer.\n";
            options += "credential request create:: prover create credential request.\n";
            options += "credential create:: issuer creates the credential.\n";
            options += "credential store:: prover stores the credential in his wallet.\n";
            options += "credential list:: list all crednetials in open wallet.\n";

            options += "doctor proof request:: shows predefined request for doctor certificate.\n";
            options += "doctor proof create:: creates proof based on the first credential that meets the requiremets.\n";


            options += "EHR environment setup:: creates wallets for Trustee1, Steward1, Steward2\n";
            options += "                     :: creates wallets for Doctor1, Doctor2, Doctor3\n";
            options += "                     :: creates wallet for Gov-Health-Department\n";
            options += "                     :: creates schema and CredDef for Doctor-Certificate\n";
            options += "                     :: creates Doctor-Certificate credential for Doctor{1-3}.\n";
            options += "exit:: quit program.\n";
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



        public List<string> readSharedSecrets()
        {
            List<string> secrets = new List<string>();
            string input = consoleInteraction("The shared secrets(end list with an empty line):");
            while (input != "")
            {
                secrets.Add(input);
                input = Console.ReadLine();
            }
            return secrets;
        }

        public int sharedSecretMinimum()
        {
            string min = consoleInteraction("The minimum number of people required to recover the secret(minimum of 3):");
            int res = stringToIntParser(min);
            if (res < 3)
                throw new InvalidDataException("The number should be bigger than 3.");

            return res;
        }

        public int sharedSecretTotal()
        {
            string tot =  consoleInteraction("The number of people that the secret is shared with:");
            return stringToIntParser(tot);
        }

        public string sharedSecret()
        {
            return consoleInteraction("The shared secret:");
        }

        public string proofJson()
        {
            return consoleInteraction("The proof JSON / the result of proof create:");
        }

        public string proofRequestJson()
        {
            return consoleInteraction("The proof request JSON:");
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

        public string walletIdentifier()
        {
            return consoleInteraction("Name for the import wallet(this name will be used from now on):");
        }

        public string walletMasterKey()
        {
            return consoleInteraction("Wallet master encryption key:");
        }

        public string walletExportKey()
        {
          return consoleInteraction("Wallet export encryption key:");
        }

        public string walletPath()
        {
            return consoleInteraction("Path of the wallet file:");
        }

        public string walletConfigPath()
        {
            return consoleInteraction("Path of the wallet config.json file:");
        }

        public string walletQuery()
        {
            return consoleInteraction("Wallet Query in JSON | {}");
        }

        public string walletOptions()
        {
            return consoleInteraction("Wallet query options in JSON | {\"retrieveTotalCount\": true, \"retrieveType\": true, \"retrieveTags\": true}");
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

        private int stringToIntParser(string input)
        {
            if (!StringFacilitator.IsDigitsOnly(input))
                throw new InvalidDataException("This value may only contain numbers");

            return Int32.Parse(input);
        }
    }
}
