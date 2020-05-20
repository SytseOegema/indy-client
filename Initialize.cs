using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hyperledger.Indy.PoolApi;


namespace indyClient
{
    class Initialize
    {
        private DidController d_didController;
        private WalletController d_walletController;
        private LedgerController d_ledgerController;

        public Initialize(
        ref DidController didController,
        ref WalletController walletController,
        ref LedgerController ledgerController)
        {
            d_didController = didController;
            d_walletController = walletController;
            d_ledgerController = ledgerController;
        }

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
            var exists = await d_walletController.exists("Trustee1");
            if (exists)
                return;

            DidController did = new DidController();
            WalletController wallet = new WalletController(ref did);

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

        public async Task setupIdentity(string myWalletName,
            string trusteeWalletName, string role)
        {
            await d_walletController.close();

            await d_walletController.create(myWalletName);
            await d_walletController.open(myWalletName);

            d_didController.setOpenWallet(d_walletController.getOpenWallet());
            var didJson = await d_didController.create("");

            var did = JObject.Parse(didJson)["Did"].ToString();
            var verkey = JObject.Parse(didJson)["VerKey"].ToString();

            await d_walletController.open(trusteeWalletName);
            var didListJson = await d_didController.list();

            await d_ledgerController.sendNymRequest(trusteeWalletName,
                did, verkey, "", role);
        }


        public async Task WalletSetupCLI()
        {
            Console.WriteLine("Setup a new wallet with a first did.");
            Console.WriteLine("Name of the new wallet:");
            string name = Console.ReadLine();
            Console.WriteLine("Name of the Trustee that signs the NYM request:");
            string trusteeName = Console.ReadLine();
            Console.WriteLine("The Role of the ID of the new wallet(TRUSTEE, STEWARD, ENDORSER, IDENTITY_OWNER):");
            await setupIdentity(name, trusteeName,
                Console.ReadLine());
        }

    }
}
