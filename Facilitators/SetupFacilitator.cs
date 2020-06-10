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

            await d_wallet.open("Trustee1");
            string didList = await d_wallet.listDids();
            string trusteeDid = JArray.Parse(didList)[0]["did"].ToString();

            Console.WriteLine("create Gov-Health-Department wallet");
            await createAndPublishWallet("Trustee1", trusteeDid, myName,
                "00000000000Gov-Health-Department");

            // await d_wallet.create(myName);
            // await d_wallet.open(myName);
            // await d_wallet.createDid("00000000000Gov-Health-Department",
            //     "{purpose: Verinym}");
            //
            //
            // await d_wallet.open(myName);
            // didList = await d_wallet.listDids();
            // await sendNym("Trustee1", trusteeDid, didList, "ENDORSER");

            string govDid = JArray.Parse(didList)[0]["did"].ToString();
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

            string[] trustees = {"TrustedPaty1", "TrustedPaty2", "TrustedPaty3"
                , "TrustedPaty4", "TrustedPaty5"};

            for(int idx = 0; idx < 5; idx++)
            {
                o = (JObject) JArray.Parse(secretsJson)[idx];

                string schemaValues =
                    "[\"" + trustees[idx] + "\", \"" + issuer + "\", \"" +
                    o["value"] + "\"]";

                await issueCredential(issuer, trustees[idx], "shared-secret",
                    schemaAttributes, schemaValues, schemaJson,
                    credOffer, credDefDefinition);
            }
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
            return schemaJson;
        }

        private async Task issueCredential(string issuer, string walletId,
            string masterSecret, string schemaAttributes, string schemaValues,
            string schemaJson, string credOffer, string credDefDefinition)
        {
            await d_wallet.open(walletId);

            // takes the first did from the list and makes it teh active did
            string didList = await d_wallet.listDids();
            d_wallet.setActiveDid(
                JArray.Parse(didList)[0]["did"].ToString());

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
            await d_wallet.open(issuer);
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

            Console.WriteLine(credDefDefinition);

            JObject o = JObject.Parse(credDefDefinition);
            string credDefId = o["id"].ToString();

            string credOffer = await d_wallet.createCredentialOffer(credDefId);

            Console.WriteLine("Creating Docotor-Certificate Credential for: ");
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
                await d_wallet.open(doctor);

                // takes the first did from the list and makes it teh active did
                string didList = await d_wallet.listDids();
                d_wallet.setActiveDid(
                    JArray.Parse(didList)[0]["did"].ToString());

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

            await createAndPublishWallet(issuer, issuerDid, "TrustedPaty1",
                "00000000000000000000TrustedPaty1");
            await createAndPublishWallet(issuer, issuerDid, "TrustedPaty2",
                "00000000000000000000TrustedPaty2");
            await createAndPublishWallet(issuer, issuerDid, "TrustedPaty3",
                "00000000000000000000TrustedPaty3");
            await createAndPublishWallet(issuer, issuerDid, "TrustedPaty4",
                "00000000000000000000TrustedPaty4");
            await createAndPublishWallet(issuer, issuerDid, "TrustedPaty5",
                "00000000000000000000TrustedPaty5");
        }

        public async Task createDoctorWallets(string issuer, string issuerDid)
        {
            var exists = await d_wallet.exists("Doctor1");
            if (exists)
            {
                Console.WriteLine("Doctor wallets already exists.");
                return;
            }
            Console.WriteLine("create wallet for doctor 1");
            await createAndPublishWallet(issuer, issuerDid, "Doctor1",
                "0000000000000000000000000Doctor1");
            await createAndPublishWallet(issuer, issuerDid, "Doctor2",
                "0000000000000000000000000Doctor2");
            await createAndPublishWallet(issuer, issuerDid, "Doctor3",
                "0000000000000000000000000Doctor3");

            // await d_wallet.create("Doctor1");
            // await d_wallet.open("Doctor1");
            // await d_wallet.createDid("0000000000000000000000000Doctor1",
            //     "{purpose: Verinym}");
            //
            // string didList = await d_wallet.listDids();
            // await sendNym(issuer, issuerDid, didList);
            //
            // Console.WriteLine("create wallet for doctor 2");
            // await d_wallet.create("Doctor2");
            // await d_wallet.open("Doctor2");
            // await d_wallet.createDid("0000000000000000000000000Doctor2",
            //     "{purpose: Verinym}");
            //
            // didList = await d_wallet.listDids();
            // await sendNym(issuer, issuerDid, didList);
            //
            // Console.WriteLine("create wallet for doctor 3");
            // await d_wallet.create("Doctor3");
            // await d_wallet.open("Doctor3");
            // await d_wallet.createDid("0000000000000000000000000Doctor3",
            //     "{purpose: Verinym}");
            //
            // didList = await d_wallet.listDids();
            // await sendNym(issuer, issuerDid, didList);
            //
            // await d_wallet.close();
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
            // await d_wallet.create("Trustee1");
            // await d_wallet.open("Trustee1");
            // await d_wallet.createDid("000000000000000000000000Trustee1",
            //     "{purpose: Verinym}");

            // await d_wallet.create("Steward1");
            // await d_wallet.open("Steward1");
            // await d_wallet.createDid("000000000000000000000000Steward1",
            //     "{purpose: Verinym}");

            // await d_wallet.create("Steward2");
            // await d_wallet.open("Steward2");
            // await d_wallet.createDid("000000000000000000000000Steward2",
            //     "{purpose: Verinym}");
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
            string docDid = JArray.Parse(didList)[0]["did"].ToString();
            string docVer = JArray.Parse(didList)[0]["verkey"].ToString();

            await initialize(issuer, issuerDid);

            await d_ledger.sendNymRequest(docDid, docVer, "Doctor", role);
            await d_wallet.close();
        }

        private async Task initialize(string issuer, string issuerDid)
        {
          await d_wallet.open(issuer);
          d_wallet.setActiveDid(issuerDid);
        }
    }
}
