using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalonManager
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void cmdStart_Click(object sender, EventArgs e)
        {

        }

        public void RefreshFormConponentsState()
        {
            // display new configuration
            cmbPrinters.Text = Config.PrinterName;
            txtPrinterPort.Text = Config.PrintServerPort;
            txtServerPort.Text = Config.WebServerPort;
            cmdDrawers.Text = Config.DrawerPortName;
            
            // change state of button
            if (ServerController.isPrintServerStarted)
                btnStartServer.Text = "&Stop Server";
            else
                btnStartServer.Text = "&Start Server";

            btnLaunchPOS.Enabled = ServerController.isWebServerStarted;
            cmbPrinters.Enabled = !ServerController.isPrintServerStarted;
            cmdDrawers.Enabled = !ServerController.isPrintServerStarted;
            txtServerPort.Enabled = !ServerController.isPrintServerStarted;
            txtPrinterPort.Enabled = !ServerController.isPrintServerStarted;
        }
    }
}
