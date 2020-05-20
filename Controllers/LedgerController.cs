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
                    issuerDid, schema.SchemaJson
                );

                await d_walletController.open(issuerName);

                // publish schema to ledger
                var ledgerJSON = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
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

        public async Task createCredDef(string name, string version,
            string attributes, string issuerDid, string issuerName,
            string trusteeDid, string trusteeName)
        {
            var schemaJson = await createSchema(name, version, attributes,
            trusteeDid, trusteeName);

            d_walletController.open(issuerName);

            string credDefConfigJson = "{\"support_revocation\":false}";

            var res = await AnonCreds.IssuerCreateAndStoreCredentialDefAsync(
                d_walletController.getOpenWallet(),
                issuerDid,
                schemaJson,
                "Tag1",
                "CL",
                credDefConfigJson);

            // var credDefId = createCredDefResult.CredDefId;
            // var credDefJson = createCredDefResult.CredDefJson;
            Console.WriteLine(res);
        }

        public async Task createSchemaCLI()
        {
            Console.WriteLine("Name of credential owner:");
            string issuerName = Console.ReadLine();
            Console.WriteLine("Name of the schema:");
            string name = Console.ReadLine();
            Console.WriteLine("Version of the schema: (x.x.x)");
            string version = Console.ReadLine();
            Console.WriteLine("Attributes of the schema: [\"name\", \"age\"]");
            string attributes = Console.ReadLine();
            Console.WriteLine("did of the issuer: ");
            string trusteeDid = Console.ReadLine();
            if (trusteeDid == "")
            {
              Console.WriteLine("no did of the issuer specified.");
              Console.WriteLine("Did of Steward1 will be used.");
              trusteeDid = "Th7MpTaRZVRYnPiabds81Y";
            }
            Console.WriteLine("name of the controller(steward): ");
            string trusteeName = Console.ReadLine();
            Console.WriteLine("name of the issuer: ");
            string issuerName = Console.ReadLine();

            d_walletController.open(issuerName);
            var didListJson = await d_didController.list();
            var issuerDid = JArray.Parse(didListJson)[0]["did"].ToString();

            var res = await createSchema(name, version, attributes,
                issuerDid, issuerName, trusteeDid, trusteeName);
            Console.WriteLine(res);
        }

    }
}
