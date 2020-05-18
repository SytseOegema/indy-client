using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Hyperledger.Indy.PoolApi;


namespace indyClient
{
    class PoolController
    {
        private Pool d_openPool;
        public PoolController()
        {}

        public async Task connect(string poolname)
        {
            try
            {
                d_openPool = await Pool.OpenPoolLedgerAsync(poolname, "{}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public async Task close()
        {
            if (d_openPool == null)
            {
                return;
            }

            try
            {
                await Pool.CloseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
