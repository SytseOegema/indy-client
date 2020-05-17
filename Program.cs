using System;
using System.Threading.Tasks;

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

            try
            {
                await Wallet.ImportAsync(walletConfig, walletCredentails, importConfig);

                // Open the wallet
                using (var stewardWallet = await Wallet.OpenWalletAsync(walletConfig, walletCredentails))
                {


                    // Retrieve stored key
                    var myKeys = await Did.ListMyDidsWithMetaAsync(stewardWallet);

                    // Compare the two keys
                    Console.WriteLine(mykeys);

                    await stewardWallet.CloseAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                //10. Delete wallets and Pool ledger config
                // await WalletUtils.DeleteWalletAsync(walletConfig, walletCredentials);
                // await WalletUtils.DeleteWalletAsync(theirWalletConfig, theirWalletCredentials);
                // await PoolUtils.DeletePoolLedgerConfigAsync(PoolUtils.DEFAULT_POOL_NAME);
            }

        }
    }
}
