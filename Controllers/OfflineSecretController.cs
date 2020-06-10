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

            IOFacilitator io = new IOFacilitator();
            string path = io.getIpfsExportPathAbs(walletIdentifier);

            WalletExportModel model = JsonConvert.DeserializeObject
                <WalletExportModel>(File.ReadAllText(path));

            return JsonConvert.SerializeObject(model);
        }
    }
}
