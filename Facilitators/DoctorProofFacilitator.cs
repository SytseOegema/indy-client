using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Hyperledger.Indy.AnonCredsApi;


namespace indyClient
{
    class DoctorProofFacilitator
    {
        WalletController d_walletController;

        public DoctorProofFacilitator(ref WalletController wallet)
        {
            d_walletController = wallet;
        }

        public string getProofRequest()
        {
            return File.ReadAllText("Models/DoctorProofRequest.json");
        }

        /*
         * Proofs the holder of the open wallet is a doctor by using the first
         * credential that meets the docotr proof requirements.
         */
        public async Task<string> createDoctorProof()
        {
            string proofReqJson = getProofRequest();
            proofReqJson = proofReqJson.Replace(" ", string.Empty);
            proofReqJson = proofReqJson.Replace(Environment.NewLine, string.Empty);

            try
            {
                var credList =
                    await AnonCreds.ProverSearchCredentialsForProofRequestAsync(
                        d_walletController.getOpenWallet(), proofReqJson);

                string attr1Cred = await getCredentialforRequest(
                    credList, "attr1_referent");
                string attr2Cred = await getCredentialforRequest(
                    credList, "attr2_referent");
                string predicate1Cred = await getCredentialforRequest(
                    credList, "predicate1_referent");

                Console.WriteLine(predicate1Cred);
                Console.WriteLine(attr1Cred);
                Console.WriteLine(attr2Cred);

                string requestedCreds = proverDoctorRequestCreds(
                    getReferentFromCredential(attr1Cred),
                    getReferentFromCredential(attr2Cred),
                    getReferentFromCredential(predicate1Cred));

                return requestedCreds;
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

        private string getReferentFromCredential(string json)
        {
            JArray jArr = JArray.Parse(json);
            if (jArr.Count == 0)
                throw new InvalidOperationException("This wallet does not contain a credential that meets the doctor proof requirements");

            return jArr[0]["cred_info"]["referent"].ToString();
        }

        private string proverDoctorRequestCreds(string attr1_referent,
            string attr2_referent, string predicate1_referent)
        {
            string json = "{";
              json += "\"self_attested_attributes\": {},";
              json += "\"requested_attributes\": {";
                json += "\"attr1_referent\": {";
                  json += "\"cred_id\": \"" + attr1_referent + "\",";
                  json += "\"reveald\": True";
                json += "}";
                json += "\"attr2_referent\": {";
                  json += "\"cred_id\": \"" + attr2_referent + "\",";
                  json += "\"reveald\": True";
                json += "}";
              json += "}";
              json += "\"requested_predicates\": {";
                json += "\"predicate1_referent\": {";
                  json += "\"cred_id\": \"" + predicate1_referent + "\"";
                json += "}";
              json += "}";
            json += "}";

            return json;
        }

        private async Task<string> getCredentialforRequest(
            CredentialSearchForProofRequest search, string itemReferent)
        {
                return await AnonCreds.ProverFetchCredentialsForProofRequestAsync(search,
                    itemReferent, 1);
        }
    }
}