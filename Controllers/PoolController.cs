using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Hyperledger.Indy.PoolApi;


namespace indyClient
{
    class PoolController
    {
        private string d_identifier = "";
        private Pool d_openPool;

        public string getIdentifier()
        {
            return d_identifier;
        }

        public bool isOpen()
        {
            return d_identifier != "";
        }

        public Pool getOpenPool()
        {
            return d_openPool;
        }

        public async Task<string> create(string poolName,
            string genesisFilePath = "/var/lib/indy/sandbox/pool_transactions_genesis")
        {
            string config = "{\"genesis_txn\": \"" + genesisFilePath + "\"}";
            try
            {
                await Pool.CreatePoolLedgerConfigAsync(poolName, config);
                return "Created pool " + poolName + ".\n";
            }
            catch(Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> list()
        {
            try
            {
                return await Pool.ListPoolsAsync();
            }
            catch(Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        public async Task connect(string poolname)
        {
            try
            {
                d_openPool = await Pool.OpenPoolLedgerAsync(poolname, "{}");
                d_identifier = poolname;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                d_identifier = "";
            }
        }

        public async Task close()
        {
            if (!isOpen())
                return;

            try
            {
                await d_openPool.CloseAsync();
                d_identifier = "";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
