using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;

namespace indyClient
{
    class Program
    {
        static async Task Main(string[] args)
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

                // Open the wallet
                using (var stewardWallet = await Wallet.OpenWalletAsync(walletConfig, walletCredentails))
                {

                    Console.WriteLine("3");

                    // Retrieve stored key
                    var myKeys = await Did.ListMyDidsWithMetaAsync(stewardWallet);

                    Console.WriteLine("4");

                    // Compare the two keys
                    Console.WriteLine(myKeys);

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
