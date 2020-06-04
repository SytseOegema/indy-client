using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

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

        public async Task<string> getCredentialForProof()
        {
            string proofReqJson = getProofRequest();
            proofReqJson = proofReqJson.Replace(" ", string.Empty);
            proofReqJson = proofReqJson.Replace(Environment.NewLine, string.Empty);

            Console.WriteLine(proofReqJson);

            try
            {
                Console.WriteLine("1");
                var credList =
                    await AnonCreds.ProverSearchCredentialsForProofRequestAsync(
                        d_walletController.getOpenWallet(), proofReqJson);
                Console.WriteLine("2");

                string attr1Cred = await getCredentialforRequest(
                    credList, "attr1_referent");

                Console.WriteLine("3");

                return attr1Cred;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        private async Task<string> getCredentialforRequest(
            CredentialSearchForProofRequest search, string itemReferent)
        {
            try
            {
                Console.WriteLine("a1");
                return await AnonCreds.ProverFetchCredentialsForProofRequestAsync(search,
                    itemReferent, 1);
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }
    }
}
