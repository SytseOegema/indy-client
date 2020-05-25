using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Hyperledger.Indy.NonSecretsApi;
using Hyperledger.Indy.AnonCredsApi;



namespace indyClient
{
    class WalletController
    {
        private string d_walletConfig;
        private string d_walletCredentials;
        private string d_identifier = "";
        private Wallet d_openWallet;
        private DidController d_didController;

        public WalletController()
        {
            d_didController = new DidController();
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

        public async Task create(string identifier)
        {
            d_identifier = identifier;
            setWalletInfo();

            try
            {
                await Wallet.CreateWalletAsync(d_walletConfig, d_walletCredentials);
                resetWalletInfo();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task<string> open(string identifier)
        {
            await close();

            d_identifier = identifier;
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

        public async Task<string> createCredDef(string schemaJson, string tag)
        {
            try
            {
                string credDefConfigJson = "{\"support_revocation\":false}";

                Console.WriteLine(schemaJson);
                Console.WriteLine(credDefConfigJson);

                var res = await AnonCreds.IssuerCreateAndStoreCredentialDefAsync(
                getOpenWallet(),
                getActiveDid(),
                schemaJson,
                tag,
                null,
                credDefConfigJson);

                // Console.WriteLine(res.CredDefId);
                // Console.WriteLine(res.CredDefJson);
                return res.CredDefJson;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error make cred def: {e.Message}");
                return e.Message;
            }
        }

        public async Task getCredentials(string walletQuery)
        {
            try
            {
                var creds = await AnonCreds.ProverSearchCredentialsAsync(
                    d_openWallet, walletQuery);
                    Console.WriteLine(creds);

                var res = await AnonCreds.ProverFetchCredentialsAsync(
                creds, 1);
                Console.WriteLine(res);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
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
              Console.WriteLine(count);

              // get count schema's
              res = await NonSecrets.FetchNextRecordsAsync(
              d_openWallet, list, Int32.Parse(count));

              // make response human readable
              o = JObject.Parse(res);

              // parse member value, because it contains schemaJson
              for (int idx = 0; idx < Int32.Parse(count); ++idx)
              {
                  o["records"][idx]["value"] = JObject.Parse(o["records"][idx]["value"].ToString());
              }

              return o["records"].ToString();
          }
          catch (Exception e)
          {
            return $"Error: {e.Message}";
          }
        }

          public async Task<string> getSchemas()
        {
            return await getRecord("schema", "{}",
            "{\"retrieveTotalCount\": true, \"retrieveType\": true, \"retrieveTags\": true}");
        }

        private void setWalletInfo()
        {
            d_walletConfig = "{ \"id\": \"" + d_identifier + "\" }";
            d_walletCredentials = "{ \"key\": \"" + d_identifier + "\" }";
        }

        private void resetWalletInfo()
        {
          d_walletConfig = "";
          d_walletCredentials = "";
          d_identifier = "";
        }

        public bool isOpen()
        {
            return d_identifier != "";
        }
    }
}
