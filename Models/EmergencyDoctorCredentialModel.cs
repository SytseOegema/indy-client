using System;
using Newtonsoft.Json;

namespace indyClient
{
    public class EmergencyDoctorCredentialModel
    {
        public string issuer_did;
        public string schema_id;
        public string schema_json;
        public string cred_def_id;
        public string cred_def_json;

        public EmergencyDoctorCredentialModel(string issuerDid,
            string schemaId, string schemaJson,
            string credDefId, string credDefJson)
        {
            issuer_did = issuerDid;
            schema_id = schemaId;
            schema_json = schemaJson;
            cred_def_id = credDefId;
            cred_def_json = credDefJson;
        }

        static public string filePath()
        {
            return $"env/emergency_doctor_credential.json";
        }

        static public EmergencyDoctorCredentialModel importFromJsonFile()
        {
            string importJson = IOFacilitator.readFile(
                EmergencyDoctorCredentialModel.filePath());

            return JsonConvert.DeserializeObject
                <EmergencyDoctorCredentialModel>(importJson);
        }

        public void exportToJsonFile()
        {
            IOFacilitator.createFile(toJson(), filePath());
        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
