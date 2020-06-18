using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace indyClient
{
    static class IOFacilitator
    {

        static public bool fileExists(string relativePath)
        {
            return File.Exists(IOFacilitator.homePath() + relativePath);
        }

        static public string convertByteToTextFile(string relPath)
        {
            string path = IOFacilitator.homePath() + relPath;
            string command = "xxd " + path + " > " + path + ".txt";
            ShellFacilitator.Bash(command);
            return path + ".txt";
        }

        static public void convertTextToByteFile(string relPathTxt, string relPath)
        {
            string path = IOFacilitator.homePath();
            string command = "xxd -r " + path + relPathTxt + " > " + path + relPath;
            ShellFacilitator.Bash(command);
        }

        static public void createFile(Stream content, string relFilePath)
        {
            using (var fileStream = File.Create(IOFacilitator.homePath()
                + relFilePath))
            {
              content.CopyTo(fileStream);
            }
        }

        static public void createFile(string content, string relFilePath)
        {
            using (StreamWriter fileStream = new StreamWriter(
                IOFacilitator.homePath() + relFilePath))
            {
                fileStream.Write(content);
                fileStream.Flush();
            }
        }

        static public string readFile(string relativeFilePath)
        {
            return (string) File.ReadAllText(homePath() + relativeFilePath);
        }

        static public string homePath()
        {
            // /home/hyper/.indy_client
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            return Environment.GetEnvironmentVariable(envHome) + "/.indy_client/";
        }

        static public void listDirectories(string relativePath)
        {
            string fullPath = IOFacilitator.homePath() + relativePath;
            string [] files = Directory.GetDirectories(fullPath);
            List<string> list = new List<string>(files);
            list.Sort();
            foreach(string file in list)
            {
                Console.WriteLine(file.Replace(fullPath + "/", ""));
            }
        }

        static public bool directoryExists(string pathAbs, string directory)
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
