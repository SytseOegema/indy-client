using System;
using System.Threading.Tasks;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;

namespace indyClient
{
    class DidController
    {
        // private string d_activeDid;
        private Wallet d_openWallet;

        public DidController()
        {}

        public void setOpenWallet(Wallet openWallet)
        {
            d_openWallet = openWallet;
        }

        public async Task<string> create(string seed)
        {
            string didJson;
            if (seed.Length != 0)
            {
                didJson = "{\"seed\": \"" + seed + "\"}";
            }
            else
            {
                didJson = "{}";
            }
            try
            {
                var myDid = await Did.CreateAndStoreMyDidAsync(d_openWallet,
                  didJson);
                Console.WriteLine("did: " + myDid.Did);
                Console.WriteLine("verkey: " + myDid.VerKey);
                return myDid;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return e.Message;
            }
        }

        public async Task list()
        {
            try
            {
                var keys = await Did.ListMyDidsWithMetaAsync(d_openWallet);
                Console.WriteLine(keys);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
