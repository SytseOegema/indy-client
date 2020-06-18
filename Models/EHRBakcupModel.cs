using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace indyClient
{
    /*
     * This class is used to convert wallet backup information to json.
     * This information can be used to recover a lost wallet.
     */
    public class EHRBackupModel
    {
        public string ipfs_path;
        public string encryption_key;

        public EHRBackupModel(string ipfsPath, string encryptionKey)
        {
            ipfs_path = ipfsPath;
            encryption_key = encryptionKey;
        }

        static public string filePath(string walletIdentifier)
        {
            return $"wallet_export/{walletIdentifier}_ES.json";
        }

        static public EHRBackupModel importFromJson(string configJson)
        {
            return JsonConvert.DeserializeObject
                <EHRBackupModel>(configJson);
        }

        static public EHRBackupModel importFromJsonFile(string walletIdentifier)
        {
            string importJson = IOFacilitator.readFile(
                EHRBackupModel.filePath(walletIdentifier));
            return EHRBackupModel.importFromJson(importJson);
        }

        /*
         * The EHR backup information is stored in a file in
         * filePath(identifier). This file should be deleted after it has
         * served its purpose.
         */
        public void exportToJsonFile(string walletIdentifier)
        {
            IOFacilitator.createFile(toJson(), filePath(walletIdentifier));
        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        static public async Task<string> backupEHR(string walletId, string ehrJson)
        {
            string encryptionKey = "test";
            string relPath = walletId + "ESjson.temp";
            IOFacilitator.createFile(ehrJson, relPath);
            IpfsFacilitator ipfs = new IpfsFacilitator();
            string ipfsPath = await ipfs.addFile(IOFacilitator.homePath() + relPath);


            EHRBackupModel model = new EHRBackupModel(ipfsPath, encryptionKey);
            model.exportToJsonFile(walletId);
            return model.toJson();
        }
    }

}
