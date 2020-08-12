using SM_Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace SalonManager
{
    class WebServerController: ServerController
    {
        public const string WEBROOT_VERSION_CHECK_URL = "http://pos.salonmanager.us/public/VERSION_CHECK.txt";

        private static string BaseDirectory;
        private static string CacheDirectory;
        private static string CurrentBuildNumber;
        private static string newBuildNumber;
        private static string BuildFileName;
        private static FormUpgradeStatus frmDownloader;
        private static FormUpgradePrompt frmUpgradePrompt;
        private static SMFileServer WebServer;



        /// <summary>
        /// Check for Webroot Updates and show the prompt to update
        /// </summary>
        /// <returns></returns>
        public static Boolean CheckForUpdates()
        {
            WebClient downloadAgency = new WebClient();
            try
            {
                Stream response = downloadAgency.OpenRead(WEBROOT_VERSION_CHECK_URL);
                StreamReader streamReader = new StreamReader(response);

                // get base and cache directory
                BaseDirectory = Directory.GetCurrentDirectory();
                CacheDirectory = BaseDirectory + "\\cache";

                // get ignore build number
                CurrentBuildNumber = Config.WebRootBuildNumber;
                //string ignoredBuildNumber = Config.IniFile.Read("IgnoreBuild", Config.INI_SERVER_SECTION);
                string ignoredBuildNumber = Config.INI.GetSetting(Config.INI_SERVER_SECTION, Config.INI_KEY_IGNORE_BUILD_NUMBER);

                // set default build number for comparison
                CurrentBuildNumber = CurrentBuildNumber != "" ? CurrentBuildNumber : "00000000";

                // read first lin eof response from the client
                newBuildNumber = streamReader.ReadLine();

                // read second line for file name
                BuildFileName = streamReader.ReadLine();

                // version info
                string versionInfo = streamReader.ReadToEnd();

                // close the response stream
                response.Close();

                // show dialog
                if (Convert.ToDouble(newBuildNumber) > Convert.ToDouble(CurrentBuildNumber) && ignoredBuildNumber != newBuildNumber)
                {
                    frmUpgradePrompt = new FormUpgradePrompt();
                    frmUpgradePrompt.lblInfo.Text = "Installed POS Build: " + CurrentBuildNumber + "\n" +
                        "Available POS Build: " + newBuildNumber + "\n" +
                        "Change log: \n" + versionInfo;

                    // button ignore build handler
                    frmUpgradePrompt.btnDoitLater.Click += (s, e) =>
                    {
                        //Config.IniFile.Write("IgnoreBuild", newBuildNumber);
                        Config.INI.AddSetting(Config.INI_SERVER_SECTION, Config.INI_KEY_IGNORE_BUILD_NUMBER, newBuildNumber);
                        Config.INI.SaveSettings();
                    };

                    // do it later button handler
                    frmUpgradePrompt.btnDoitLater.Click += (s, e) =>
                    {
                        frmUpgradePrompt.Hide();
                        frmUpgradePrompt.Dispose();
                    };

                    frmUpgradePrompt.btnUpgradeNow.Click += (s, e) => { UpgradeWebroot(); };

                    // show the form
                    frmUpgradePrompt.Show();

                    // return the true
                    return true;
                }
            }
            catch(Exception e)
            {
                return false;
                //MessageBox.Show(e.Message, "Server Error", )
            }

           

            return false;
        }



        /// <summary>
        /// Do the Webroot upgrade bu showing frmDownloader, and download the base file
        /// </summary>
        private static void UpgradeWebroot()
        {
            try
            {
                // close the prompter
                frmUpgradePrompt.Hide();
                frmUpgradePrompt.Dispose();

                // show the downloader
                frmDownloader = new FormUpgradeStatus();
                frmDownloader.DownloadURI = new Uri("http://pos.salonmanager.us/public/" + BuildFileName);
                frmDownloader.SaveTarget = BaseDirectory + "\\Base.pkg";
                frmDownloader.onDownloadComplete += client_DownloadFileCompleted;
                frmDownloader.DownloadAsync();
                
                //Thread downloadThread = new Thread(()=>{
                //    WebClient downloadClient = new WebClient();
                //    downloadClient.DownloadProgressChanged += client_DownloadProgressChanged;
                //    downloadClient.DownloadFileCompleted += client_DownloadFileCompleted;
                //    downloadClient.DownloadFileAsync(new Uri("http://pos.salonmanager.us/public/" + BuildFileName), BaseDirectory + "\\Base.pkg");
                //});
                //downloadThread.Start();

                //Dim thread As Thread = New Thread(Sub()
                //                                  Dim client As WebClient = New WebClient()
                //                                  AddHandler client.DownloadProgressChanged, New DownloadProgressChangedEventHandler(AddressOf client_DownloadProgressChanged)
                //                                  AddHandler client.DownloadFileCompleted, New AsyncCompletedEventHandler(AddressOf client_DownloadFileCompleted)
                //                                  client.DownloadFileAsync(New Uri("http://pos.salonmanager.us/public/" & BuildFileName), BaseDirectory & "\Base.pkg")

                //                              End Sub)



            }
            catch(Exception e)
            {
                Logger.getInstance().write(e.ToString());
                Debug.WriteLine(e.ToString());
            }
        }


        /// <summary>
        /// Event Handler: called when download completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void client_DownloadFileCompleted(Object sender, EventArgs e)
        {
            frmDownloader.BeginInvoke((MethodInvoker)delegate
            {
                frmDownloader.ProgressBar1.Value = 66;
                frmDownloader.lblProgress.Text = "Download Completed. Deploying...";

      

                // extract file
                // in a new thread
                ThreadStart unzipping = () =>
                {
                    if (Directory.Exists(CacheDirectory))
                    {
                        DirectoryInfo di = new DirectoryInfo(CacheDirectory);
                        foreach (DirectoryInfo diChild in di.GetDirectories())
                            TraverseDirectory(diChild);

                        CleanAllFilesInDirectory(di);
                    }

                    // create new cache directory
                    Directory.CreateDirectory(CacheDirectory);
                    
                    ZipFile.ExtractToDirectory(BaseDirectory + "\\Base.pkg", CacheDirectory);
                };
                unzipping += () =>
                {
                    // this function called when the aboved thread finishes
                    // write information to INI
                    CurrentBuildNumber = newBuildNumber;
                    Config.WebRootBuildNumber = CurrentBuildNumber;
                    Config.Save();
                    
                    frmDownloader.BeginInvoke((MethodInvoker)delegate {
                        frmDownloader.ProgressBar1.Value = 100;
                        frmDownloader.Hide();
                        frmDownloader.Dispose();
                    });

                    //RestartWebServer();
                };

                Thread unzipThread = new Thread(unzipping);
                unzipThread.Start();

   
                
            });
        }









        /// <summary>
        /// Returns true if Webroot cache installed and false otherwise
        /// </summary>
        /// <returns></returns>
        public static Boolean IsWebrootInstalled()
        {
            //return CurrentBuildNumber != "00000000";
            return Directory.Exists(CacheDirectory);
        }












        /// <summary>
        /// Starts Web Server and notify user.
        /// This method does not check for webroot installation in Cache folder
        /// make sure you check for webroot installation by using IsWebrootInstalled()
        /// method with a true returned before calling this function to prevent any error
        /// </summary>
        public static void StartWebServer()
        {
            return;
            //WebSocketSharp.Server.HttpServer a = new WebSocketSharp.Server.HttpServer(port: Convert.ToInt32(Config.WebServerPort));
            //a.DocumentRootPath = CacheDirectory;
            //a.Start();
            string prefixes = "http://localhost:" + Config.WebServerPort + "/";
            WebServer = new SMFileServer(CacheDirectory, prefixes);

            
            TrayIcon.NotifyUser("Web Server Started", "Web Server started and ready to use.");
            isWebServerStarted = true;
        }

        





        /// <summary>
        /// Stops the web server by destroying it
        /// </summary>
        public static void StopWebServer()
        {
            if (isWebServerStarted && WebServer != null) WebServer.Dispose();
            isPrintServerStarted = false;
        }








        /// <summary>
        /// Basically a combination of StopWebServer() and StartWebServer()
        /// </summary>
        public static void RestartWebServer()
        {
            StopWebServer();
            StartWebServer();
        }









        /// <summary>
        /// Clean all files in a directory
        /// </summary>
        /// <param name="di"></param>
        private static void CleanAllFilesInDirectory(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles())
            {
                //Read only files can not be deleted, so mark the attribute as 'IsReadOnly = False'
                fi.IsReadOnly = false;
                fi.Delete();

                //'On a rare occasion, files being deleted might be slower than program execution, and upon returning
                //'from this call, attempting to delete the directory will throw an exception stating it is not yet
                //'empty, even though a fraction of a second later it actually is.  Therefore the 'Optional' code below
                //'can stall the process just long enough to ensure the file is deleted before proceeding. The value
                //'can be adjusted as needed from testing and running the process repeatedly.
                System.Threading.Thread.Sleep(50);  //50 millisecond stall (0.05 Seconds)
            }
        }









        /// <summary>
        /// Traverse Directory b and delete all files in Directory and child directories
        /// Then delete the directory itself if empty
        /// </summary>
        /// <param name="di"></param>
        private static void TraverseDirectory(DirectoryInfo di)
        {
            foreach (DirectoryInfo diChild in di.GetDirectories())
                TraverseDirectory(diChild);
            CleanAllFilesInDirectory(di);
            if (di.GetFiles().Count() == 0) di.Delete();
        }

    }
}
