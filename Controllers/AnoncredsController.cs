using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using Hyperledger.Indy.AnonCredsApi;


namespace indyClient
{
    class AnoncredsController
    {
        public AnoncredsController()
        {}

        public async Task<string> createSchema(string name, string version,
            string schemaJson, string issuerDid)
        {
            try
            {
                var schema = await AnonCreds.IssuerCreateSchemaAsync(
                    issuerDid, name, version, schemaJson);

                return  JsonConvert.SerializeObject(schema);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return e.Message;
            }
        }

        public async Task<string> createCredDef()
        {

            var res = AnonCreds.IssuerCreateAndStoreCredentialDefAsync(
                issuerWallet,
                issuerDid,
                schemaJson,
                credDefTag,
                null,
                credDefConfigJson);
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
            var res = await createSchema(name, version, attributes, issuerDid);
            Console.WriteLine(res);
        }




    }
}
