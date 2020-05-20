using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.AnonCredsApi;


namespace indyClient
{
    class LedgerController
    {
        private PoolController d_poolController;
        private DidController d_didController;
        private WalletController d_walletController;


        public LedgerController(ref PoolController poolController,
            ref DidController didController,
            ref WalletController walletController)
        {
            d_poolController = poolController;
            d_didController = didController;
            d_walletController = walletController;
        }

        public async Task sendNymRequest(string trusteeName, string did,
            string verkey ,string alias, string role)
        {
            try
            {
                // open trustee wallet
                string originalIdentifier =
                d_walletController.getIdentifier();
                await d_walletController.open(trusteeName);

                var didListJson = await d_didController.list();
                var trusteeDid = JArray.Parse(didListJson)[0]["did"].ToString();

                // build nym request for owner of did
                var nymJson = await Ledger.BuildNymRequestAsync(trusteeDid, did,
                    verkey ,alias, role);

                // Trustee sends nym request
                var nymResponseJson = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
                    trusteeDid,
                    nymJson);

                await d_walletController.open(originalIdentifier);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task<string> createSchema(string name, string version,
            string attributes, string issuerDid, string issuerName)
        {
            try
            {
                // issuer schema
                var schema = await AnonCreds.IssuerCreateSchemaAsync(
                    issuerDid, name, version, attributes);

                // build schema
                var buildschema = await Ledger.BuildSchemaRequestAsync(
                    issuerDid, schema.schemaJson
                )

                WalletController issuerWal = new WalletController();
                await issuerWal.open(issuerName);

                // publish schema to ledger
                var ledgerJSON = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    issuerWal.getOpenWallet(),
                    issuerDid,
                    buildschema);

                return  JsonConvert.SerializeObject(ledgerJSON);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return e.Message;
            }
        }

        public async Task createSchemaCLI()
        {
            Console.WriteLine("Name of the schema:");
            string name = Console.ReadLine();
            Console.WriteLine("Version of the schema: (x.x.x)");
            string version = Console.ReadLine();
            Console.WriteLine("Attributes of the schema: [\"name\", \"age\"]");
            string attributes = Console.ReadLine();
            Console.WriteLine("did of the issuer: ");
            string issuerDid = Console.ReadLine();
            if (issuerDid == "")
            {
              Console.WriteLine("no did of the issuer specified.");
              Console.WriteLine("Did of Steward1 will be used.");
              issuerDid = "Th7MpTaRZVRYnPiabds81Y";
            }
            Console.WriteLine("name of the issuer: ");
            string issuerName = Console.ReadLine();

            var res = await createSchema(name, version, attributes,
                issuerDid, issuerName);
            Console.WriteLine(res);
        }

    }
}
