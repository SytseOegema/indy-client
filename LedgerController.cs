using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.LedgerApi;


namespace indyClient
{
    class LedgerController
    {
        private PoolController d_poolController;
        private WalletController d_walletController;

        public LedgerController(ref PoolController poolController,
            ref WalletController walletController)
        {
            d_poolController = poolController;
            d_walletController = walletController;
        }

        public async Task sendNymRequest(string trusteeName, string trusteeDid,
            string did, string verkey ,string alias, string role)
        {
            try
            {
                // build nym request for owner of did
                var nymJson = await Ledger.BuildNymRequestAsync(trusteeDid, did,
                    verkey ,alias, role);

                // open trustee wallet
                string originalIdentifier =
                    d_walletController.getIdentifier();
                await d_walletController.open(trusteeName);

                // Trustee sends nym request
                var nymResponseJson = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
                    trusteeDid,
                    nymJson);

                d_walletController.open(originalIdentifier);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
