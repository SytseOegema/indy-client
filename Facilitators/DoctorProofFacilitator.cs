using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.WalletApi;


namespace indyClient
{
    public static class DoctorProofFacilitator
    {

        public static string getProofRequest()
        {
            return File.ReadAllText("Models/DoctorProofRequest.json");
        }

        /*
         * Proofs the holder of the open wallet is a doctor by using the first
         * credential that meets the docotr proof requirements.
         */
        public static async Task<string> createDoctorProof(
            Wallet wallet,
            string masterKey = "doctor-certificate")
        {
            string proofReqJson = getProofRequest();
            proofReqJson = proofReqJson.Replace(" ", string.Empty);
            proofReqJson = proofReqJson.Replace(Environment.NewLine, string.Empty);
            try
            {
                var credList =
                    await AnonCreds.ProverSearchCredentialsForProofRequestAsync(
                        wallet, proofReqJson);

                string attr1Cred = await getCredentialforRequest(
                    credList, "attr1_referent");
                string attr2Cred = await getCredentialforRequest(
                    credList, "attr2_referent");
                string predicate1Cred = await getCredentialforRequest(
                    credList, "predicate1_referent");

                string requestedCreds = proverDoctorRequestCreds(
                    getReferentFromCredential(attr1Cred),
                    getReferentFromCredential(attr2Cred),
                    getReferentFromCredential(predicate1Cred));

                IOFacilitator io = new IOFacilitator();
                DoctorCredDefInfoModel model = JsonConvert.DeserializeObject
                    <DoctorCredDefInfoModel>(File.ReadAllText(
                        io.getDoctorCredDefConfigPathAbs()));
                string schemas = "{";
                schemas += "\"" + model.schema_id + "\":" + model.schema_json;
                schemas += "}";
                string credDefs = "{";
                credDefs += "\"" + model.cred_def_id + "\":" + model.cred_def_json;
                credDefs += "}";

                string res = await AnonCreds.ProverCreateProofAsync(
                    wallet,
                    proofReqJson,
                    requestedCreds,
                    masterKey,
                    schemas,
                    credDefs,
                    "{}"
                    );

                return res;
            }
            catch (InvalidOperationException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public static async Task<bool> verifyDoctorProof(string proofJson)
        {
            string proofReqJson = getProofRequest();
            proofReqJson = proofReqJson.Replace(" ", string.Empty);
            proofReqJson = proofReqJson.Replace(Environment.NewLine, string.Empty);
            try
            {
                IOFacilitator io = new IOFacilitator();
                DoctorCredDefInfoModel model = JsonConvert.DeserializeObject
                    <DoctorCredDefInfoModel>(File.ReadAllText(
                        io.getDoctorCredDefConfigPathAbs()));
                string schemas = "{";
                schemas += "\"" + model.schema_id + "\":" + model.schema_json;
                schemas += "}";
                string credDefs = "{";
                credDefs += "\"" + model.cred_def_id + "\":" + model.cred_def_json;
                credDefs += "}";

                bool result = await AnonCreds.VerifierVerifyProofAsync(proofReqJson, proofJson,
                    schemas, credDefs, "{}", "{}");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return false;
            }
        }

        private static string getReferentFromCredential(string json)
        {
            JArray jArr = JArray.Parse(json);
            if (jArr.Count == 0)
                throw new InvalidOperationException("This wallet does not contain a credential that meets the doctor proof requirements");

            return jArr[0]["cred_info"]["referent"].ToString();
        }

        private static string proverDoctorRequestCreds(string attr1_referent,
            string attr2_referent, string predicate1_referent)
        {
            string json = "{";
              json += "\"self_attested_attributes\": {},";
              json += "\"requested_attributes\": {";
                json += "\"attr1_referent\": {";
                  json += "\"cred_id\": \"" + attr1_referent + "\",";
                  json += "\"revealed\": true";
                json += "},";
                json += "\"attr2_referent\": {";
                  json += "\"cred_id\": \"" + attr2_referent + "\",";
                  json += "\"revealed\": true";
                json += "}";
              json += "},";
              json += "\"requested_predicates\": {";
                json += "\"predicate1_referent\": {";
                  json += "\"cred_id\": \"" + predicate1_referent + "\"";
                json += "}";
              json += "}";
            json += "}";

            return json;
        }

        private static async Task<string> getCredentialforRequest(
            CredentialSearchForProofRequest search, string itemReferent)
        {
            try
            {
                return await AnonCreds.ProverFetchCredentialsForProofRequestAsync(search,
                    itemReferent, 1);
            }
            catch (Exception e)
            {
                throw new Exception($"Error in fetching a credential for {itemReferent}. {e.Message}");
            }
        }
    }
}
