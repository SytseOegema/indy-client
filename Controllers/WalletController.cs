using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Hyperledger.Indy.NonSecretsApi;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.BlobStorageApi;



namespace indyClient
{
    class WalletController
    {
        private string d_walletConfig;
        private string d_walletCredentials;
        private string d_identifier = "";
        private string d_masterKey = "";
        private Wallet d_openWallet;
        private DidController d_didController;

        public WalletController()
        {
            d_didController = new DidController();
        }

        public bool hasActiveDid()
        {
            return d_didController.hasActiveDid();
        }

        public string getActiveDid()
        {
            return d_didController.getActiveDid();
        }

        public void setActiveDid(string did)
        {
            d_didController.setActiveDid(did);
        }

        public Wallet getOpenWallet()
        {
            return d_openWallet;
        }

        public string getIdentifier()
        {
            return d_identifier;
        }

        public async Task create(string identifier, string masterKey = "")
        {
            d_identifier = identifier;
            if (masterKey == "")
                masterKey = identifier;
            d_masterKey = masterKey;
            setWalletInfo();

            try
            {
                await Wallet.CreateWalletAsync(d_walletConfig, d_walletCredentials);
                await close();
                resetWalletInfo();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task<string> open(string identifier, string key = "")
        {
            // for ease of use most wallet keys are equal to the identifier.
            if (key == "")
                key = identifier;

            await close();

            d_identifier = identifier;
            d_masterKey = key;
            setWalletInfo();

            try
            {
              d_openWallet = await Wallet.OpenWalletAsync(d_walletConfig, d_walletCredentials);

              d_didController.setOpenWallet(d_openWallet);

              return "wallet " + d_identifier + " opened";
            }
            catch (Exception e)
            {
                d_identifier = "";
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> close()
        {
            try
            {
                // Check if there is an wallet open
                if (isOpen())
                {
                    await d_openWallet.CloseAsync();

                    string identifier = d_identifier;
                    resetWalletInfo();

                    d_didController.setOpenWallet(null);

                    return "wallet " + identifier + " closed";
                }
                return "There is no open wallet to close.";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<bool> exists(string identifier)
        {
            var res = await open(identifier);

            await close();

            res.ToString();

            return !res.Contains("Error: The wallet does not exist.");
        }

        public async Task<string> listDids()
        {
            return await d_didController.list();
        }

        public async Task<string> createDid(string seed, string metaData)
        {
            var didJson = await d_didController.create(seed);
            var did = JObject.Parse(didJson)["Did"].ToString();
            await d_didController.addMetaData(did, metaData);
            return didJson;
        }

        public async Task<string> listCredDefs()
        {
            string res = await getRecord("creddef", "{}",
                "{\"retrieveTotalCount\": true, \"retrieveType\": true, \"retrieveTags\": true}");
            PrettyPrintFacilitator pretty = new PrettyPrintFacilitator();
            return pretty.dePrettyJsonMember(res, "value");
        }

        public async Task<string> createMasterSecret(string secretId)
        {
            try
            {
                string secret = await AnonCreds.ProverCreateMasterSecretAsync(
                    d_openWallet, secretId);
                return secret;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> createCredentialOffer(string credDefId)
        {
            try
            {
                string credOfferJson = await AnonCreds.IssuerCreateCredentialOfferAsync(
                d_openWallet, credDefId);
                return credOfferJson;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> createCredentialRequest(
            string credOfferJson,
            string credDefJson,
            string linkSecret)
        {
            try
            {
                var credReq = await AnonCreds.ProverCreateCredentialReqAsync(
                    d_openWallet,
                    d_didController.getActiveDid(),
                    credOfferJson,
                    credDefJson,
                    linkSecret);

                // credReq: {CredentialRequestJson, CredentialRequestMetadataJson}
                string json = JsonConvert.SerializeObject(credReq);
                JObject o = new JObject();
                o["CredentialRequestJson"] = JObject.Parse(credReq.CredentialRequestJson);
                o["CredentialRequestMetadataJson"] = JObject.Parse(credReq.CredentialRequestMetadataJson);
                json = o.ToString();
                PrettyPrintFacilitator pretty =  new PrettyPrintFacilitator();
                json = pretty.dePrettyJsonMember(json, "CredentialRequestJson");
                json = pretty.dePrettyJsonMember(json, "CredentialRequestMetadataJson");
                return json;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> createCredential(string credOfferJson,
            string credReqJson, string credValueJson, string revRegId = null,
            BlobStorageReader blob = null)
        {
            try
            {
                var cred = await AnonCreds.IssuerCreateCredentialAsync(
                    d_openWallet, credOfferJson, credReqJson, credValueJson,
                    revRegId, blob);
                // cred: {CredentailJson, RevocId, RevocRegDeltaJson}
                return cred.CredentialJson;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> storeCredential(string credReqMetaJson,
            string credJson, string credDefJson, string revRegDefJson = null)
        {
            try
            {
                string res = await AnonCreds.ProverStoreCredentialAsync(
                    d_openWallet, null,credReqMetaJson, credJson, credDefJson,
                    revRegDefJson);
                return res;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> getCredentials(string walletQuery)
        {
            try
            {
                var creds = await AnonCreds.ProverSearchCredentialsAsync(
                    d_openWallet, walletQuery);

                int count = creds.TotalCount;
                // return "0" if there are no records for the type and query
                if (count == 0)
                    return "0";

                // get count schema's
                string res = await AnonCreds.ProverFetchCredentialsAsync(
                creds, count);

                // make response human readable
                JArray a = JArray.Parse(res);

                return a.ToString();
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> addRecord(string type,
            string id, string value, string tagsJson)
        {
            try
            {
                await NonSecrets.AddRecordAsync(
                    d_openWallet, type, id, value, tagsJson);
                return "succes!";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task deleteRecord(string type , string id)
        {
            try
            {
                await NonSecrets.DeleteRecordAsync(d_openWallet, type, id);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task updateRecordTag(string type, string id,
            string tagJson)
        {
            try
            {
                await NonSecrets.UpdateRecordTagsAsync(d_openWallet, type, id,
                    tagJson);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        public async Task<string> getRecord(string type,
        string queryJson, string optionsJson)
        {
          try
          {
              var list = await NonSecrets.OpenSearchAsync(
              d_openWallet, type, queryJson, optionsJson);

              // get 0 schema's
              var res = await NonSecrets.FetchNextRecordsAsync(
              d_openWallet, list, 0);

              // parse result to see the count of schema's
              JObject o = JObject.Parse(res);
              string count = o["totalCount"].ToString();

              // return "0" if there are no records for the type and query
              if (count == "0")
                  return "0";

              // get count schema's
              res = await NonSecrets.FetchNextRecordsAsync(
              d_openWallet, list, Int32.Parse(count));

              // make response human readable
              o = JObject.Parse(res);

              return o["records"].ToString();
          }
          catch (Exception e)
          {
            return $"Error: {e.Message}";
          }
        }

        public async Task<string> listSchemas()
        {
            string res = await getRecord("schema", "{}",
            "{\"retrieveTotalCount\": true, \"retrieveType\": true, \"retrieveTags\": true}");
            PrettyPrintFacilitator pretty = new PrettyPrintFacilitator();
            return pretty.dePrettyJsonMember(res, "value");
        }

        public async Task<string> walletExportLocal(string path, string key)
        {
            string json = "{\"path\": \"" + path + "\",";
            json += "\"key\": \"" + key + "\"}";
            try
            {
                await d_openWallet.ExportAsync(json);
                return "Wallet " + d_identifier + " has been exported to " +
                    path;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> walletExportIpfs(
            string exportKey, string walletKey = "")
        {
            string path = IOFacilitator.homePath() + d_identifier + "WBtemp";
            try
            {
                // create export file
                await walletExportLocal(path, exportKey);

                // convert export file from byte file to txt file
                string textFilePath = IOFacilitator.convertByteToTextFile(
                    d_identifier + "WBtemp");

                // upload text file to ipfs
                IpfsFacilitator ipfs = new IpfsFacilitator();
                string ipfsPath = await ipfs.addFile(textFilePath);

                WalletBackupModel model = new WalletBackupModel(
                    ipfsPath,
                    d_identifier,
                    (walletKey == "" ? d_identifier : walletKey),
                    exportKey);

                model.exportToJsonFile();

                return JsonConvert.SerializeObject(model);
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> walletImportLocal(string identifier, string path,
            string walletKey, string exportKey)
        {
            string config = "{\"id\": \"" + identifier + "\"}";
            string credentials = "{\"key\":\"" + walletKey + "\"}";
            string importConf = "{\"path\": \"" + path + "\",";
            importConf += "\"key\": \"" + exportKey + "\"}";

            try
            {
                await Wallet.ImportAsync(config, credentials, importConf);
                return "Wallet " + identifier + " has been imported";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> walletImportIpfs(string identifier,
            string jsonConfig)
        {
            WalletBackupModel model =
                WalletBackupModel.importFromJson(jsonConfig);

            IpfsFacilitator ipfs = new IpfsFacilitator();

            string localPath = IOFacilitator.homePath() + "temp";
            try
            {
                // get file content
                string txt = await ipfs.getFile(model.ipfs_path, identifier);
                // create local file from ipfs content
                IOFacilitator.createFile(txt, "temp.txt");
                // convert txt to binary
                IOFacilitator.convertTextToByteFile("temp.txt", "temp");
                // import wallet into client
                string res = await walletImportLocal(identifier, localPath, model.wallet_key,
                    model.export_key);
                return res;
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> listSharedSecrets(string query = "{}")
        {
          string res = await getRecord("shared-secret", query,
              "{\"retrieveTotalCount\": true, \"retrieveType\": true, \"retrieveTags\": true}");
          return res;
        }

        public async Task<string> createWalletBackupSharedSecrets(
            int min, int total)
        {
            string list = await listSharedSecrets();
            if (list != "0")
                throw new Exception("There allready exist shared secrets.");

            if (!IOFacilitator.fileExists(WalletBackupModel.filePath(d_identifier)))
                throw new Exception("There must be an IPFS backup of the wallet. No IPFS export JSON file was found for this wallet.");

            WalletBackupModel model =
                WalletBackupModel.importFromJsonFile(d_identifier);

            List<string> secrets = SecretSharingFacilitator.createSharedSecret(
                model.toJson(), min, total);

            int idx = 0;
            foreach (string secret in secrets)
            {
                await addRecord(
                    "shared-secret",
                    secret,
                    "WBSS",
                    createSharedSecretTagJson(++idx, min, total));
            }

            list = await listSharedSecrets();
            return list;
        }

        public async Task<string> holderSharedSecretProvide(
            string doctorProofJson, string issuerWalletName)
        {
            bool res = await DoctorProofFacilitator.verifyDoctorProof(
                doctorProofJson);
            if (!res)
                return "The doctor proof json that was provided is not valid!";

            string json = "{\"schema_id\": \"NcZ4tw9KDDGnCWpGShk9n5:2:Shared-Secret:1.0.0\"}";

            // return array with credentials json
            json = await getCredentials(json);
            JArray a = JArray.Parse(json);
            for(int idx = 0; idx < a.Count; ++idx)
            {
                JObject cred = (JObject) a[idx];
                if (cred["attrs"]["secret_issuer"].ToString() == issuerWalletName)
                    return cred["attrs"]["secret"].ToString();
            }
            return "No secret found for specified identifier";
        }

        private async Task<string> backupEHR()
        {
            string ehrJson = await getEHRCredentials();
            string emergencySecret = await EHRBackupModel.backupEHR(
                d_identifier, ehrJson);
            return emergencySecret;
        }

        public async Task<string> getEHRCredentials()
        {
            GovernmentSchemasModel model =
                GovernmentSchemasModel.importFromJsonFile();
            string schemaId =
                GovernmentSchemasModel.getSchemaId(
                    model.electronic_health_record_schema);

            return await getCredentials("{\"schema_id\": \""
            + schemaId + "\"}");
        }

        private string createSharedSecretTagJson(int num, int min, int total)
        {
            string json = "{";
            json += "\"is_shared\": \"0\",";
            json += "\"number\": \"" + num + "\",";
            json += "\"minimum\": \"" + min + "\",";
            json += "\"total\": \"" + total + "\"}";
            return json;
        }

        private void setWalletInfo()
        {
            d_walletConfig = "{ \"id\": \"" + d_identifier + "\" }";
            d_walletCredentials = "{ \"key\": \"" + d_masterKey + "\" }";
            setActiveDid("");
        }

        private void resetWalletInfo()
        {
            d_walletConfig = "";
            d_walletCredentials = "";
            d_identifier = "";
            d_masterKey = "";
            setActiveDid("");
        }

        public bool isOpen()
        {
            return d_identifier != "";
        }
    }
}
