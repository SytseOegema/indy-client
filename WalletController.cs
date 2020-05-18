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
        private string d_walletCredentials;
        private string d_identifier;
        private Wallet d_openWallet;

        public WalletController()
        {}

        public Wallet getOpenWallet()
        {
            return d_openWallet;
        }

        public async Task create(string identifier)
        {
            d_identifier = identifier;
            setWalletInfo();

            try
            {
                await Wallet.CreateWalletAsync(d_walletConfig, d_walletCredentials);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task open(string identifier)
        {
            close();
            d_identifier = identifier;
            setWalletInfo();

            try
            {
              d_openWallet = await Wallet.OpenWalletAsync(d_walletConfig, d_walletCredentials);

              Console.WriteLine("wallet " + d_identifier + " opened");
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
                Console.WriteLine("wallet " + d_identifier + " closed");
                resetWalletInfo();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private void setWalletInfo()
        {
            d_walletConfig = "{ \"id\": \"" + d_identifier + "\" }";
            d_walletCredentials = "{ \"key\": \"" + d_identifier + "\" }";
        }

        private void resetWalletInfo()
        {
          d_walletConfig = "";
          d_walletCredentials = "";
          d_identifier = "";
        }
    }
}
