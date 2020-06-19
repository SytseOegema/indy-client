using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Ipfs.Http;
using Newtonsoft.Json.Linq;

namespace indyClient
{
    public class IpfsFacilitator
    {
        private string d_baseUrl = "http://127.0.0.1:5001";
        private static IpfsClient ipfs = new IpfsClient("http://127.0.0.1:5001");
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> addFile(string absoluteFilePath)
        {
            var res = await ipfs.FileSystem.AddFileAsync(absoluteFilePath);
            return res.Id;
        }

        public async Task<string> getFile(string ipfsPath, string walletName)
        {
            string url = d_baseUrl + "/api/v0/cat?arg=" + ipfsPath;
            var response = await client.PostAsync(url, null);

            string contentString = await response.Content.ReadAsStringAsync();
            return contentString;
        }
    }
}
