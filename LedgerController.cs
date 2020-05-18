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
            :
            d_poolController(poolController)
        {}

        public async Task sendNymRequest(string trusteeName, string trusteeDid,
            string did, string verkey ,string alias, string role)
        {
            try
            {
                // build nym request for owner of did
                var nymJson = Ledger.BuildNymRequestAsync(trusteeDid, did,
                    verkey ,alias, role);

                // open trustee wallet
                WalletController wCon = new WalletController();
                wCon.open(trusteeName);

                // Trustee sends nym request
                var nymResponseJson = Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    wCon.getOpenWallet(),
                    trusteeDid,
                    nymJson);
            }
        }
    }
}
