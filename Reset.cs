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
            await PoolUtils.CreatePoolLedgerConfig();

            Console.WriteLine("reinitialize not implemented yet.");
        }
    }
}
