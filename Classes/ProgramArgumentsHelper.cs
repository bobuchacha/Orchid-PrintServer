using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SalonManager
{
    class ProgramArgumentsHelper
    {
        public static void HandleArguments(string[] args)
        {
            string arg = String.Join("\t", args);
            Debug.WriteLine("New argument received: " + arg);
            string Pattern = @"^(" + URISchemaHelper.UriScheme + @"\:([\/\>]*)?)?(.+?)(\/)?$";
            Regex ex = new Regex(Pattern);
            Match match = ex.Match(arg);
            arg = match.Groups[3].Value;

            ParseProgramArguments(arg);
        }

        /// <summary>
        /// parse argument parsed to program at launch and call correspondent procedure to handle it
        /// </summary>
        /// <param name="arg"></param>
        private static void ParseProgramArguments(string arg)
        {
            // first argument will be splitted into parts by deliminater [space]
            string[] _argParts = arg.Split(' ');        // argument parts
            int _nextPartTobeProcessed = 0;             // used to point to the next part to be handle

            while (_nextPartTobeProcessed < _argParts.Length-1)
            {
                _nextPartTobeProcessed += ParseProgramArgumentSingle(ref _argParts, _nextPartTobeProcessed);
            }

        }


        /// <summary>
        /// Parse a single argument. If that argument takes another sub argument, it will take it and
        /// return the numbe rof arguments it took.
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static int ParseProgramArgumentSingle(ref string[] parts, int i)
        {
            int _requiredArguments = 0; // how many part we will take to fulfill this command argument
            switch (parts[i].ToLower())
            {
                case "showlog":
                    Program.ShowLog();
                    _requiredArguments = 0;
                    break;

                case "start":
                    if (parts.Length < i + 2) MessageBox.Show("Invalid START command.\nUsage: START SERVER|WEB");
                    if (parts[i + 1].ToLower() == "server" && !PrintServerController.isPrintServerStarted) PrintServerController.StartPrintServer();
                    else if (parts[i + 1].ToLower() == "web" && !WebServerController.isWebServerStarted) WebServerController.StartWebServer();
                    _requiredArguments = 1;
                    break;


                case "stop":
                    if (parts.Length < i + 2) MessageBox.Show("Invalid STOP command.\nUsage: STOP SERVER|WEB");
                    if (parts[i + 1].ToLower() == "server" && PrintServerController.isPrintServerStarted) PrintServerController.StopPrintServer();
                    else if (parts[i + 1].ToLower() == "web" && WebServerController.isWebServerStarted) WebServerController.StopWebServer();
                    _requiredArguments = 1;
                    break;
                    
            }

            return ++_requiredArguments;
        }
    }
}
