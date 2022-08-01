using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace HKMCodeInstallerV2
{
   

    class Program
    {
        const int HWND_BROADCAST = 0xffff;
        const uint WM_SETTINGCHANGE = 0x001a;
        const string hkmArchive = @"HkmCode/archive/refs/tags/V0.1.zip";
        private static readonly string hkmCode = "HkmCode-0.1.zip";
        private static  string version = ComputerHash(hkmArchive);
        private static string tmpfolder = Path.Combine(Path.GetTempPath(), version);
        private static readonly string hkmCodeTmpFile = Path.Combine(tmpfolder, hkmCode);
        private static List<string> wrkfolders = new List<string>();
        private static string configFile = ".hkmcode";

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam);

        private static string ComputerHash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for(int i = 0; i<bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();

            }
        }
        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.Blue;
            var text = @"
                           _   _ _  ____  __  ____          _      ____  _   _ ____  
                          | | | | |/ /  \/  |/ ___|___   __| | ___|  _ \| | | |  _ \ 
                          | |_| | ' /| |\/| | |   / _ \ / _` |/ _ \ |_) | |_| | |_) |
                          |  _  | . \| |  | | |__| (_) | (_| |  __/  __/|  _  |  __/ 
                          |_| |_|_|\_\_|  |_|\____\___/ \__,_|\___|_|   |_| |_|_|    
                                                                                    
                          ___           _        _ _                           ___        _ 
                         |_ _|_ __  ___| |_ __ _| | | ___ _ __   __   __      / _ \      / |
                          | || '_ \/ __| __/ _` | | |/ _ \ '__|  \ \ / /     | | | |     | |
                          | || | | \__ \ || (_| | | |  __/ |      \ V /   _  | |_| |  _  | |
                         |___|_| |_|___/\__\__,_|_|_|\___|_|       \_/   (_)  \___/  (_) |_|
                                                                   
                                                                                                                                             
                    ";


            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Green;


            Console.Title = "HkmCode Installer";
            RunAsync();
        }

        static void CRTFolWorkers()
        {
            wrkfolders.Add(string.Format(@"C:\Users\{0}\AppData\Roaming\HKMCode\php", "Default"));
            foreach (string user in GetUserAccounts()) wrkfolders.Add(string.Format(@"C:\Users\{0}\AppData\Roaming\HKMCode\php", user));
            wrkfolders.ForEach(Crwkfolders);

            CreatingHKMCodePath();
        }

        static void Crwkfolders(string path)
        {
            if (!isDirExist(path)) Directory.CreateDirectory(path);
            string fileConfHkm = Path.Combine(path, configFile);
            if (!File.Exists(fileConfHkm)) File.Create(fileConfHkm);

            if (File.Exists(hkmCodeTmpFile))Helpers.ZipHelpers.Unzip(hkmCodeTmpFile, path);
            
        }

        static void CreatingHKMCodePath()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (isElevated)
            {
                using (var envKey = Registry.LocalMachine.OpenSubKey(
                                @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", true
                                ))
                {
                    Contract.Assert(envKey != null, @"registry key is missing!");
                    var pathValue = envKey.GetValue("Path");

                    bool d = PathToListCheck(pathValue.ToString(), @"C:\HKMCodePHP");

                    if (d) Console.WriteLine(@"C:\HKMCodePHP path exist in path environment manager!");
                    else
                    {
                        envKey.SetValue("Path", pathValue.ToString() + @";C:\HKMCodePHP");
                        SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE, (UIntPtr)0, "Environment");
                        Console.WriteLine(pathValue.ToString() + @";C:\HKMCodePHP");

                    }
                    envKey.Close();
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Run the setup as administrator");
                Console.ReadLine();

            }
        }


        static bool PathToListCheck(string path, string value)
        {
            String[] spearator = { ";" };
            String[] strlist = path.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
            bool returned = false;
            foreach (string s in strlist)
            {
                if (s == value)
                {
                    returned = true;
                    break;
                }
            }
            return returned;
        }

        static List<string> GetUserAccounts()
        {
            List<string> users = new List<string>();
            int EntriesRead;
            int TotalEntries;
            int Resume;

            IntPtr bufPtr;

            NetworkAPI.NetUserEnum(null, 0, 2, out bufPtr, -1, out EntriesRead, out TotalEntries, out Resume);

            if (EntriesRead > 0)
            {
                NetworkAPI.USER_INFO_0[] Users = new NetworkAPI.USER_INFO_0[EntriesRead];
                IntPtr iter = bufPtr;
                for (int i = 0; i < EntriesRead; i++)
                {
                    Users[i] = (NetworkAPI.USER_INFO_0)Marshal.PtrToStructure(iter, typeof(NetworkAPI.USER_INFO_0));
                    iter = (IntPtr)((int)iter + Marshal.SizeOf(typeof(NetworkAPI.USER_INFO_0)));

                    if (isDirExist(string.Format(@"C:\Users\{0}", Users[i].Username)))
                    {
                        users.Add(Users[i].Username);
                    }
                }
                NetworkAPI.NetApiBufferFree(bufPtr);
            }

            return users;
        }

        static bool isDirExist(string path)
        {
            return Directory.Exists(path);
        }



        static void RunAsync()
        {
            List<string> filesList = new List<string>();
            filesList.Add(hkmArchive);

            if (isDirExist(tmpfolder))
            {
                if (File.Exists(hkmCodeTmpFile)) CRTFolWorkers();
                else DownloadingTask.DownloadFile(filesList, "https://github.com/", "", "", tmpfolder);
            }
            else
            {
                Directory.CreateDirectory(tmpfolder);
                DownloadingTask.DownloadFile(filesList, "https://github.com/", "", "", tmpfolder);
                CRTFolWorkers();

            }
            Console.ReadLine();


            



        }





}
}


