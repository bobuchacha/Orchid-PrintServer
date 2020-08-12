using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonManager
{
    class MyLogger: WebSocketSharp.Logger
    {
        private static MyLogger _instance;
        public static string Data { get; set; }

        private MyLogger()
            : base()
        {
            
        }
        private static MyLogger _getInstance()
        {
            if (_instance == null)
            {
                _instance = new MyLogger();
                _instance.Output = defaultLogHandler;
            }
            return _instance;
        }
        public static void SetLevel(WebSocketSharp.LogLevel level)
        {
            _getInstance().Level = level;
        }
        public static void SetLogHandler(Action<WebSocketSharp.LogData, string> handler)
        {
            _getInstance().Output = handler;
        }
        private static void defaultLogHandler(WebSocketSharp.LogData logdata, string filePath)
        {
            Data += logdata.ToString() + "\n";
        }

        public static string GetData()
        {
            return Data;
        }
        public static void ClearLog()
        {
            Data = "";
        }
        public static MyLogger GetInstance()
        {
            return _getInstance();
        }
        public static void error(string s) { _getInstance().Error(s); }
        public static void info(string s) { _getInstance().Info(s); }
        public static void fatal(string s) { _getInstance().Fatal(s); }
        public static void debug(string s) { _getInstance().Debug(s); }
        public static void trace(string s) { _getInstance().Trace(s); }
        public static void warn(string s) { _getInstance().Warn(s); }
        
    }
}
