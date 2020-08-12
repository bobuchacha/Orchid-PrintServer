using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SM_Lib;
using SM_Lib.SMWebSocket;

/*
 * USING SMWebSocketServer from SM_Lib is deprecated and should be removed
 */ 

namespace SalonManager
{
    class PrintServerController: ServerController
    {
        public static void onMessageReceived(Server server, Client client, string message)
        {
            Debug.WriteLine("NEW MESSAGE RECEIVED: " + message);
            SMCommand receivedCommand = ParseCommand(ref message);

            switch (receivedCommand.cmdType())
            {
                // this is a request command, send the request to all clients
                // connected to server. Except the sender
                 case SMCommandType.SM_COMMAND_REQUEST:
                   
                    foreach (Client _client in server.ClientCollection)
                        if (client.getId() != _client.getId())
                        {
                            _client.SendMessage("@REQUEST " + receivedCommand.cmdData);
                            Debug.WriteLine("\nSending Request back to " + _client.getId() );
                        }
                           
                   
                    break;


                // this is when a client response to a request from another client,
                // this response will be sent to all connected client except the responder
                case SMCommandType.SM_COMMAND_RESPONSE:
                    foreach (Client _client in server.ClientCollection)
                        if (client.getId() != _client.getId())
                            _client.SendMessage("@RESPONSE " + receivedCommand.cmdData);
                    break;

                // transmit JSON to all connected clients except the sender
                case SMCommandType.SM_COMMAND_TRANSMIT_JSON:
                    foreach (Client _client in server.ClientCollection)
                        if (client.getId() != _client.getId())
                            _client.SendMessage("@JSON " + receivedCommand.cmdData);
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
                    Logger.getInstance().write("\n@PRINT64");
                    break;

                case SMCommandType.SM_COMMAND_REGISTER_SERVER:
                    //ServerController.Servers.Add(client);
                    Logger.getInstance().write("- Register client [" + client.getId() + "] as Server");
                    break;

                case SMCommandType.SM_COMMAND_REGISTER_CLIENT:
                    //ServerController.Clients.Add(client);
                    Logger.getInstance().write("- Register client [" + client.getId() + "] as Client");
                    break;

                default:
                    Logger.getInstance().write("\n[Err] Invalid command: " + receivedCommand.cmdName);
                    break;
            }
        }

        public static string Base64Decode(string input)
        {
            byte[] data = System.Convert.FromBase64String(input);
            return System.Text.ASCIIEncoding.ASCII.GetString(data);
        }

        public static void onServerStartError(Server server, SocketException e)
        {
            MessageBox.Show("An error occurred when starting Print Server.\n " +
                "Port " + Config.PrintServerPort + " already in used.\n" +
                "Please change Printer Port in Configuration Panel and try again"
                , "Print Server Cannot Start"
                , MessageBoxButtons.OK
                , MessageBoxIcon.Error);
            ServerController.isPrintServerStarted = false;
        }
        public static void  OnClientConnected(Object sender, ref Client client)
        {
            client.SendMessage("SALON MANAGER COMMUNICATION SERVER 2019 version " + Program.AppVersion);
            client.SendMessage("@flag Base64=true");
            client.SendMessage("@flag Json=true");
           
        }
        public static void onNewClientConnected(Server server, SM_Lib.SMWebSocket.Client client)
        {
            Logger.getInstance().write("\nNew client connected to server[" + server.getId() + "] - client id: " + client.getId());
            client.SendMessage("SALON MANAGER COMMUNICATION SERVER 2019 version " + Program.AppVersion);
            client.SendMessage("@flag Base64=true");
            client.SendMessage("@flag Json=true");
        }

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

            if (firstlinePosition != -1) { 
                first = szInput.Substring(0, firstlinePosition).Trim();
                data = szInput.Substring(firstlinePosition + 1, szInput.Length - 2 - firstlinePosition).Trim();
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
    }
}
