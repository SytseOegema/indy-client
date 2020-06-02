using System;
using System.Threading.Tasks;
using Ipfs.Http;

namespace indyClient
{
    class IpfsFacilitator
    {
        static IpfsClient ipfs = new IpfsClient("https://ipfs.ioz");

        public async Task test()
        {
            var peer = await ipfs.IdAsync();
            Console.WriteLine(peer);
        }
    }
}
