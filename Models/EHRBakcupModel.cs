using System;
using System.IO;
using System.Text;
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
        public string encryption_iv;

        public EHRBackupModel(string ipfsPath, string encryptionKey,
            string encryptionIV)
        {
            ipfs_path = ipfsPath;
            encryption_key = encryptionKey;
            encryption_iv = encryptionIV;
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
            // encrypt ehr data
            CipherFacilitator cipher = new CipherFacilitator();
            string encryptedEHR = cipher.encrypt(ehrJson);

            Console.WriteLine("EHR encrypted: " + encryptedEHR);

            string relPath = walletId + "ESjson.temp";
            IOFacilitator.createFile(encryptedEHR, relPath);
            IpfsFacilitator ipfs = new IpfsFacilitator();
            string ipfsPath =
                await ipfs.addFile(IOFacilitator.homePath() + relPath);

            EHRBackupModel model = new EHRBackupModel(ipfsPath,
                cipher.getKey(), cipher.getIV());

            model.exportToJsonFile(walletId);
            return model.toJson();
        }

        public async Task<string> downloadEmergencyEHR()
        {
            IpfsFacilitator ipfs = new IpfsFacilitator();
            string encryptedEHR = await ipfs.getFile(ipfs_path, "");

            // decrypt ehr data
            CipherFacilitator cipher = new CipherFacilitator();
            cipher.setKey(encryption_key);
            cipher.setIV(encryption_iv);
            return cipher.decrypt(encryptedEHR);
        }
    }

}
