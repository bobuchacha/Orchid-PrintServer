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
    public partial class FormLogViewer : Form
    {
        int logTextBoxLeft;
        int logTextBoxTop;

        public FormLogViewer()
        {
            InitializeComponent();
        }

        private void FormLogViewer_Load(object sender, EventArgs e)
        {
            logTextBoxLeft = txtLog.Location.X;
            logTextBoxTop = txtLog.Location.Y;
            SetTextBoxDimension();
            txtLog.Text = Program.Log;
            this.Resize += (s2, e2) => { this.SetTextBoxDimension(); };
        }

        private void FormLogViewer_Resize(object sender, EventArgs e)
        {
            SetTextBoxDimension();
        }

        public void SetTextBoxDimension()
        {
            txtLog.Width = this.Width - logTextBoxLeft * 2;        // make padding the same
            txtLog.Height = this.Height - logTextBoxTop - logTextBoxLeft; // make padding the same as left and right
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            Program.ClearLog();
            txtLog.Text = Program.Log;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }
        public void RefreshTextBox()
        {
            txtLog.Text = Program.Log;
        }
    }
}
