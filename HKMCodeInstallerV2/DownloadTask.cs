using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;

namespace HKMCodeInstallerV2
{
    class DownloadingTask
    {
        private static string FILENAME;
        private static WebClient wc = new WebClient();
        public static bool DownloadFile(List<string> files, string host, string username, string password, string savePath)
        {
            try
            {
                //setup FTP client

                foreach (string f in files)
                {
                    FILENAME = f.Split('/').Last();
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    wc.DownloadFileAsync(new Uri(host + f), savePath + f);
                    while (wc.IsBusy)
                        System.Threading.Thread.Sleep(1000);

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        private static void ProgressChanged(object obj, System.Net.DownloadProgressChangedEventArgs e)
        {
            Console.Write("\r --> Downloading " + FILENAME + ": " + string.Format("{0:n0}", e.BytesReceived / 1000) + " kb");

        }

        private static void Completed(object obj, AsyncCompletedEventArgs e)
        {
        }


    }
}
