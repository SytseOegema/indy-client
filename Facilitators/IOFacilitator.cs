using System;
using System.IO;
using System.Collections;

namespace indyClient
{
    class IOFacilitator
    {
var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
var home = Environment.GetEnvironmentVariable(envHome);`

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
