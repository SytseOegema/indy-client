using System;
using System.IO;
using System.Threading.Tasks;
using Ipfs.Http;

namespace indyClient
{
    class IpfsFacilitator
    {
        static IpfsClient ipfs = new IpfsClient("http://127.0.0.1:5001");
        private IOFacilitator io = new IOFacilitator();

        public async Task test()
        {
            var peer = await ipfs.IdAsync();
            Console.WriteLine(peer);
        }

        public async Task<string> addFile(string localPath)
        {
            var res = await ipfs.FileSystem.AddFileAsync(localPath);
            return res.Id;
        }

        public async Task getFile(string ipfsPath)
        {
            Stream res = await ipfs.FileSystem.GetAsync(ipfsPath);
            io.createFile(res, "test");
        }
    }
}
