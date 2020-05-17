using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Hyperledger.Indy.PoolApi;

namespace indyClient
{
    class Reset
    {
        public Reset()
        {}

        public async Task reinitialize()
        {
            try
            {
                await PoolApi.CreatePoolLedgerConfig();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            Console.WriteLine("reinitialize not implemented yet.");
        }
    }
}
