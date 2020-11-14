using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace SalonManager.Classes.WebSocketServerControllers
{
  
    class RawPrinterDirect : WebSocketBehavior
    {
        private string _printerName;

        // static fields
        private static int _number = 0;
        private static Dictionary<string, WSClient> WSClientCollection;

        #region Construction
        // constructor with no arguments
        public RawPrinterDirect() : this(null) { }
        public RawPrinterDirect(string prefix){}
        #endregion



        #region STATIC METHODS
        /// <summary>
        /// Add Connected WebSocket Client Object to Client Collection for further communication
        /// </summary>
        /// <param name="machineID">MachineID sent during connection, aquired using QueryString mid</param>
        /// <param name="SessionID">Session ID of Websocket Client</param>
        /// <param name="ws">WebSocket Object</param>
        protected static void AddClient(string machineID, string SessionID, WebSocket ws)
        {
            WSClient newESClient = new WSClient();
            newESClient.machineID = machineID;
            newESClient.clientID = SessionID;
            newESClient.WebSocket = ws;
            WSClientCollection.Add(machineID, newESClient);
            ServerController.LogDebug("New client Added. Client ID=" + machineID + ". Session ID=" + SessionID);
        }




        /// <summary>
        /// Return WebSocket Object used to communicate to client with machineID
        /// </summary>
        /// <param name="machineID">machineID of client used to search the WebSocket</param>
        /// <returns>WebSocket object if found using machineID, null otherwise</returns>
        protected static WebSocket GetClientWebSocket(string machineID)
        {
            if (WSClientCollection.ContainsKey(machineID))
            {
                return WSClientCollection[machineID].WebSocket;
            }
            else
            {
                return null;
            }
        }




        /// <summary>
        /// Return SessionID of client with machineID
        /// </summary>
        /// <param name="machineID"></param>
        /// <returns></returns>
        protected static string GetClientSessionID(string machineID)
        {
            if (WSClientCollection.ContainsKey(machineID))
            {
                return WSClientCollection[machineID].clientID;
            }
            else
            {
                return null;
            }
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Get MachineID during connection set by QueryString mid
        /// </summary>
        /// <returns></returns>
        private string getPrinterName()
        {
            return Context.QueryString["mid"];
        }
        #endregion


        #region Events
        protected override void OnOpen()
        {
            _printerName = getPrinterName();
            this.Send("Welcome to Salon Orchid Raw Printer Assistant. Your printer name is " + _printerName);
            ServerController.LogDebug("New RawPrinterDirect established to " + _printerName);

        }
        protected override void OnClose(CloseEventArgs e)
        {
            ServerController.LogDebug("Session closed. Printer name was " + _printerName);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            bool opened = ServerController.Printer.OpenPrint(_printerName);
            if (!opened)
            {
                ServerController.LogWarn("Print failed. Printer " + _printerName);
                this.Send("failed");
                return;
            }


            // send data to printer
            if (e.IsBinary)
            {
                Debug.WriteLine("Sending binary to printer " + _printerName);
                ServerController.Printer.SendToPrinter(_printerName, e.RawData, e.RawData.Length);
            }
            else
            {
                Debug.WriteLine("Sending text to printer " + _printerName);
            
                bool printed = ServerController.Printer.SendStringToPrinter(_printerName, SMCommand.ESC_INIT + e.Data);
                
               
            }

            ServerController.Printer.ClosePrint();
            this.Send("Success");
            ServerController.LogInfo("Print successfully. Printer " + _printerName);
        }


        #endregion

    }
}
