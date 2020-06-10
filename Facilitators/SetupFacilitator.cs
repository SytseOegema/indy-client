using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace indyClient
{
    class SetupFacilitator
    {
        WalletController d_wallet;
        LedgerController d_ledger;

        public SetupFacilitator(ref WalletController wallet,
            ref LedgerController ledger)
        {
            d_wallet = wallet;
            d_ledger = ledger;
        }

        public async Task setupEHREnvironment()
        {
            await createGenesisWallets();
            string myName = "Gov-Health-Department";

            string trusteeDid = await initialize("Trustee1");


            Console.WriteLine("create Gov-Health-Department wallet");
            await createAndPublishWallet("Trustee1", trusteeDid, myName,
                "00000000000Gov-Health-Department");


            string govDid = await initialize(myName);

            await createDoctorWallets(myName, govDid);
            await createERCredentials(myName, govDid);
            await createEHRWallets(myName, govDid);
            string schemaJson = await createSharedSecretSchema(myName, govDid);

            await setupSharedSecretCredentials("Patient1", schemaJson);
            await setupSharedSecretCredentials("Patient2", schemaJson);
        }

        public async Task setupSharedSecretCredentials(string issuer,
            string schemaJson)
        {
            await initialize(issuer);
            // create creddef in patient wallet
            string credDefDefinition = await d_ledger.createCredDef(
                schemaJson, "TAG1");

            JObject o = JObject.Parse(credDefDefinition);
            string credDefId = o["id"].ToString();

            // create cred def offer to share with trusted parties
            string credOffer = await d_wallet.createCredentialOffer(credDefId);

            // export wallet to ipfs
            await d_wallet.walletExportIpfs("export_key", issuer);

            // create shared secrets
            string secretsJson =
                await d_wallet.createEmergencySharedSecrets(3, 5);

            // schemaAttributes
            string schemaAttributes =
                "[\"secret_owner\", \"secret_issuer\", \"secret\"]";

            string[] trustees = {"TrustedParty1", "TrustedParty2", "TrustedParty3"
                , "TrustedParty4", "TrustedParty5"};

            for(int idx = 0; idx < 5; idx++)
            {
                o = (JObject) JArray.Parse(secretsJson)[idx];

                string schemaValues =
                    "[\"" + trustees[idx] + "\", \"" + issuer + "\", \"" +
                    o["id"] + "\"]";

                await issueCredential(issuer, trustees[idx], "shared-secret-" + issuer,
                    schemaAttributes, schemaValues, schemaJson,
                    credOffer, credDefDefinition);
            }
            await d_wallet.close();
        }

        public async Task<string> createSharedSecretSchema(string issuer,
            string issuerDid)
        {
            await initialize(issuer, issuerDid);
            Console.WriteLine("creating schema for sharing emergency shared secrets");

            string schemaAttributes =
            "[\"secret_owner\", \"secret_issuer\", \"secret\"]";
            string schemaJson = await d_ledger.createSchema(
                "Emergency-Shared-Secret", "1.0.0", schemaAttributes);

            Console.WriteLine("schemaJson:" + schemaJson);
            await d_wallet.close();
            return schemaJson;
        }

        private async Task issueCredential(string issuer, string walletId,
            string masterSecret, string schemaAttributes, string schemaValues,
            string schemaJson, string credOffer, string credDefDefinition)
        {
            await initialize(walletId);

            string linkSecret =
                await d_wallet.createMasterSecret(masterSecret);
            string credReq = await d_wallet.createCredentialRequest(
                credOffer, credDefDefinition, linkSecret);

            JObject o = JObject.Parse(credReq);
            string credReqJson = o["CredentialRequestJson"].ToString();
            string credReqMetaJson =
                o["CredentialRequestMetadataJson"].ToString();

            CredDefFacilitator credDefFac = new CredDefFacilitator();

            string credValue = credDefFac.generateCredValueJson(
                schemaAttributes, schemaValues);

            await initialize(issuer);

            string cred = await d_wallet.createCredential(credOffer,
                credReqJson, credValue);

            await d_wallet.open(walletId);
            await d_wallet.storeCredential(credReqMetaJson,
                cred, credDefDefinition);
        }

        public async Task createERCredentials(string issuer, string issuerDid)
        {
            await initialize(issuer, issuerDid);

            Console.WriteLine("creating schema Doctor-Certificate");
            string schemaAttributes =
                "[\"name\", \"is_emergency_doctor\", \"school\"]";
            string schemaJson = await d_ledger.createSchema(
                "Doctor-Certificate", "1.0.0", schemaAttributes);

            Console.WriteLine("creating CredDef for schema Doctor-Certificate");
            string credDefDefinition = await d_ledger.createCredDef(
                schemaJson, "TAG1");

            JObject o = JObject.Parse(credDefDefinition);
            string credDefId = o["id"].ToString();

            string credOffer = await d_wallet.createCredentialOffer(credDefId);

            Console.WriteLine("Creating Docotor-Certificate Credentials for: ");
            string[] doctors = {"Doctor1", "Doctor2", "Doctor3"};
            CredDefFacilitator credDefFac = new CredDefFacilitator();

            o = JObject.Parse(schemaJson);
            string schemaId = o["id"].ToString();

            DoctorCredDefInfoModel model = new DoctorCredDefInfoModel();
            model.issuer_did = issuerDid;
            model.schema_id = schemaId;
            model.schema_json = schemaJson;
            model.cred_def_json = credDefDefinition;
            model.cred_def_id = credDefId;
            IOFacilitator io = new IOFacilitator();

            io.createFile(JsonConvert.SerializeObject(model),
                io.getDoctorCredDefConfigPathRel());

            foreach (string doctor in doctors) {
                Console.WriteLine(doctor);
                await initialize(doctor);

                string linkSecret =
                    await d_wallet.createMasterSecret("doctor-certificate");
                string credReq = await d_wallet.createCredentialRequest(
                    credOffer, credDefDefinition, linkSecret);

                o = JObject.Parse(credReq);
                string credReqJson = o["CredentialRequestJson"].ToString();
                string credReqMetaJson =
                    o["CredentialRequestMetadataJson"].ToString();


                string schemaValues = "[\"" + doctor + "\", 1, \"RUG\"]";
                string credValue = credDefFac.generateCredValueJson(
                    schemaAttributes, schemaValues);
                await d_wallet.open(issuer);
                string cred = await d_wallet.createCredential(credOffer,
                    credReqJson, credValue);

                await d_wallet.open(doctor);
                await d_wallet.storeCredential(credReqMetaJson,
                    cred, credDefDefinition);
            }

        }


        public async Task createEHRWallets(string issuer, string issuerDid)
        {
            await createAndPublishWallet(issuer, issuerDid, "Patient1",
                "000000000000000000000000Patient1");
            await createAndPublishWallet(issuer, issuerDid, "Patient2",
                "000000000000000000000000Patient2");

            await createAndPublishWallet(issuer, issuerDid, "TrustedParty1",
                "0000000000000000000TrustedParty1");
            await createAndPublishWallet(issuer, issuerDid, "TrustedParty2",
                "0000000000000000000TrustedParty2");
            await createAndPublishWallet(issuer, issuerDid, "TrustedParty3",
                "0000000000000000000TrustedParty3");
            await createAndPublishWallet(issuer, issuerDid, "TrustedParty4",
                "0000000000000000000TrustedParty4");
            await createAndPublishWallet(issuer, issuerDid, "TrustedParty5",
                "0000000000000000000TrustedParty5");
        }

        public async Task createDoctorWallets(string issuer, string issuerDid)
        {
            var exists = await d_wallet.exists("Doctor1");
            if (exists)
            {
                Console.WriteLine("Doctor wallets already exists.");
                return;
            }
            await createAndPublishWallet(issuer, issuerDid, "Doctor1",
                "0000000000000000000000000Doctor1");
            await createAndPublishWallet(issuer, issuerDid, "Doctor2",
                "0000000000000000000000000Doctor2");
            await createAndPublishWallet(issuer, issuerDid, "Doctor3",
                "0000000000000000000000000Doctor3");
        }

        public async Task createGenesisWallets()
        {
            var exists = await d_wallet.exists("Trustee1");
            if (exists)
            {
                Console.WriteLine("Genesis wallets already exists.");
                return;
            }

            Console.WriteLine("Create genesis wallets:");
            await createWallet("Trustee1",
                "000000000000000000000000Trustee1");
            await createWallet("Steward1",
                "000000000000000000000000Steward1");
            await createWallet("Steward2",
                "000000000000000000000000Steward2");
            await d_wallet.close();
        }

        private async Task createAndPublishWallet(string issuer,
            string issuerDid, string walletId, string seed = "")
        {
            await createWallet(walletId, seed);
            await publishNYM(issuer, issuerDid, walletId);
        }

        private async Task createWallet(string walletId, string seed = "")
        {
            Console.WriteLine("Create wallet for " + walletId);
            await d_wallet.create(walletId);
            await d_wallet.open(walletId);
            await d_wallet.createDid(seed,
                "{purpose: Verinym}");
            await d_wallet.close();
        }

        private async Task publishNYM(string issuer, string issuerDid,
            string walletId)
        {
            await d_wallet.open(walletId);

            string didList = await d_wallet.listDids();
            await sendNym(issuer, issuerDid, didList);
        }

        private async Task sendNym(string issuer, string issuerDid,
        string didList, string role = "ENDORSER")
        {
            string did = JArray.Parse(didList)[0]["did"].ToString();
            string ver = JArray.Parse(didList)[0]["verkey"].ToString();

            await initialize(issuer, issuerDid);

            await d_ledger.sendNymRequest(did, ver, "", role);
            await d_wallet.close();
        }

        private async Task<string> initialize(string issuer, string issuerDid = "")
        {
          await d_wallet.open(issuer);

          string didList = await d_wallet.listDids();
          string did = JArray.Parse(didList)[0]["did"].ToString();

          if (issuerDid != "")
              did = issuerDid;

          d_wallet.setActiveDid(did);
          return did;
        }
    }
}
