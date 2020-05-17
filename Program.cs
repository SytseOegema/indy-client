using System;

using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;

namespace indyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var walletConfig = "{\"id\":\"Steward1\"}";
            var walletCredentails = "{\"key\":\"Steward1\"}";
            var importConfig = JsonConvert.SerializeObject(new
                {
                    path = "/home/hyper/wallets/steward_wallet",
                    key = "Steward1"
                });

            await Wallet.ImportAsync(walletConfig, walletCredentails, importConfig);

            // Open the wallet
            using (var stewardWallet = await Wallet.OpenWalletAsync(walletConfig, walletCredentails))
            {


                // Retrieve stored key
                var myKeys = await Did.ListMyDidsWithMetaAsync(stewardWallet);

                // Compare the two keys
                Console.WriteLine(Mykeys);

                await stewardWallet.CloseAsync();
            }
        }
    }
}
