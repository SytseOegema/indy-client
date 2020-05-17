using System;


namespace indyClient
{
    class Wallet123
    {
        private WalletImportConfig importConfig;

        public Wallet123()
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
