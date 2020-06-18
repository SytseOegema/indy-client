using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;

namespace indyClient
{
    public static class OfflineSecretController
    {
        public static async Task<string> obtain(string doctorProofJson,
            string walletIdentifier)
        {
            bool res = await DoctorProofFacilitator.verifyDoctorProof(
                doctorProofJson);
            if (!res)
                return "The doctor proof json that was provided is not valid!";

            EHRBackupModel model = EHRBackupModel.importFromJsonFile();

            return model.toJson();
        }
    }
}
