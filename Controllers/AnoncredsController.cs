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
                var schema = await AnonCreds.IssuerCreateSchemaAsync(name, version,
                    schemaJson, issuerDid);

                return  JsonConvert.SerializeObject(schema);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return e.Message;
            }
        }

        public void createSchemaCLI()
        {
            Console.WriteLine("Name of the schema:");
            string name = Console.ReadLine();
            Console.WriteLine("Version of the schema: (x.x.x)");
            string version = Console.ReadLine();
            Console.WriteLine("Attributes of the schema: [\\\"name\\\", \\\"age\\\"]");
            string attributes = Console.ReadLine();
            Console.WriteLine(name + version + attributes);
        }


    }
}
