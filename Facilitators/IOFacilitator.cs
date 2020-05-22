using System;
using System.IO;
using System.Collections;
using System.Runtime;
using System.Runtime.InteropServices;

namespace indyClient
{
    class IOFacilitator
    {
        private string d_env;
        private string d_homePath;

        public IOFacilitator()
        {
            d_env = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            d_homePath = Environment.GetEnvironmentVariable(envHome);

            Console.WriteLine(d_homePath);
        }


        public void listDirectories(string path)
        {
            string [] files = Directory.GetDirectories(path);
            foreach(string file in files)
            {
                Console.WriteLine(file);
            }
        }

    }
}
