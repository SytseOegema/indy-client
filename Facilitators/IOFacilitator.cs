using System;
using System.IO;
using System.Collections;

namespace indyClient
{
    class IOFacilitator
    {
        public void listFiles(string path)
        {
            string [] files = Directory.GetFiles(path);
            foreach(string file in files)
            {
                Console.WriteLine(file);
            }
        }

    }
}
