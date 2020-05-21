﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Hyperledger.Indy.NonSecretsApi;



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

            return !res.Contains("Error: The wallet does not exists.");
        }

        public async Task<string> listDids()
        {
            return await d_didController.list();
        }

        public async Task<string> createDid(string seed)
        {
            return await d_didController.create(seed);
        }

        public async Task<string> addRecord(string type,
            string id, string value, string tagsJson)
        {
            try
            {
                var res = await NonSecrets.AddRecordAsync(
                    d_openWallet, type, id, value, tagsJson);
                return res;
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

            var res = await NonSecrets.FetchNextRecordsAsync(
            d_openWallet, list, 1);
            return res;
          }
          catch (Exception e)
          {
            return $"Error: {e.Message}";
          }
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
