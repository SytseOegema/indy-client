using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using Hyperledger.Indy.AnoncredsApi;


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
                var schema = Anoncreds.IssuerCreateSchemaAsync(name, version,
                    schemaJson, issuerDid);

                return  JsonConvert.SerializeObject(schema);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return e.Message;
            }
        }


    }
}
