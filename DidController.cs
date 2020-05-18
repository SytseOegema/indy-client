using System;
using System.Threading.Tasks;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;

namespace indyClient
{
    class DidController
    {
        private string d_activeDid;
        private Wallet d_openWallet;

        public DidController()
        {}

        public setOpenWallet(Wallet openWallet)
        {
            d_openWallet = openWallet;
        }

        public async Task list()
        {
            try
            {
                await Did.ListMyDidsWithMetaAsync(d_openWallet);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
