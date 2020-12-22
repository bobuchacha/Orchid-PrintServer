using SM_Lib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SM_Lib.SMWebSocket;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SalonManager
{
    class ServerController
    {
        
        public static Boolean isPrintServerStarted = false;
        public static Boolean isWebServerStarted = false;
        public static SMRawPrinterHelper Printer;
        public static SerialPort COM;
        //public static List<Client> Servers = new List<Client>();
        //public static List<Client> Clients = new List<Client>();

        // private variables
        //static Server WebSocketServer;
        static Thread PrintServerThread;
        static WebSocketServer WSSServer;
        static HttpServer MainServer;

        public static void LogInfo(string str)
        {
            WSSServer.Log.Info(str);
        }
        public static void LogDebug(string str)
        {
            WSSServer.Log.Debug(str);
        }
        public static void LogWarn(string str)
        {
            WSSServer.Log.Warn(str);
        }
        public static void LogError(string str)
        {
            WSSServer.Log.Error(str);
        }

        public static void Initialize()
        {
            Printer = new SMRawPrinterHelper();
            COM = new SerialPort();
        }

        public static void StartPrintServer()
        {

            StartRawPrinterService();
            StartCOMPortDrawerService();

            
            // start Web Socket print Server in a new thread
            //PrintServerThread = new Thread(StartWebSocketServer);
            //PrintServerThread.Start();

            //if (PrintServerThread.ThreadState == ThreadState.Running)
            //{
            //    TrayIcon.NotifyUser("Print Server Started", "Print server has been started and ready to take messages");
            //    isPrintServerStarted = true;
            //}

            WSSServer = new WebSocketServer(IPAddress.Any, Convert.ToInt16(Config.PrintServerPort));
            WSSServer.Log.File = "log.txt";
            WSSServer.Log.Level = LogLevel.Debug;

            WSSServer.AddWebSocketService<SalonManager.Classes.WebSocketServerControllers.Printing>("/");                       // default, legacy module
            WSSServer.AddWebSocketService<CommunicationServerBehavior>("/communication");                                       // legacy communication module - acts like a chat server
            WSSServer.AddWebSocketService<SalonManager.Classes.WebSocketServerControllers.RawPrinterDirect>("/raw-printers");   // access to raw printers using ?m=printername
            WSSServer.AddWebSocketService<SalonManager.Classes.WebSocketServerControllers.Communication>("/comm");              // another communication
            WSSServer.AddWebSocketService<SalonManager.Classes.WebSocketServerControllers.Utility>("/utility");                 // utility to access system wise function such as list printers name, etc.
            
            WSSServer.Log.Debug("Server started");

            WSSServer.KeepClean = false;

            try
            {
                WSSServer.Start();
                TrayIcon.NotifyUser("Print Server Started", "Print server has been started and ready to take messages");
                isPrintServerStarted = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Another application is listening to port " + Config.PrintServerPort + ".\nChange your port number and try again", "Print Server can not start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }


        public static void StopPrintServer()
        {
            //WebSocketServer.stopServer();
            //PrintServerThread.Abort();
            isPrintServerStarted = false;
            WSSServer.Stop();
        }


        


        /******************************************* PRIVATE METHODS ******************************************/
        private static void StartWebSocketServer()
        {

            /*
             * WebSocketServer = new Server(IPAddress.Any, Convert.ToInt16(Config.PrintServerPort));
            WebSocketServer.onMessageReceived += PrintServerController.onMessageReceived;
            WebSocketServer.onNewClientConnected += PrintServerController.onNewClientConnected;
            WebSocketServer.onServerStartError += PrintServerController.onServerStartError;
            WebSocketServer.OnClientConnect += PrintServerController.OnClientConnected;
            WebSocketServer.startServer();
            */




        }
        private static void StartRawPrinterService()
        {
            

        }
        private static void StartCOMPortDrawerService()
        {
            string CashDrawerPort = Config.DrawerPortName;
            if (CashDrawerPort.Length > 3)
            {
                // if COM is open, close it
                if (COM.IsOpen) {
                    COM.Close();
                    Thread.Sleep(200);
                }

                COM.PortName = CashDrawerPort;
                COM.BaudRate = 9600;
                COM.DataBits = 8;
                COM.Parity = System.IO.Ports.Parity.None;
                COM.StopBits = System.IO.Ports.StopBits.One;
                COM.Handshake = System.IO.Ports.Handshake.RequestToSend;
                COM.ReadTimeout = 2000;
                COM.WriteTimeout = -1;
                COM.NewLine = "\n";
                COM.ReadBufferSize = 12;
                COM.ReceivedBytesThreshold = COM.ReadBufferSize;

                try
                {
                    COM.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                    
                

            }
        }
    }
}
