using System;
using System.Threading.Tasks;
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

            Console.WriteLine("create Gov-Health-Department wallet");
            await d_wallet.create(myName);
            await d_wallet.open(myName);
            await d_wallet.createDid("00000000000Gov-Health-Department",
                "{purpose: Verinym}");

            await d_wallet.open("Trustee1");
            string didList = await d_wallet.listDids();
            string trusteeDid = JArray.Parse(didList)[0]["did"].ToString();

            await d_wallet.open(myName);
            didList = await d_wallet.listDids();
            await sendNym("Trustee1", trusteeDid, didList, "ENDORSER");

            string govDid = JArray.Parse(didList)[0]["did"].ToString();
            await createDoctorWallets(myName, govDid);
            await createERCredentials(myName, govDid);
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
                io.getWalletExportPathRel()
                + "config_doctor_cred_def.json");

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


        public async Task createDoctorWallets(string issuer, string issuerDid)
        {
            var exists = await d_wallet.exists("Doctor1");
            if (exists)
            {
                Console.WriteLine("Doctor wallets already exists.");
                return;
            }
            Console.WriteLine("create wallet for doctor 1");
            await d_wallet.create("Doctor1");
            await d_wallet.open("Doctor1");
            await d_wallet.createDid("0000000000000000000000000Doctor1",
                "{purpose: Verinym}");

            string didList = await d_wallet.listDids();
            await sendNym(issuer, issuerDid, didList);

            Console.WriteLine("create wallet for doctor 2");
            await d_wallet.create("Doctor2");
            await d_wallet.open("Doctor2");
            await d_wallet.createDid("0000000000000000000000000Doctor2",
                "{purpose: Verinym}");

            didList = await d_wallet.listDids();
            await sendNym(issuer, issuerDid, didList);

            Console.WriteLine("create wallet for doctor 3");
            await d_wallet.create("Doctor3");
            await d_wallet.open("Doctor3");
            await d_wallet.createDid("0000000000000000000000000Doctor3",
                "{purpose: Verinym}");

            didList = await d_wallet.listDids();
            await sendNym(issuer, issuerDid, didList);

            await d_wallet.close();
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
            await d_wallet.create("Trustee1");
            await d_wallet.open("Trustee1");
            await d_wallet.createDid("000000000000000000000000Trustee1",
                "{purpose: Verinym}");

            await d_wallet.create("Steward1");
            await d_wallet.open("Steward1");
            await d_wallet.createDid("000000000000000000000000Steward1",
                "{purpose: Verinym}");

            await d_wallet.create("Steward2");
            await d_wallet.open("Steward2");
            await d_wallet.createDid("000000000000000000000000Steward2",
                "{purpose: Verinym}");
            await d_wallet.close();
        }

        private async Task sendNym(string issuer, string issuerDid,
        string didList, string role = "")
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
