using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            await wallet.close();
        }

        public async Task setupIdentities(string myWalletName,
            string trusteeWalletName,
            ref DidController didController,
            ref WalletController walletController,
            ref LedgerController ledgerController)
        {
            await walletController.close();

            await walletController.create(walletName);
            await walletController.open(walletName);

            didController.setOpenWallet(wallet.getOpenWallet());
            var didJson = await did.create("");

            var did = JObject.Parse(didJson)["Did"];
            var verkey = JObject.Parse(didJson)["VerKey"];

            await walletController.open(trusteeWalletName);
            var didListJson = await didController.list();

            var trusteeDid = JObject.Parse(didListJson)[0]["did"];
            Console.WriteLine(trusteeDid);

            await ledgerController.sendNymRequest(trusteeWalletName,
                trusteeDid, did, verkey, "", "TRUSTEE");
        }
    }
}
