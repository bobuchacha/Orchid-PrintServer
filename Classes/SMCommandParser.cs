using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace SalonManager.Classes
{
    internal enum SMCommandTypeEx
    {
        NONE,               // Default - null type
        REQUEST,            // Request
        RESPONSE,           // Response
        ECHO                // Echo - only used to test print server connectivity
    }



    class SMCommandParser
    {
        public SMCommandTypeEx CommandType { get; set; }
        public string TargetMachineID { get; set; }
        public string Data { get; set;  }

        public SMCommandParser(string Input)
        {
            string TestPattern = @"^\@(.*?)\s(\{(.*?)\}\s)?([\s\S]*)$";
            Regex re = new Regex(TestPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Match match = re.Match(Input);
            if (!match.Success) throw (new Exception("Invalid Input Command Data"));

            GroupCollection MatchedGroup = match.Groups;
            if (MatchedGroup.Count != 4) throw (new Exception("Error parsing Command Data"));

            TargetMachineID = MatchedGroup[3].Value;
            Data = MatchedGroup[4].Value;

            switch (MatchedGroup[1].Value.ToLower())
            {
                case "request":
                    CommandType = SMCommandTypeEx.REQUEST;
                    break;
                case "response":
                    CommandType = SMCommandTypeEx.RESPONSE;
                    break;
                case "echo":
                    CommandType = SMCommandTypeEx.ECHO;
                    break;
                default:
                    CommandType = SMCommandTypeEx.NONE;
                    break;

            }

        }
    }
}
