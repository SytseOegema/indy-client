using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace indyClient
{
    /*
     * This class is used to convert wallet backup information to json.
     * This information can be used to recover a lost wallet.
     */
    public class WalletBackupModel
    {
        public string ipfs_path;
        public string wallet_identifier;
        public string wallet_key;
        public string export_key;

        public WalletBackupModel(string ipfsPath, string walletIdentifier,
            string walletKey, string exportKey)
        {
            ipfs_path = ipfsPath;
            wallet_identifier = walletIdentifier;
            wallet_key = walletKey;
            export_key = exportKey;
        }

        static public string filePath(string walletIdentifier)
        {
            return $"wallet_export/{walletIdentifier}_WBS.json";
        }

        static public WalletBackupModel importFromJson(string configJson)
        {
            return JsonConvert.DeserializeObject
                <WalletBackupModel>(configJson);
        }

        static public WalletBackupModel importFromJsonFile(string walletIdentifier)
        {
            string importJson = IOFacilitator.readFile(
                WalletBackupModel.filePath(walletIdentifier));
            return WalletBackupModel.importFromJson(importJson);
        }

        /*
         * The wallet backup information is stored in a file in
         * filePath(identifier). This file should be deleted after it has
         * served its purpose.
         */
        public void exportToJsonFile()
        {
            IOFacilitator.createFile(toJson(), filePath(wallet_identifier));
        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
