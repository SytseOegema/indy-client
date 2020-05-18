using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;



namespace indyClient
{
    class WalletController
    {
        private string d_walletConfig;
        private string d_walletCredentails;
        private string d_identifier;
        private Wallet d_openWallet;

        public WalletController()
        {}

        public async Task create()
        {
            Console.WriteLine("name of the wallet you would like to create:");
            d_identifier = Console.ReadLine();
            setWalletInfo();

            try
            {
                await WalletUtils.CreateWalletAsync(firstWalletConfig, firstWalletCredentials);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task open()
        {
            Console.WriteLine("name of the wallet you would like to open:");
            d_identifier = Console.ReadLine();
            setWalletInfo(d_identifier);

            try
            {
              d_openWallet = await Wallet.OpenWalletAsync(d_walletConfig, d_walletCredentails);

              // Retrieve stored key
              var myKeys = await Did.ListMyDidsWithMetaAsync(d_openWallet);
              Console.WriteLine("wallet " + d_identifier " opened");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task close()
        {
            try
            {
                await d_openWallet.CloseAsync();
                Console.WriteLine("wallet " + d_identifier " closed");
                resetWalletInfo();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private setWalletInfo()
        {
            d_walletConfig = "{ \"id\": \"" + d_identifier + "\" }";
            d_walletCredentails = "{ \"key\": \"" + d_identifier + "\" }";
        }

        privaet resetWalletInfo()
        {
          d_walletConfig = "";
          d_walletCredentails = "";
          d_identifier = "";
        }
    }
}
