using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Ipfs.Http;

namespace indyClient
{
    class IpfsFacilitator
    {
        private string d_baseUrl = "http://127.0.0.1:5001";
        static IpfsClient ipfs = new IpfsClient("http://127.0.0.1:5001");
        private static readonly HttpClient client = new HttpClient();
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
            string url = d_baseUrl + "/api/v0/get?arg=" + ipfsPath + "&output=/home";
            var response = await client.PostAsync(url, null);
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
        }
    }
}
