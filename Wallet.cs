using System;
using Newtonsoft.Json;


namespace indyClient
{
    class Wallet
    {
        private WalletImportConfig importConfig;

        public Wallet()
        {
            importConfig = new WalletImportConfig(
                "sandbox",
                "Steward1",
                "default",
                "Steward1",
                "/home/hyper/wallets/steward_wallet",
                "test");
        }

        public void test()
        {
            Console.WriteLine(importConfig.toJson());
        }
    }
}
