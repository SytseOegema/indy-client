using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace indyClient
{
    public class PatientController
    {
        private LedgerController d_ledgerController;
        private WalletController d_walletController;


        public LedgerController(ref LedgerController ledgerController,
            ref WalletController walletController)
        {
            d_ledgerController = ledgerController;
            d_walletController = walletController;
        }


        public async Task createPatientCredentialDefinitions()
        {
            GovernmentSchemasModel model =
                GovernmentSchemasModel.importFromJsonFile();
            // create cred def that defines trusted party with emergency secret
            await d_ledger.createCredDef(
                model.emergency_trusted_parties_schema,
                "EHRTP");
            // create cred def for wallet backup data
            await d_ledger.createCredDef(
                model.shared_secret_schema,
                "EHRSS");
            // create cred def for emergency EHR access data
            await d_ledger.createCredDef(
                model.shared_secret_schema,
                "WBSS");
            // create cred def for EHR data
            await d_ledger.createCredDef(
                model.electronic_health_record_schema,
                "EHR");

        }

        public async Task getCredDef(string tag)
        {
            string list = await d_walletController.listCredDefs();
            Console.WriteLine(list);
            JArray a = JArray.Parse(list);
            foreach(var o in a.Children())
            {
                Console.WriteLine(o.ToString());
                string id = item["id"].ToString();
                if (id.Contains(tag))
                    return o.ToString();
            }
        }

    }
}
