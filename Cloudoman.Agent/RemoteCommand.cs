using System;
using System.Linq;
using System.IO;

namespace Cloudoman.Agent
{
    using System.Diagnostics;

    public class RemoteCommand
    {
        public string Url { get; set; }
        public ProcessStartInfo ProcessInfo { get { return ExecutionString(); } }
        
        public string Execute()
        {
            var process = Process.Start(ProcessInfo);
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }


        ProcessStartInfo ExecutionString()
        {

            // Create temp folder if required
            var tempFolder = Environment.GetEnvironmentVariable("USERPROFILE") + @"\webscripts";
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            // Download file from URL
            var scriptName = Url.Split('/').Last();
            var extention = scriptName.Split('.').Last();
            var downloadedFile = tempFolder + "\\" + scriptName;

            Console.WriteLine("Downloading file: {0}",downloadedFile);
            new System.Net.WebClient().DownloadFile(new Uri(Url), downloadedFile);
            Console.WriteLine("Downloading Complete: {0}", downloadedFile);

            

            // Use file extension to determine execution host
            string fileName, arguments;

            switch (extention.ToLower())
            {
                case "exe":
                    fileName = downloadedFile;
                    arguments = "";
                    break;

                case "ps1":
                    fileName = "powershell.exe";
                    arguments = downloadedFile;
                    break;

                case "cmd":
                case "bat":
                    fileName = "cmd.exe";
                    arguments = "/C " + downloadedFile;
                    break;
                default:
                    fileName = "powershell.exe";
                    arguments = "-command {'Sorry, cannot determine how to run " + downloadedFile + "'";
                    break;
            }

            // Create process start info for downloaded file
            var processStartInfo = new ProcessStartInfo {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
            };
                                   
            return processStartInfo;
        }
    }


}