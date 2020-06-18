using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace indyClient
{
    static class CredDefFacilitator
    {
        static public async Task createPatientCredentialDefinitions(
            LedgerController ledger)
        {
            GovernmentSchemasModel model =
                GovernmentSchemasModel.importFromJsonFile();
            // create cred def that defines trusted party with emergency secret
            await ledger.createCredDef(
                model.emergency_trusted_parties_schema,
                "EHRTP");
            // create cred def for wallet backup data
            await ledger.createCredDef(
                model.shared_secret_schema,
                "EHRSS");
            // create cred def for emergency EHR access data
            await ledger.createCredDef(
                model.shared_secret_schema,
                "WBSS");
            // create cred def for EHR data
            await ledger.createCredDef(
                model.electronic_health_record_schema,
                "EHR");
            return;
        }

        static public async Task<string> getCredDef(string tag, WalletController wallet)
        {
            string list = await wallet.listCredDefs();
            Console.WriteLine(list);
            JArray a = JArray.Parse(list);
            foreach(var o in a.Children())
            {
                Console.WriteLine(o.ToString());
                string id = o["id"].ToString();
                if (id.Contains(tag))
                    return o.ToString();
            }
            return "No credential matches the tag: " + tag;
        }

    }
}
