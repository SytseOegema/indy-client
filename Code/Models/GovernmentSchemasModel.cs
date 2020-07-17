using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace indyClient
{
    /*
     * This class is used to convert wallet backup information to json.
     * This information can be used to recover a lost wallet.
     */
    public class GovernmentSchemasModel
    {
        public string shared_secret_schema;
        public string emergency_trusted_parties_schema;
        public string electronic_health_record_schema;
        public string doctor_certificate_schema;

        public GovernmentSchemasModel()
        {}

        static public string filePath()
        {
            return $"env/government_schemas.json";
        }

        static public GovernmentSchemasModel importFromJsonFile()
        {
            string importJson = IOFacilitator.readFile(
                GovernmentSchemasModel.filePath());

            return JsonConvert.DeserializeObject
                <GovernmentSchemasModel>(importJson);
        }

        public void exportToJsonFile()
        {
            IOFacilitator.createFile(toJson(), filePath());
        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        static public string getSchemaId(string schema)
        {
            JObject o = JObject.Parse(schema);
            return o["id"].ToString();
        }

        static public string getSchemaAttributes(string schema)
        {
            JObject o = JObject.Parse(schema);
            return o["attrNames"].ToString();
        }
    }
}
