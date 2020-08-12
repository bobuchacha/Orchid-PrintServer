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
    public struct WSClient
    {
        public WebSocket WebSocket;
        public string machineID;
        public string clientID;
    }
    class Communication : WebSocketBehavior
    {
        private string _machineId;
        private string _prefix;

        // static fields
        private static int _number = 0;
        private static Dictionary<string, WSClient> WSClientCollection;

        #region Construction
        // constructor with no arguments
        public Communication() : this(null){ }
        public Communication(string prefix)
        {
            _prefix = !prefix.IsNullOrEmpty() ? prefix : "client#";
        }
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
            }else
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
        private string getMachineId()
        {
            var name = Context.QueryString["mid"];
            return !name.IsNullOrEmpty() ? name : _prefix + getNumber();
        }




        /// <summary>
        /// get Increment number for ID'ing the sequence of connected client
        /// </summary>
        /// <returns></returns>
        private static int getNumber()
        {
            return Interlocked.Increment(ref _number);
        }
        #endregion




        #region Events 
        protected override void OnOpen()
        {
            _machineId = getMachineId();
            string msg = String.Format("@INFO User Connected: [{0}]", _machineId);
            Sessions.Broadcast(msg);
            Debug.WriteLine(msg);
        }
        protected override void OnClose(CloseEventArgs e)
        {
            string msg = String.Format("@INFO User Disconnected: [{0}]", _machineId);
            Sessions.Broadcast(msg);
            Debug.WriteLine(msg);
            
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            // we will parse command if the first character
            // of the stream is @. Otherwise usually we have nothing
            // to do with the other, just broadcast it or ignore it
            if (e.Data[0] == '@')
            {
                SMCommandParser Command = new SMCommandParser(e.Data);
                switch (Command.CommandType)
                {
                    // if the client sends REQUEST or RESPONSE to Communication Server to broadcast to other
                    // connected clients, we will check if the Command includes target machine ID. If yes, we
                    // will only send that request to targeted machine, otherwise, we will broadcast it to
                    // all connected WS Clients except the sender
                    case SMCommandTypeEx.REQUEST:
                    case SMCommandTypeEx.RESPONSE:
                        
                        if (!Command.TargetMachineID.IsNullOrEmpty())
                        {
                            GetClientWebSocket(Command.TargetMachineID).Send(e.Data);
                        }else
                        {
                            foreach(string ActiveID in Sessions.ActiveIDs)
                            {
                                if (ActiveID != ID)
                                {
                                    Sessions.SendTo(e.Data, ActiveID);
                                }
                            }
                        }
                        break;

                    // if the client wants to check if communication server is connected,
                    // it sends ECHO command to server. the Server will just reply what ever 
                    // it sent
                    case SMCommandTypeEx.ECHO:
                        Context.WebSocket.Send(e.Data);
                        break;
                }
            }
            else
            {
                Debug.WriteLine("Ignored Data Received:\n" + e.Data);
            }
        }


        #endregion

    }
}
