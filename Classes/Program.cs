using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

using SM_Lib;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using System.Reflection;
using System.Drawing.Printing;
using System.IO.Ports;

namespace SalonManager
{

    /*
     * class Program - main program of the Application
     * this is a singleton application, so everything in here should be static
     */
    class Program
    {
        // constants
        private const string CHROME_x86_PATH = "c:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
        private const string CHROME_x64_PATH = "c:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

        // private variables
        private static string   _interprocessID;
        private static Mutex    _appSingletonMaker;
        
        // public accessible variables
        public static string AppVersion;
        public static string Log { get; set; }



        #region Main Entry of Program
        /*********************************************************************************************
         * MAIN ENTRY OF PROGRAM
         * *********************************************************************************************
         */

        static void output(WebSocketSharp.LogData data, string path)
        {
            MessageBox.Show(data.ToString());
            MessageBox.Show(path);
        }

        [STAThread]
        static void Main(string[] args)
        {

            // get the GUID of the assembly, and initialize the Mutex using that GUID
            _interprocessID = Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>().Value.ToUpper();
            _appSingletonMaker = new Mutex(true, _interprocessID);

            // Application Instance has not initialized yet. We initialize the server
            if (_appSingletonMaker.WaitOne(TimeSpan.Zero, true))
                InitializeApplication(args);                        // initialize Program
            else
                SendArgumentsToInitializedInstance(args);           // send arguments to running instance


        }
        #endregion

        #region Send Arguments To Initialized Program Instance
        /*
         * this method will be called when Application is call when another instance 
         * already initialized in memory and running.
         * 
         * it will send the arguments to the previous initialized instance for processing
         */
        private static void SendArgumentsToInitializedInstance(string[] args)
        {
            if (args.Length > 0)
            {
                using (var client = new NamedPipeClientStream(_interprocessID))
                {
                    client.Connect();
                    var writer = new StreamWriter(client);
                    using (var reader = new StreamReader(client))
                    {
                        string arg = String.Join("\t", args);
                        writer.WriteLine(arg);
                        writer.Flush();
                        reader.ReadLine();
                    }
                }
            }
        }
        #endregion

        #region Initialize Application - to be called from Main Entry to initialize first program instance
        /*
         * This method is called when program is executed and no othe rinstances is running before
         * to prevent port conflicting, etc. It will do all initialization and start server itself
         * if all configuration is set.
         * 
         * otherwise it will show configuration form (for the first time)
         */
        private static void InitializeApplication(string[] args)
        {
            Debug.WriteLine("Application initalization");
            /*
            * This task will act like a Mutex server to listen to any incoming 
            * arguments when user launch the app again with argument. 
            */

            Task.Run(() =>
            {
                using (var server = new NamedPipeServerStream(_interprocessID))
                {
                    using (var reader = new StreamReader(server))
                    {
                        using (var writer = new StreamWriter(server))
                        {

                            // this infinity loop is to receive any thing from the other instance
                            while (true)
                            {
                                server.WaitForConnection();
                                var incomingArgs = reader.ReadLine().Split('\t');
                                server.Disconnect();
                                ProgramArgumentsHelper.HandleArguments(incomingArgs);
                            }
                        }
                    }
                }
            });



            // Enable Visual Style
            Application.EnableVisualStyles();

            // release the Mutex
            _appSingletonMaker.ReleaseMutex();

            // get App version
            AppVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

            // handle the initialize argument
            // since this program will start without any required arguments, we just gonna
            // comment it out. Or just leave it. The Handler will do nothing anyway
            ProgramArgumentsHelper.HandleArguments(args);

            // initialize URI schema, by reregistering the URI schema to the registry
            URISchemaHelper.InitializeURISchema();

            // initialize Logger
            var logger = Logger.getInstance();
            Log = "";
            // Add logger Handler here
            logger.onNewEntry += (s) => {
                string NewLogEntry = "";
                NewLogEntry = "\n[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] " + s;
                Log += NewLogEntry + "\n";
                Debug.WriteLine(s);
            };

            // Initialize Configuration
            Config.LoadConfigurations();

            // initialize ServerController
            ServerController.Initialize();


            // check for auto start or show configuration
            if (Config.PrinterName != "" && ServerController.isPrintServerStarted==false)
            {
                ServerController.StartPrintServer();
            } else
            {
                ShowConfiguration();
            }

            // check for web root update
            //CheckForWebrootUpdates();


            if (WebServerController.IsWebrootInstalled())
                WebServerController.StartWebServer();

            // this method will terminate initializing script here
            // all scripts after this line will be execute on application.exit() call
            InitializeTrayIconEvents();
            TrayIcon.InitializeTrayIcon();
            
        }
        #endregion

        #region Check for Webroot upgrade
        /**
         * check for web root updates
         */
        public static void CheckForWebrootUpdates()
        {
            WebServerController.CheckForUpdates();
        }
        #endregion

