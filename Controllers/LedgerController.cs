using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using Hyperledger.Indy.LedgerApi;


namespace indyClient
{
    class LedgerController
    {
        private PoolController d_poolController;
        private DidController d_didController;
        private WalletController d_walletController;

        public LedgerController(ref PoolController poolController,
            ref DidController didController
            ref WalletController walletController)
        {
            d_poolController = poolController;
            d_didController = didController;
            d_walletController = walletController;
        }

        public async Task sendNymRequest(string trusteeName, string did,
            string verkey ,string alias, string role)
        {
            try
            {
                // open trustee wallet
                string originalIdentifier =
                d_walletController.getIdentifier();
                await d_walletController.open(trusteeName);

                var didListJson = await d_didController.list();
                var trusteeDid = JArray.Parse(didListJson)[0]["did"].ToString();

                // build nym request for owner of did
                var nymJson = await Ledger.BuildNymRequestAsync(trusteeDid, did,
                    verkey ,alias, role);

                // Trustee sends nym request
                var nymResponseJson = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
                    trusteeDid,
                    nymJson);

                await d_walletController.open(originalIdentifier);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
