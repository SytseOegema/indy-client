using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.LedgerApi;


namespace indyClient
{
    class LedgerController
    {
        private PoolController d_poolController;

        public LedgerController(PoolController poolController)
        {
            d_poolController = poolController;
        }


        {
            try
            {
                // build nym request for owner of did
                var nymJson = await Ledger.BuildNymRequestAsync(trusteeDid, did,
                    verkey ,alias, role);

                // open trustee wallet
                WalletController wCon = new WalletController();
                await wCon.open(trusteeName);

                // Trustee sends nym request
                var nymResponseJson = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    wCon.getOpenWallet(),
                    trusteeDid,
                    nymJson);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
