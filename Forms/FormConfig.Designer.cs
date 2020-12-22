namespace SalonManager
{
    partial class FormConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.cmdDrawers = new System.Windows.Forms.ComboBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtPrinterPort = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.cmbPrinters = new System.Windows.Forms.ComboBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.btnLaunchPOS = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(560, 189);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(53, 20);
            this.txtServerPort.TabIndex = 37;
            this.txtServerPort.Text = "8000";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Segoe UI Semilight", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(488, 193);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(62, 13);
            this.Label4.TabIndex = 36;
            this.Label4.Text = "Ser&ver Port:";
            // 
            // btnStartServer
            // 
            this.btnStartServer.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartServer.Location = new System.Drawing.Point(129, 229);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(115, 34);
            this.btnStartServer.TabIndex = 35;
            this.btnStartServer.Text = "&Start Server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // cmdDrawers
            // 
            this.cmdDrawers.Enabled = false;
            this.cmdDrawers.FormattingEnabled = true;
            this.cmdDrawers.Location = new System.Drawing.Point(129, 190);
            this.cmdDrawers.Name = "cmdDrawers";
            this.cmdDrawers.Size = new System.Drawing.Size(353, 21);
            this.cmdDrawers.TabIndex = 34;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Segoe UI Semilight", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(12, 193);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(93, 13);
            this.Label5.TabIndex = 33;
            this.Label5.Text = "Cash &Drawer Port:";
            // 
            // txtPrinterPort
            // 
            this.txtPrinterPort.Location = new System.Drawing.Point(561, 163);
            this.txtPrinterPort.Name = "txtPrinterPort";
            this.txtPrinterPort.Size = new System.Drawing.Size(53, 20);
            this.txtPrinterPort.TabIndex = 32;
            this.txtPrinterPort.Text = "8123";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Segoe UI Semilight", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(488, 166);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(71, 13);
            this.Label3.TabIndex = 31;
            this.Label3.Text = "&Listening Port:";
            // 
            // cmbPrinters
            // 
            this.cmbPrinters.FormattingEnabled = true;
            this.cmbPrinters.Location = new System.Drawing.Point(129, 163);
            this.cmbPrinters.Name = "cmbPrinters";
            this.cmbPrinters.Size = new System.Drawing.Size(353, 21);
            this.cmbPrinters.TabIndex = 30;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Segoe UI Semilight", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(12, 166);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(109, 13);
            this.Label2.TabIndex = 29;
            this.Label2.Text = "Receipt &Printer Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 30);
            this.label1.TabIndex = 42;
            this.label1.Text = "Print Server 2020";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 16);
            this.label6.TabIndex = 41;
            this.label6.Text = "Salon Orchid®";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(12, 113);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(406, 34);
            this.Label9.TabIndex = 40;
            this.Label9.Text = "Please select Receipt Printer Name and press Start Server \r\nto connect your Recei" +
    "pt printer to the Point of Sale Web Application.";
            // 
            // btnLaunchPOS
            // 
            this.btnLaunchPOS.Enabled = false;
            this.btnLaunchPOS.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLaunchPOS.Location = new System.Drawing.Point(250, 229);
            this.btnLaunchPOS.Name = "btnLaunchPOS";
            this.btnLaunchPOS.Size = new System.Drawing.Size(112, 34);
            this.btnLaunchPOS.TabIndex = 43;
            this.btnLaunchPOS.Text = "Launch &POS";
            this.btnLaunchPOS.UseVisualStyleBackColor = true;
            // 
            // btnMinimize
            // 
            this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMinimize.Location = new System.Drawing.Point(368, 229);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(112, 34);
            this.btnMinimize.TabIndex = 44;
            this.btnMinimize.Text = "&Close";
            this.btnMinimize.UseVisualStyleBackColor = true;
            // 
            // FormConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 275);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnLaunchPOS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.cmdDrawers);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.txtPrinterPort);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.cmbPrinters);
            this.Controls.Add(this.Label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print Server Configuration";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.TextBox txtServerPort;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Button btnStartServer;
        internal System.Windows.Forms.ComboBox cmdDrawers;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.TextBox txtPrinterPort;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.ComboBox cmbPrinters;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label6;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Button btnLaunchPOS;
        internal System.Windows.Forms.Button btnMinimize;
    }
}