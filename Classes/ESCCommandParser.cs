using System;
using System.Diagnostics;
using System.Windows.Forms;

enum SMCommandType : int
{

    SM_COMMAND_PRINT,

    //  print information. Default
    SM_COMMAND_REGISTER_SERVER,

    //  register client as Server Application
    SM_COMMAND_REGISTER_CLIENT,

    //  register client as Client Application/Module
    SM_COMMAND_CREATE_TRANSACTION,

    //  create transaction and send to Server
    SM_COMMAND_TRANSMIT_JSON,

    //  send JSON to other connected client
    SM_COMMAND_CLOSE,

    //  close connection
    SM_COMMAND_PRINT_BASE64,

    //  print base64 encoded data
    SM_COMMAND_TRANSMIT_JSON_BASE64,

    //  transmit json in base64 encoded
    SM_COMMAND_OPEN_DRAWER,

    //  ask to open drawer only
    SM_COMMAND_REQUEST,

    //  send a request to server
    SM_COMMAND_RESPONSE,
}
struct SMCommand
{
    const string ESC = "\u001B";
    const string GS = "\u001D";
    const string NULL = "\0";

    public const string ESC_NORMAL_TEXT =  ESC + "!" + NULL;
    public const string ESC_INIT = ESC_NORMAL_TEXT + "\u000A\u001B" + "c6\u0001\u001B" + "R3";

    public SMCommandType cmdType()
    {
        switch (cmdName)
        {
            case "@CLOSE":
                return SMCommandType.SM_COMMAND_CLOSE;
                //break;
            case "@DRAWER_OPEN":
                return SMCommandType.SM_COMMAND_OPEN_DRAWER;
                //break;
            case "@JSON":
                return SMCommandType.SM_COMMAND_TRANSMIT_JSON;
                //break;
            case "@PRINT64":
                // TODO: On Error Resume Next Warning!!!: The statement is not translatable 
                try
                {
                    byte[] tmpData = System.Convert.FromBase64String(cmdData);
                    cmdData = System.Text.Encoding.UTF8.GetString(tmpData);
                }catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
                return SMCommandType.SM_COMMAND_PRINT_BASE64;
                //break;
            case "@REQUEST":
                return SMCommandType.SM_COMMAND_REQUEST;
                //break;
            case "@RESPONSE":
                return SMCommandType.SM_COMMAND_RESPONSE;
                //break;
            case "@JSON64":
                return SMCommandType.SM_COMMAND_TRANSMIT_JSON_BASE64;
                // register server, server is the one who receive request
                //break;
            case "@REGISTER_SERVER":
                return SMCommandType.SM_COMMAND_REGISTER_SERVER;
                //  clients are the one that receive responses
                //break;
            case "@REGISTER_CLIENT":
                return SMCommandType.SM_COMMAND_REGISTER_CLIENT;
                //break;
            default:
                cmdData = (cmdName + ('\n' + cmdData));
                //  consolidate data if it's print command, for backward compatible
                return SMCommandType.SM_COMMAND_PRINT;
                //break;
        }
    }

    public string cmdName;

    public string cmdData;
}