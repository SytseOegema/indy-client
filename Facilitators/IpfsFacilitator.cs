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
        private IOFacilitator io = new IOFacilitator();

        public async Task<string> addFile(string relPath)
        {
            string[] paths = splitFullPathLinux(relPath);
            string textFilePath = io.convertByteToTextFile(paths[0], paths[1]);
            var res = await ipfs.FileSystem.AddFileAsync(textFilePath);
            return res.Id;
        }

        public async Task getFile(string ipfsPath, string walletName)
        {
            string url = d_baseUrl + "/api/v0/cat?arg=" + ipfsPath;
            var response = await client.PostAsync(url, null);

            Stream contentStream = await response.Content.ReadAsStreamAsync();
            // string localPath = "export_wallets/" + walle tName;
            io.createFile(contentStream,
                "export_wallets/" + walletName + ".txt");
            io.convertTextToByteFile("export_wallets/", walletName);
        }

        private string[] splitFullPathLinux(string fullPath)
        {
            int idx = fullPath.LastIndexOf('/') + 1;
            string[] output = new string[2];
            output[0] = fullPath.Substring(0, idx);
            output[1] = fullPath.Substring(idx);
            return output;
        }
    }
}
