using System;
using System.Threading.Tasks;

namespace indyClient
{
    class SetupFacilitator
    {
        WalletController d_wallet;
        LedgerController d_ledger;
        PoolController d_pool;

        public SetupFacilitator(ref WalletController wallet,
            ref LedgerController ledger, ref PoolController pool)
        {
            d_wallet = wallet;
            d_ledger = ledger;
            d_pool = pool;
        }

        public async Task createERCredentials(string issuer, string issuerDid,
            string poolName)
        {
            await initialize(issuer, issuerDid, poolName);

            Console.WriteLine("creating schema Doctor-Certificate");
            string schemaAttributes = "[\"name\", \"is_emergency_doctor\", \"school\"]";
            string schemaJson = await d_ledger.createSchema(
                "Doctor-Certificate", "1.0.0", schemaAttributes);

            string credDefDefinition = await d_ledger.createCredDef(
                schemaJson, "TAG1");

            Console.WriteLine("Creating Docotor-Certificat Credential for: ");
            string[] doctors = {"Doctor1", "Doctor2", "Doctor3"};
            foreach (string doctor in doctors) {
                Console.WriteLine(doctor);

            }

        }

        private async Task initialize(string issuer, string issuerDid,
            string poolName)
        {
            await d_wallet.open(issuer);
            d_wallet.setActiveDid(issuerDid);
            await d_pool.connect(poolName);
        }

        public async Task createGenesisWallets()
        {
            var exists = await d_wallet.exists("Trustee1");
            if (exists)
            {
                Console.WriteLine("Genesis wallets already exists.");
                return;
            }

            await d_wallet.create("Trustee1");
            await d_wallet.open("Trustee1");
            await d_wallet.createDid("000000000000000000000000Trustee1",
                "{\"purpose\": \"Verinym\"}");

            await d_wallet.create("Steward1");
            await d_wallet.open("Steward1");
            await d_wallet.createDid("000000000000000000000000Steward1",
                "{\"purpose\": \"Verinym\"}");

            await d_wallet.create("Steward2");
            await d_wallet.open("Steward2");
            await d_wallet.createDid("000000000000000000000000Steward2",
                "{\"purpose\": \"Verinym\"}");
            await d_wallet.close();
        }

    }
}
