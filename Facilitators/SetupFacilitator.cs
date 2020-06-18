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

        static public void setupFolderStructure()
        {
            string command = "mkdir " + IOFacilitator.homePath();
            // create env folder
            ShellFacilitator.Bash(command + "/env");
            // create wallet_export folder
            ShellFacilitator.Bash(command + "/wallet_export");
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

            GovernmentSchemasModel govModel = new GovernmentSchemasModel();

            govModel.doctor_certificate_schema =
                await createDoctorCertificateSchema(myName, govDid);

            govModel.emergency_trusted_parties_schema =
                await createEmergencyTrustedPartiesSchema(myName, govDid);

            govModel.electronic_health_record_schema =
                await createElectronicHealthRecordSchema(myName, govDid);

            govModel.shared_secret_schema =
                await createSharedSecretSchema(myName, govDid);
            govModel.exportToJsonFile();


            await createDoctorWallets(myName, govDid);
            await createERCredentials(myName, govDid,
                govModel.doctor_certificate_schema);
            await createEHRWallets(myName, govDid);



            await setupSharedSecretCredentials("Patient1",
                govModel.shared_secret_schema);
            await setupSharedSecretCredentials("Patient2",
                govModel.shared_secret_schema);
            Console.WriteLine("\n\nAll Done!\n Have fun with the setup!");
        }

        public async Task createEHRCredentials(string schemaJson)
        {
            string[] doctors = {"Doctor1", "Doctor2", "Doctor3"};
            string[] patients = {"Patient1", "Patient2"};
            foreach (string doctor in doctors)
            {
                await initialize(doctor);
                string credDefDefinition =
                    await CredDefFacilitator.getCredDef("EHR", d_wallet);
                JObject o = JObject.Parse(credDefDefinition);
                string credDefId = o["id"].ToString();

                // create cred def offer to share with trusted parties
                string credOffer = await d_wallet.createCredentialOffer(credDefId);

                string schemaAttributes =
                    GovernmentSchemasModel.getSchemaAttributes(schemaJson);
                string schemaValues = "[\"1\", {\"issuer\":" + doctor + ", \"data\": \"data sample\"}]";
                foreach (string patient in patients)
                {
                    await issueCredential(doctor, patient, "EHR" + doctor + ":" + patient,
                        schemaAttributes, schemaValues, schemaJson,
                        credOffer, credDefDefinition);
                }
            }
        }

        public async Task setupSharedSecretCredentials(string issuer,
            string schemaJson)
        {
            await initialize(issuer);
            // create creddef in patient wallet
            string credDefDefinition = await d_ledger.createCredDef(
                schemaJson, "Emergency-Health-Record-Access");

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

                // share secret via credential
                await issueCredential(issuer, trustees[idx], "emergency-shared-secret-key-" + issuer,
                    schemaAttributes, schemaValues, schemaJson,
                    credOffer, credDefDefinition);

                // mark secret as shared
                await initialize(issuer);
                await d_wallet.updateRecordTag(
                    "shared-secret",
                    o["id"].ToString(),
                    "{\"~is_shared\": \"1\"}");
            }
            await d_wallet.close();
        }

        public async Task<string> createElectronicHealthRecordSchema(string issuer,
            string issuerDid)
        {
            await initialize(issuer, issuerDid);
            Console.WriteLine("creating schema for Electronic Health Records");

            string schemaAttributes =
            "[\"importance_level\", \"json\"]";
            string schemaJson = await d_ledger.createSchema(
                "Electronic-Health-Record", "1.0.0", schemaAttributes);

            Console.WriteLine("schemaJson:" + schemaJson);
            await d_wallet.close();
            return schemaJson;
        }

        public async Task<string> createEmergencyTrustedPartiesSchema(string issuer,
            string issuerDid)
        {
            await initialize(issuer, issuerDid);
            Console.WriteLine("creating schema for for the sharing of emeregency trusted parties between the shared secret issuer and the Gov-Health-Department");

            string schemaAttributes =
            "[\"secret_owner\", \"secret_issuer\", \"min\", \"total\"]";
            string schemaJson = await d_ledger.createSchema(
                "Emergency-Trusted-Parties", "1.0.0", schemaAttributes);

            Console.WriteLine("schemaJson:" + schemaJson);
            await d_wallet.close();
            return schemaJson;
        }

        public async Task<string> createDoctorCertificateSchema(string issuer, string issuerDid)
        {
            await initialize(issuer, issuerDid);
            Console.WriteLine("creating schema Doctor-Certificate");
            string schemaAttributes =
                "[\"name\", \"is_emergency_doctor\", \"school\"]";
            string schemaJson = await d_ledger.createSchema(
                "Doctor-Certificate", "1.0.0", schemaAttributes);

            Console.WriteLine("schemaJson:" + schemaJson);
            await d_wallet.close();
            return schemaJson;
        }

        public async Task<string> createSharedSecretSchema(string issuer,
            string issuerDid)
        {
            await initialize(issuer, issuerDid);
            Console.WriteLine("creating schema for sharing emergency shared secrets");

            string schemaAttributes =
            "[\"secret_owner\", \"secret_issuer\", \"secret\"]";
            string schemaJson = await d_ledger.createSchema(
                "Shared-Secret", "1.0.0", schemaAttributes);

            Console.WriteLine("schemaJson:" + schemaJson);
            await d_wallet.close();
            return schemaJson;
        }

        private async Task issueCredential(string issuer, string walletId,
            string masterSecret, string schemaAttributes, string schemaValues,
            string schemaJson, string credOffer, string credDefDefinition)
        {
            Console.WriteLine("issue credential, issuer: " + issuer + ", credential owner: " + walletId);
            await initialize(walletId);

            string linkSecret =
                await d_wallet.createMasterSecret(masterSecret);
            string credReq = await d_wallet.createCredentialRequest(
                credOffer, credDefDefinition, linkSecret);

            JObject o = JObject.Parse(credReq);
            string credReqJson = o["CredentialRequestJson"].ToString();
            string credReqMetaJson =
                o["CredentialRequestMetadataJson"].ToString();


            string credValue = CredentialFacilitator.generateCredValueJson(
                schemaAttributes, schemaValues);

            await initialize(issuer);

            string cred = await d_wallet.createCredential(credOffer,
                credReqJson, credValue);

            await d_wallet.open(walletId);
            await d_wallet.storeCredential(credReqMetaJson,
                cred, credDefDefinition);
        }

        public async Task createERCredentials(string issuer, string issuerDid,
            string schemaJson)
        {
            await initialize(issuer, issuerDid);



            Console.WriteLine("creating CredDef for schema Doctor-Certificate");
            string credDefDefinition = await d_ledger.createCredDef(
                schemaJson, "TAG1");

            JObject o = JObject.Parse(credDefDefinition);
            string credDefId = o["id"].ToString();

            string credOffer = await d_wallet.createCredentialOffer(credDefId);

            Console.WriteLine("Creating Docotor-Certificate Credentials for: ");
            string[] doctors = {"Doctor1", "Doctor2", "Doctor3"};

            o = JObject.Parse(schemaJson);
            string schemaId = o["id"].ToString();

            EmergencyDoctorCredentialModel model = new EmergencyDoctorCredentialModel(
                issuerDid,
                schemaId,
                schemaJson,
                credDefId,
                credDefDefinition);

            model.exportToJsonFile();

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

                string schemaAttributes =
                    GovernmentSchemasModel.getSchemaAttributes(schemaJson);
                string schemaValues = "[\"" + doctor + "\", 1, \"RUG\"]";
                string credValue = CredentialFacilitator.generateCredValueJson(
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
            await initialize("Patient1");
            await CredDefFacilitator.createPatientCredentialDefinitions(d_ledger);

            await createAndPublishWallet(issuer, issuerDid, "Patient2",
                "000000000000000000000000Patient2");
            await initialize("Patient2");
            await CredDefFacilitator.createPatientCredentialDefinitions(d_ledger);

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
            await initialize("Doctor1");
            await CredDefFacilitator.createPatientCredentialDefinitions(d_ledger);
            await createAndPublishWallet(issuer, issuerDid, "Doctor2",
                "0000000000000000000000000Doctor2");
            await initialize("Doctor2");
            await CredDefFacilitator.createPatientCredentialDefinitions(d_ledger);
            await createAndPublishWallet(issuer, issuerDid, "Doctor3",
                "0000000000000000000000000Doctor3");
            await initialize("Doctor3");
            await CredDefFacilitator.createPatientCredentialDefinitions(d_ledger);
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
