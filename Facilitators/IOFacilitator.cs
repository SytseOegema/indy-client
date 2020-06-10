using System;
using System.IO;
using System.Collections;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace indyClient
{
    class IOFacilitator
    {
        private string d_walletExportPathRel;
        private string d_doctorCredDefConfigPathRel;
        private string d_homePath;

        public IOFacilitator()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            d_homePath = Environment.GetEnvironmentVariable(envHome) + "/.indy_client/";
            // /home/hyper/.indy_client
            d_walletExportPathRel = "wallet_export/";
            d_doctorCredDefConfigPathRel = "wallet_export/config_doctor_cred_def.json";
        }

        public string getDoctorCredDefConfigPathAbs()
        {
            return d_homePath + d_doctorCredDefConfigPathRel;
        }

        public string getDoctorCredDefConfigPathRel()
        {
            return d_doctorCredDefConfigPathRel;
        }

        public string getWalletExportPathAbs()
        {
            return d_homePath + d_walletExportPathRel;
        }

        public string getWalletExportPathRel()
        {
            return d_walletExportPathRel;
        }

        public string getHomePath()
        {
            return d_homePath;
        }

        public string getIpfsExportPathRel(string identifier)
        {
            return d_walletExportPathRel + identifier + "_ipfs_export.json";
        }

        public string getIpfsExportPathAbs(string identifier)
        {
            return getHomePath() + getIpfsExportPathRel(identifier);
        }

        public bool existsIpfsExportFile(string identifier)
        {
            return File.Exists(getIpfsExportPathAbs(identifier));
        }

        public string convertByteToTextFile(string relPath, string file)
        {
            string path = d_homePath + relPath;
            string command = "xxd " + path + file + " > " + path + file + ".txt";
            ShellFacilitator.Bash(command);
            return path + file + ".txt";
        }

        public void convertTextToByteFile(string relPath, string file)
        {
            string path = d_homePath + relPath;
            string command = "xxd -r " + path + file + ".txt > " + path + file;
            ShellFacilitator.Bash(command);
        }

        public void createFile(Stream content, string file)
        {
            using (var fileStream = File.Create(d_homePath + file))
            {
              content.CopyTo(fileStream);
            }
        }

        public void createFile(string content, string file)
        {
            using (StreamWriter fileStream = new StreamWriter(d_homePath + file))
            {
                fileStream.Write(content);
                fileStream.Flush();
            }
        }

        public void listDirectories(string path)
        {
            string fullPath = d_homePath + path;
            string [] files = Directory.GetDirectories(fullPath);
            foreach(string file in files)
            {
                Console.WriteLine(file.Replace(fullPath + "/", ""));
            }
        }

        public bool directoryExists(string pathAbs, string directory)
        {
            string [] files = Directory.GetDirectories(pathAbs);
            foreach(string file in files)
            {
                if (file.Replace(pathAbs, "") == directory)
                    return true;
            }
            return false;
        }
    }
}