        #region Initialize Tray Icons Events
        private static void InitializeTrayIconEvents()
        {
            TrayIcon.AddEventHandler(TrayIcon.EventType.ShowMainWindow, (s, e) => { ShowConfiguration(); });
            TrayIcon.AddEventHandler(TrayIcon.EventType.DoubleClick, (s, e) => { ShowConfiguration(); });
            TrayIcon.AddEventHandler(TrayIcon.EventType.About, (s, e) => { ShowAbout(); });
            TrayIcon.AddEventHandler(TrayIcon.EventType.CheckUpdates, (s, e) => {
                if (WebServerController.CheckForUpdates() == false)
                {
                    MessageBox.Show("You are running the latest version of Point of Sale.");
                }
            });
            TrayIcon.AddEventHandler(TrayIcon.EventType.Log, (s, e) =>
            {
                ShowLog();
            });
        }
        #endregion

        #region Show About Dialog
        public static void ShowLog()
        {
            FormLogViewer frmLog = new FormLogViewer();
            Logger.onNewEntryEventHandler extendTextBoxLive = (string msg) => {
                frmLog.RefreshTextBox();
            };

            Logger logger = Logger.getInstance();
            logger.onNewEntry += extendTextBoxLive;

            frmLog.Disposed += (s3, e3) => {
                logger.onNewEntry -= extendTextBoxLive;
            };
            frmLog.Show();
        }
        public static void ShowAbout()
        {
            FormAbout frmAbout = new FormAbout();
            frmAbout.lblVersion.Text = "Version: " + AppVersion;
            frmAbout.Show();
        }
        #endregion

        #region Clear Log
        public static void ClearLog()
        {
            Log = "";
        }
        #endregion

        #region Show Configuration Form
        /*
         * show frmMain
         */
        public static void ShowConfiguration()
        {
            // initialize new FormConfig
            FormConfig frmConfig = new FormConfig();

            /* 
             * event handler for btn StartServer
             * first check for configurations and start server
             */
            frmConfig.btnStartServer.Click += (s, e) =>
            {

                
                if (ServerController.isPrintServerStarted == false)
                {
                    /*
                     * if server is not started, check for printer name
                     * if printer name is not set,, stop and let user know
                     * otherwise, start the server, and change the text of the 
                     * control button
                     */
                    if (frmConfig.cmbPrinters.Text == "")
                    {
                        MessageBox.Show("Please select Receipt Printer to continue."
                            , "Receipt Printer Not Configured"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Error);
                    }
                    else if (frmConfig.txtPrinterPort.Text == "")
                    {
                        MessageBox.Show("Please set your print server port. Default is 8123."
                            , "Print Server Port not set"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Error);
                    }
                    else if (frmConfig.txtServerPort.Text == "")
                    {
                        MessageBox.Show("Please set your web server port. Default is 8000."
                            , "Web Server Port not set"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Error);
                    }
                    else
                    {
                        Config.PrinterName = frmConfig.cmbPrinters.Text;
                        Config.PrintServerPort = frmConfig.txtPrinterPort.Text;
                        Config.WebServerPort = frmConfig.txtServerPort.Text;
                        Config.Save();
                        ServerController.StartPrintServer();
                        frmConfig.RefreshFormConponentsState();
                    }
                }
                else
                {
                    /*
                     * if server is started, stop the server and change the Txt of command
                     * button
                     */
                    ServerController.StopPrintServer();
                    frmConfig.RefreshFormConponentsState();
                }
            };

            // event handler for btn Launch POS
            frmConfig.btnLaunchPOS.Click += (s, e) =>
            {
                if (ServerController.isWebServerStarted)
                    LaunchChromeBrowser();
                else
                    MessageBox.Show("Local POS is not started. Please restart the program, and download new version of POS and try again."
                        , "POS Not Started"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
            };

            // event handler for btn minimize
            frmConfig.btnMinimize.Click += (s, e) =>
            {
                frmConfig.Hide();
                frmConfig.Dispose();
            };

            // init the combo boxes
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                frmConfig.cmbPrinters.Items.Add(printerName);
            }
            foreach (string port in SerialPort.GetPortNames())
            {
                frmConfig.cmdDrawers.Items.Add(port);
            }

            frmConfig.RefreshFormConponentsState();

            // show the form
            frmConfig.Show();
        }
        #endregion

        #region Launch Chrome Browser
        /*
         * Check for installed Chrome version and start the Website
         */
        public static void LaunchChromeBrowser()
        {
            string szChromeParams = " --app=http://localhost:" + Config.WebServerPort;

            if (File.Exists(CHROME_x86_PATH))
            {
                Process.Start(CHROME_x86_PATH, szChromeParams);
            }else if (File.Exists(CHROME_x64_PATH))
            {
                Process.Start(CHROME_x64_PATH, szChromeParams);
            }else
            {
                MessageBox.Show("Oops! It looks like your computer does not have Google Chrome installed. " +
                           "This software Is optimized to run on Google Chrome, pease visit chrome.google.com" +
                           "to download the newest version, then visit http://localhost:" + Config.WebServerPort + 
                           " to start your Point of Sale app", "Google Chrome Not Installed"
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Exclamation);
            }
        
        }
        #endregion
    }
}
