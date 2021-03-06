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
        private WalletController d_walletController;


        public LedgerController(ref PoolController poolController,
            ref WalletController walletController)
        {
            d_poolController = poolController;
            d_walletController = walletController;
        }

        public async Task sendNymRequest(string did, string verkey,
            string alias, string role)
        {

            var myDid = d_walletController.getActiveDid();
            try
            {
                // build nym request for owner of did
                var nymJson = await Ledger.BuildNymRequestAsync(myDid, did,
                    verkey ,alias, role);

                // Trustee sends nym request
                var nymResponseJson = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
                    myDid,
                    nymJson);
                Console.WriteLine("Identity published to ledger.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task<string> createSchema(string name, string version,
            string attributes)
        {
            var issuerDid = d_walletController.getActiveDid();
            try
            {
                // issuer schema
                var schema = await AnonCreds.IssuerCreateSchemaAsync(
                    issuerDid, name, version, attributes);

                // build schema
                var buildschema = await Ledger.BuildSchemaRequestAsync(
                    issuerDid, schema.SchemaJson
                );

                // publish schema to ledger
                var ledgerJSON = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
                    issuerDid,
                    buildschema);

                string recordJson = "{";
                recordJson += "\"issuer_did\": \"" + issuerDid + "\",";
                recordJson += "\"schema_id\": \"" + schema.SchemaId + "\",";
                recordJson += "\"schema_name\": \"" + name + "\",";
                recordJson += "\"schema_version\": \"" + version + "\"";
                recordJson += "}";

                // add record to wallet that saves the schema information.
                await d_walletController.addRecord("schema", version,
                schema.SchemaJson, recordJson);

                return schema.SchemaJson;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error create schema: {e.Message}");
                return e.Message;
            }
        }

        public async Task<string> getSchema(string submitterDid,
            string schemaId)
        {
            try
            {
                var build = await Ledger.BuildGetSchemaRequestAsync(
                    submitterDid, schemaId);
                var schema = await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
                    d_walletController.getActiveDid(),
                    build);

                var output = await Ledger.ParseGetSchemaResponseAsync(schema);
                Console.WriteLine(output.Id);
                return output.ObjectJson;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error create schema: {e.Message}");
                return e.Message;
            }

        }

        public async Task<string> createCredDef(string schemaJson, string tag)
        {
            try
            {
                string credDefConfigJson = "{\"support_revocation\":false}";

                var res = await AnonCreds.IssuerCreateAndStoreCredentialDefAsync(
                    d_walletController.getOpenWallet(),
                    d_walletController.getActiveDid(),
                    schemaJson,
                    tag,
                    null,
                    credDefConfigJson);

                await d_walletController.addRecord("creddef", res.CredDefId, res.CredDefJson, "{}");

                var credDefRequest = await Ledger.BuildCredDefRequestAsync(
                    d_walletController.getActiveDid(),
                    res.CredDefJson);

                await Ledger.SignAndSubmitRequestAsync(
                    d_poolController.getOpenPool(),
                    d_walletController.getOpenWallet(),
                    d_walletController.getActiveDid(),
                    credDefRequest);

                return res.CredDefJson;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error make cred def: {e.Message}");
                return e.Message;
            }
        }


        // public async Task initializeWallet(string myWalletName,
        //     string trusteeWalletName, string role)
        // {
        //     await d_walletController.close();
        //
        //     await d_walletController.create(myWalletName);
        //     await d_walletController.open(myWalletName);
        //
        //     var didJson = await d_walletController.createDid("", "{\"purpose\": \"Verinym\"}");
        //
        //     var did = JObject.Parse(didJson)["Did"].ToString();
        //     var verkey = JObject.Parse(didJson)["VerKey"].ToString();
        //
        //     await d_walletController.open(trusteeWalletName);
        //     var didListJson = await d_walletController.listDids();
        //
        //     await sendNymRequest(trusteeWalletName,
        //         did, verkey, "", role);
        //     Console.WriteLine("Identity published to ledger");
        // }

        // public async Task createSchemaCLI()
        // {
        //     Console.WriteLine("Name of the schema:");
        //     string name = Console.ReadLine();
        //     Console.WriteLine("Version of the schema: (x.x.x)");
        //     string version = Console.ReadLine();
        //     Console.WriteLine("Attributes of the schema: [\"name\", \"age\"]");
        //     string attributes = Console.ReadLine();
        //     Console.WriteLine("did of the issuer: ");
        //     string trusteeDid = Console.ReadLine();
        //     if (trusteeDid == "")
        //     {
        //       Console.WriteLine("no did of the issuer specified.");
        //       Console.WriteLine("Did of Steward1 will be used.");
        //       trusteeDid = "Th7MpTaRZVRYnPiabds81Y";
        //     }
        //     Console.WriteLine("name of the controller(steward): ");
        //     string trusteeName = Console.ReadLine();
        //     Console.WriteLine("name of the issuer: ");
        //     string issuerName = Console.ReadLine();
        //
        //     await d_walletController.open(issuerName);
        //     var didListJson = await d_walletController.listDids();
        //     var issuerDid = JArray.Parse(didListJson)[0]["did"].ToString();
        //
        //     await createCredDef(name, version, attributes,
        //         issuerDid, issuerName, trusteeDid, trusteeName);
        //     // Console.WriteLine(res);
        // }

    }
}
