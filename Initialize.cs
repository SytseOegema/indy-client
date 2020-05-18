using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.PoolApi;


namespace indyClient
{
    class Initialize
    {
        public Initialize()
        {}

        public async Task reinitialize()
        {
            Console.WriteLine("reinitialize not implemented yet.");
            try
            {
                Console.WriteLine("reinitialize not implemented yet.");
                await Pool.CreatePoolLedgerConfigAsync("reset", "{\"genesis_txn\":\"\"}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task createGenesisWallets()
        {
            WalletController wallet = new WalletController();
            DidController did = new DidController();

            await wallet.create("Trustee1");
            await wallet.open("Trustee1");
            did.setOpenWallet(wallet.getOpenWallet());
            await did.create("000000000000000000000000Trustee1");

            await wallet.create("Steward1");
            await wallet.open("Steward1");
            did.setOpenWallet(wallet.getOpenWallet());
            await did.create("000000000000000000000000Steward1");

            await wallet.create("Steward2");
            await wallet.open("Steward2");
            did.setOpenWallet(wallet.getOpenWallet());
            await did.create("000000000000000000000000Steward2");
        }

        public async Task setupIdentities()
        {
            WalletController wallet = new WalletController();
            DidController did = new DidController();

            await wallet.create("Anne");
            await wallet.open("Anne");
            did.setOpenWallet(wallet.getOpenWallet());
            await did.create("");

            didList = did.list();
            Console.WriteLine(didList[0].did);

          // public async Task sendNymRequest(string trusteeName, string trusteeDid,
          // string did, string verkey ,string alias, string role)
        }
    }
}
