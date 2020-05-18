using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;



namespace indyClient
{
    class WalletController
    {
        private Wallet d_openWallet;

        public WalletController()
        {}

        public async Task open()
        {
            Console.WriteLine("name of the wallet you would like to open:");
            string identifier = Console.ReadLine();

            string walletConfig = "{ \"id\": \"" + identifier + "\" }";
            string walletCredentails = "{ \"key\": \"" + identifier + "\" }";

            using (d_openWallet = await Wallet.OpenWalletAsync(walletConfig, walletCredentails))
            {

                Console.WriteLine("3");

                // Retrieve stored key
                var myKeys = await Did.ListMyDidsWithMetaAsync(d_openWallet);

                Console.WriteLine("4");

                // Compare the two keys
                Console.WriteLine(myKeys);

                await d_openWallet.CloseAsync();
            }
        }

        public async Task import()
        {
            var walletConfig = "{\"id\":\"Steward1test\"}";
            var walletCredentails = "{\"key\":\"Steward1test\"}";
            var importConfig = JsonConvert.SerializeObject(new
                {
                    path = "/home/hyper/wallets/steward_wallet",
                    key = "test"
                });

            try
            {
                Console.WriteLine("1");
                await Wallet.ImportAsync(walletConfig, walletCredentails, importConfig);

                Console.WriteLine("2");


            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

        }
    }
}
