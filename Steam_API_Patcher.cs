using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Steam_API_Patcher
{
    class Program
    {
        static string API_crack_url = "https://github.com/MEMESCOEP/SteamAPIPatcher/raw/main/steam_api.dll";
        static string API64_crack_url = "https://github.com/MEMESCOEP/SteamAPIPatcher/raw/main/steam_api64.dll";
        static ManualResetEvent ma = new ManualResetEvent(false);
        static Uri dl;
        static string dlfn = "";

        public static void DlPG(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("[downloaded {1} of {2} kb. ({3}% complete.)]          \r", (string)e.UserState, e.BytesReceived/1024, e.TotalBytesToReceive/1024, e.ProgressPercentage);
            if(e.ProgressPercentage == 100)
            {
                Console.Write("[downloaded {1} of {2} kb. (100% complete.)]          \r", (string)e.UserState, e.BytesReceived/1024, e.TotalBytesToReceive/1024);
                Console.WriteLine("\n[INFO] >> Done.");
                ma.Set();
            }
        }

        public static void Download()
        {
            using (var client = new WebClient())
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), dlfn)))
                {
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DlPG);
                    client.DownloadFileAsync(dl, dlfn);
                }
                else
                {
                    Console.WriteLine("[WARN] >> File {0} already exists and doesn't need to be downloaded.", Path.Combine(Directory.GetCurrentDirectory(), dlfn));
                    ma.Set();
                }
            }
        }

        static void Main(string[] args)
        {
            List<string> dlls = new List<string>();
            string sDir;
            if(args.Length > 0 && args[0] != null)
            {
                sDir = args[0];
            }
            else
            {
                Console.Write("Enter game path >> ");
                sDir = Console.ReadLine();
            }
            Console.WriteLine("[INFO] >> Searching for 'steam_api.dll' and 'steam_api64.dll'...");
            var extensions = new List<string> { ".dll" };
            string[] files = Directory.GetFiles(sDir, "*.*", SearchOption.AllDirectories).Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0).ToArray();
            foreach (var file in files)
            {
                if (file.Contains("steam_api.dll"))
                {
                    Console.WriteLine("[INFO] >> Found dll file: " + file);
                    dlls.Add(file);
                }
                else if (file.Contains("steam_api64.dll"))
                {
                    Console.WriteLine("[INFO] >> Found dll file: " + file);
                    dlls.Add(file);
                }
            }
            foreach (var item in dlls)
            {
                string itemname = item.Insert(item.LastIndexOf("."), "_backup");
                Console.WriteLine("[INFO] >> Backing up dll file: " + item + " to file: " + itemname);
                if (!File.Exists(itemname))
                {
                    File.Move(item, itemname);
                }
                else
                {
                    Console.WriteLine("\n[INFO] >> Item " + itemname + " is already patched!");
                }
            }
            Console.WriteLine("\n[INFO] >> Downloading API crack...");
            dl = new Uri(API_crack_url);
            dlfn = "steam_api.dll";
            ma.Reset();
            Download();
            ma.WaitOne();

            Console.WriteLine("\n[INFO] >> Downloading API64 crack...");
            dl = new Uri(API64_crack_url);
            dlfn = "steam_api64.dll";
            ma.Reset();
            Download();
            ma.WaitOne();

            Console.WriteLine("\nWaiting 5 seconds for file I/O to finish...");
            Thread.Sleep(5000);
            if (dlls.Count > 0)
            {
                Console.WriteLine("[INFO] >> Copying dlls...");
                try
                {
                    foreach (var file1 in dlls)
                    {
                        if(!File.Exists(Directory.GetParent(file1) + "\\steam_api.dll"))
                        {
                            Console.WriteLine("\n\n[INFO] >> Copying DLL: steam_api.dll to " + Directory.GetParent(file1) + "\\steam_api.dll...");
                            File.Copy("steam_api.dll", Directory.GetParent(file1) + "\\steam_api.dll");
                        }
                        else
                        {
                            Console.WriteLine("\n[INFO] >> File " + (Directory.GetParent(file1) + "\\steam_api.dll") + " has already been patched!");
                        }

                        if (!File.Exists(Directory.GetParent(file1) + "\\steam_api64.dll"))
                        {
                            Console.WriteLine("\n\n[INFO] >> Copying DLL: steam_api.dll to " + Directory.GetParent(file1) + "\\steam_api64.dll...");
                            File.Copy("steam_api.dll", Directory.GetParent(file1) + "\\steam_api64.dll");
                        }
                        else
                        {
                            Console.WriteLine("\n[INFO] >> File " + (Directory.GetParent(file1) + "\\steam_api64.dll") + " has already been patched!");
                        }
                    }
                    Console.WriteLine();
                    for(int i = 0; i < 5; i++)
                    {
                        Console.Write("[INFO] >> Exitting in " + (5-i) + " second(s).\r");
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] >> " + ex.Message);
                    Console.Write("Press any key to continue");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("[INFO] >> No DLLs were found.");
                for (int i = 0; i < 5; i++)
                {
                    Console.Write("[INFO] >> Exitting in " + (5 - i) + " second(s).\r");
                    Thread.Sleep(1000);
                }
            }
            
        }
    }
}
