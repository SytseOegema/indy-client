using System;
using System.IO;
using System.Collections;
using System.Runtime;
using System.Runtime.InteropServices;

namespace indyClient
{
    class IOFacilitator
    {
        private string d_homePath;

        public IOFacilitator()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            d_homePath = Environment.GetEnvironmentVariable(envHome) + "/.indy_client";

            Console.WriteLine(d_homePath);
        }


        public void listDirectories(string path)
        {
            string fullPath = d_homePath + path;
            string [] files = Directory.GetDirectories(path);
            foreach(string file in files)
            {
                Console.WriteLine(file.Replace(fullPath + "/", ""));
            }
        }

    }
}
