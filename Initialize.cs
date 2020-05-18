using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.PoolApi;

namespace indyClient
{
    class Initialize
    {
        public Initialize()
        {}

        public async Task reinitialize()
        {
            Console.WriteLine("reinitialize not implemented yet.");
            try
            {
                Console.WriteLine("reinitialize not implemented yet.");
                await Pool.CreatePoolLedgerConfigAsync("reset", "{ \"genesis_txn\": \"/var/lib/indy/reset/\" }");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

        }
    }
}
