using SM_Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonManager
{
    class Config
    {

        // program constants
        public const string INI_FILE_NAME = "PrintServer.ini";
        public const string INI_PRINTER_SECTION = "Printer";
        public const string INI_SERVER_SECTION = "Server";
        public const string SM_DEFAULT_PRINTER_PORT = "8123";
        public const string SM_DEFAULT_WEB_SERVER_PORT = "8080";

        // Ini Key name
        public const string INI_KEY_PRINTER_NAME = "Printer";
        public const string INI_KEY_CASH_DRAWER_NAME = "CashDrawer";
        public const string INI_KEY_PORT = "Port";
        public const string INI_KEY_BUILD_NUMBER = "Build";
        public const string INI_KEY_IGNORE_BUILD_NUMBER = "IgnoreBuild";

        // public config
        public static string PrinterName;
        public static string PrintServerPort;
        public static string WebServerPort;
        public static string DrawerPortName;
        public static string WebRootBuildNumber;
        public static string WebRootIgnoreBuildNumber;

        // public accessible variables
        //public static SMIniFile IniFile;
        public static IniParser INI;

        public static void LoadConfigurations()
        {

            //if (IniFile == null)
            //{
            //    IniFile = new SMIniFile(INI_FILE_NAME);
            //}
            //PrinterName = IniFile.Read(INI_KEY_PRINTER_NAME, INI_PRINTER_SECTION);
            //DrawerPortName = IniFile.Read(INI_KEY_CASH_DRAWER_NAME, INI_PRINTER_SECTION);

            //PrintServerPort = IniFile.Read(INI_KEY_PORT, INI_PRINTER_SECTION);
            //PrintServerPort = PrintServerPort != "" ? PrintServerPort : SM_DEFAULT_PRINTER_PORT;

            //WebServerPort = IniFile.Read(INI_KEY_PORT, INI_SERVER_SECTION);
            //WebServerPort = WebServerPort != "" ? WebServerPort : SM_DEFAULT_WEB_SERVER_PORT;

            //WebRootBuildNumber = IniFile.Read(INI_KEY_BUILD_NUMBER, INI_SERVER_SECTION);
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (INI == null) INI = new IniParser(baseDir + INI_FILE_NAME);

            PrinterName = INI.GetSetting(INI_PRINTER_SECTION, INI_KEY_PRINTER_NAME, "");
            DrawerPortName = INI.GetSetting(INI_PRINTER_SECTION, INI_KEY_CASH_DRAWER_NAME, "");
            PrintServerPort = INI.GetSetting(INI_PRINTER_SECTION, INI_KEY_PORT, SM_DEFAULT_PRINTER_PORT);
            WebServerPort = INI.GetSetting(INI_SERVER_SECTION, INI_KEY_PORT, SM_DEFAULT_WEB_SERVER_PORT);
            WebRootIgnoreBuildNumber = INI.GetSetting(INI_SERVER_SECTION, INI_KEY_IGNORE_BUILD_NUMBER, "");


        }

        /**
         * save configurations to INI file
         */
        public static void Save()
        {
            //IniFile.Write(INI_KEY_PRINTER_NAME, PrinterName, INI_PRINTER_SECTION);
            //IniFile.Write(INI_KEY_CASH_DRAWER_NAME, DrawerPortName, INI_PRINTER_SECTION);
            //IniFile.Write(INI_KEY_PORT, PrintServerPort, INI_PRINTER_SECTION);
            //IniFile.Write(INI_KEY_PORT, WebServerPort, INI_SERVER_SECTION);
            //IniFile.Write(INI_KEY_BUILD_NUMBER, WebRootBuildNumber, INI_SERVER_SECTION);
            INI.AddSetting(INI_PRINTER_SECTION, INI_KEY_PRINTER_NAME, PrinterName);
            INI.AddSetting(INI_PRINTER_SECTION, INI_KEY_CASH_DRAWER_NAME, DrawerPortName);
            INI.AddSetting(INI_PRINTER_SECTION, INI_KEY_PORT, PrintServerPort);
            INI.AddSetting(INI_SERVER_SECTION, INI_KEY_PORT, WebServerPort);
            INI.AddSetting(INI_SERVER_SECTION, INI_KEY_BUILD_NUMBER, WebRootBuildNumber);
            INI.SaveSettings();
        }
    }
}
