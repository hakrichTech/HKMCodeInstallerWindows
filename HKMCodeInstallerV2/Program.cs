using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace HKMCodeInstallerV2
{
    class Program
    {
        const int HWND_BROADCAST = 0xffff;
        const uint WM_SETTINGCHANGE = 0x001a;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam);


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
            foreach (string user in GetUserAccounts())
            {
                string newPath = string.Format(@"C:\Users\{0}\AppData\Roaming\HKMCode\php", user);
                string DefaultPath = string.Format(@"C:\Users\{0}\AppData\Roaming\HKMCode\php", "Default");
                if (!isDirExist(newPath)) Directory.CreateDirectory(newPath);
                if (!isDirExist(DefaultPath)) Directory.CreateDirectory(DefaultPath);

                Console.WriteLine(user);
            }
            Console.ReadLine();
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
            string format = "zip";
            string fileName = "/sample." + format.ToLower();
            filesList.Add(fileName);


            DownloadingTask.DownloadFile(filesList, "http://localhost", "", "", @"C:\");
            //Console.WriteLine("Done");
            Console.ReadLine();



        }


    }
}


