using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SalonManager
{

    class CommunicationServerBehavior : WebSocketBehavior
    {

        private static SMCommand ParseCommand(ref string szInput)
        {
            // TODO: On Error Resume Next Warning!!!: The statement is not translatable 
            //int firstLinePos = szInput.IndexOf("\u000A");
            //string firstline = szInput.Substring(0, firstLinePos).Trim();
            //string data = szInput.Substring((firstLinePos + 1), (szInput.Length - (firstLinePos - 2))).Trim();
            //  -2 to erase send char at end of string

            //StringReader input = new StringReader(szInput);
            //string first = input.ReadLine();
            //string data = input.ReadToEnd();
            //data = data.Substring(0, data.Length-2).Trim();
            string first = ""
                , data = "";
            int firstlinePosition = szInput.IndexOf("\u000A");
            //Debug.WriteLine("First Line POS: " + firstlinePosition);

            if (firstlinePosition != -1)
            {
                first = szInput.Substring(0, firstlinePosition).Trim();
                data = szInput.Substring(firstlinePosition + 1, szInput.Length - 1 - firstlinePosition).Trim();
            }
            else
            {
                first = szInput;
            }

            //Debug.WriteLine("first lineL " + first);
            //Debug.WriteLine("data " + data);

            SMCommand cmdReturn = new SMCommand();
            cmdReturn.cmdName = first;
            cmdReturn.cmdData = data;

            return cmdReturn;

        }

        protected override void OnOpen()
        {
            base.OnOpen();
            SM_Lib.Logger.getInstance().write("\nNew client connected - client id: " + ID);
            WebSocket client = Context.WebSocket;
            client.Send("SALON MANAGER COMMUNICATION SERVER 2019 version " + Program.AppVersion);
            client.Send("@flag Base64=true");
            client.Send("@flag Json=true");

        }







        protected override void OnMessage(MessageEventArgs e)
        {

            Debug.WriteLine("NEW MESSAGE RECEIVED: " + e.Data);
            string message = e.Data;
            SMCommand receivedCommand = ParseCommand(ref message);

            switch (receivedCommand.cmdType())
            {
                // this is a request command, send the request to all clients
                // connected to server. Except the sender
                case SMCommandType.SM_COMMAND_REQUEST:

                    foreach (string _clientId in Sessions.ActiveIDs)
                        if (_clientId != ID)
                        {
                            Sessions.SendTo("@REQUEST " + receivedCommand.cmdData, _clientId);
                            //   Debug.WriteLine("\nSending Request back to " + _clientId);
                        }


                    break;


                // this is when a client response to a request from another client,
                // this response will be sent to all connected client except the responder
                case SMCommandType.SM_COMMAND_RESPONSE:
                    foreach (string _clientId in Sessions.ActiveIDs)
                        if (_clientId != ID)
                        {
                            Sessions.SendTo("@RESPONSE " + receivedCommand.cmdData, _clientId);
                            // Debug.WriteLine("\nSending Response back to " + _clientId);
                        }

                    break;

                // transmit JSON to all connected clients except the sender
                case SMCommandType.SM_COMMAND_TRANSMIT_JSON:
                    foreach (string _clientId in Sessions.ActiveIDs)
                        if (_clientId != ID)
                        {
                            Sessions.SendTo("@JSON " + receivedCommand.cmdData, _clientId);
                            //Debug.WriteLine("\nSending Request back to " + _clientId);
                        }
                    break;

                // send data directly to printer
                case SMCommandType.SM_COMMAND_PRINT:
                    ServerController.Printer.OpenPrint(Config.PrinterName);
                    ServerController.Printer.SendStringToPrinter(Config.PrinterName, SMCommand.ESC_INIT + message);
                    ServerController.Printer.ClosePrint();
                    break;

                // open drawer command
                // also send command to all COM port o fdrawer kicker
                case SMCommandType.SM_COMMAND_OPEN_DRAWER:
                    if (ServerController.COM.IsOpen) ServerController.COM.Write("O");
                    break;

                // print base64 data
                case SMCommandType.SM_COMMAND_PRINT_BASE64:
                    ServerController.Printer.OpenPrint(Config.PrinterName);
                    ServerController.Printer.SendStringToPrinter(Config.PrinterName, SMCommand.ESC_INIT + receivedCommand.cmdData);
                    ServerController.Printer.ClosePrint();
                    SM_Lib.Logger.getInstance().write("\n@PRINT64");
                    break;

                /*case SMCommandType.SM_COMMAND_REGISTER_SERVER:
                    ServerController.Servers.Add(client);
                    SM_Lib.Logger.getInstance().write("- Register client [" + client.getId() + "] as Server");
                    break;

                case SMCommandType.SM_COMMAND_REGISTER_CLIENT:
                    ServerController.Clients.Add(client);
                    Logger.getInstance().write("- Register client [" + client.getId() + "] as Client");
                    break;
                    */
                default:
                    SM_Lib.Logger.getInstance().write("\n[Err] Invalid command: " + receivedCommand.cmdName);
                    break;
            }

        }










        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }










        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            /*MessageBox.Show("An error occurred when starting Print Server.\n " +
                "Port " + Config.PrintServerPort + " already in used.\n" +
                "Please change Printer Port in Configuration Panel and try again"
                , "Print Server Cannot Start"
                , MessageBoxButtons.OK
                , MessageBoxIcon.Error);
            */
            MessageBox.Show("An error occured when starting Print Server.\n" + e.Message, "Print Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ServerController.isPrintServerStarted = false;

        }
    }
}
