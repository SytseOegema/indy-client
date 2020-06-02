using System;
using System.Threading.Tasks;
using Ipfs.Http;

namespace indyClient
{
    class IpfsFacilitator
    {
        static IpfsClient ipfs = new IpfsClient("http://127.0.0.1:5001");

        public async Task test()
        {
            var peer = await ipfs.IdAsync();
            Console.WriteLine(peer);
        }
    }
}
