using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hyperledger.Indy.PoolApi;


namespace indyClient
{
    class CliPrompt
    {
        public string issuerWalletName()
        {
            Console.WriteLine("Wallet name of the issuer:");
            return Console.ReadLine();
        }

        public string signerWalletName()
        {
            Console.WriteLine("Wallet name of the signer:");
            return Console.ReadLine();
        }

        public string issuerRole()
        {
            Console.WriteLine("What role has the issuer:");
            return Console.ReadLine();
        }
    }
}
