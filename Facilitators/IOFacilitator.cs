using System;
using System.IO;
using System.Collections;

namespace indyClient
{
    class IOFacilitator
    {
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
