using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;



namespace indyClient
{
    class WalletController
    {
        private string d_walletConfig = "{\"id\":\"Steward1test\"}";
        private string d_walletCredentails = "{\"key\":\"Steward1test\"}";

        public WalletController()
        {}

        public async Task open()
        {
          using (var openWallet = await Wallet.OpenWalletAsync(d_walletConfig, d_walletCredentails))
          {

              Console.WriteLine("3");

              // Retrieve stored key
              var myKeys = await Did.ListMyDidsWithMetaAsync(openWallet);

              Console.WriteLine("4");

              // Compare the two keys
              Console.WriteLine(myKeys);

              await openWallet.CloseAsync();
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
