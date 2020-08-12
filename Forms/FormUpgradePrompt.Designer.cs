namespace SalonManager
{
    partial class FormUpgradePrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpgradePrompt));
            this.Label2 = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnIgnoreBuild = new System.Windows.Forms.Button();
            this.btnDoitLater = new System.Windows.Forms.Button();
            this.btnUpgradeNow = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(133, 300);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(549, 52);
            this.Label2.TabIndex = 40;
            this.Label2.Text = resources.GetString("Label2.Text");
            this.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(81, 79);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(49, 16);
            this.lblInfo.TabIndex = 39;
            this.lblInfo.Text = "Label2";
            // 
            // btnIgnoreBuild
            // 
            this.btnIgnoreBuild.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIgnoreBuild.Location = new System.Drawing.Point(136, 229);
            this.btnIgnoreBuild.Name = "btnIgnoreBuild";
            this.btnIgnoreBuild.Size = new System.Drawing.Size(171, 41);
            this.btnIgnoreBuild.TabIndex = 38;
            this.btnIgnoreBuild.Text = "Notify me next version";
            this.btnIgnoreBuild.UseVisualStyleBackColor = true;
            // 
            // btnDoitLater
            // 
            this.btnDoitLater.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDoitLater.Location = new System.Drawing.Point(489, 229);
            this.btnDoitLater.Name = "btnDoitLater";
            this.btnDoitLater.Size = new System.Drawing.Size(172, 41);
            this.btnDoitLater.TabIndex = 37;
            this.btnDoitLater.Text = "Remind me Later!";
            this.btnDoitLater.UseVisualStyleBackColor = true;
            // 
            // btnUpgradeNow
            // 
            this.btnUpgradeNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpgradeNow.Location = new System.Drawing.Point(313, 229);
            this.btnUpgradeNow.Name = "btnUpgradeNow";
            this.btnUpgradeNow.Size = new System.Drawing.Size(172, 41);
            this.btnUpgradeNow.TabIndex = 36;
            this.btnUpgradeNow.Text = "Upgrade Now";
            this.btnUpgradeNow.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(79, 38);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(652, 30);
            this.Label1.TabIndex = 35;
            this.Label1.Text = "Hi Salon Owner! We\'ve got a new version of your Point of Sale.";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // FormUpgradePrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 392);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnIgnoreBuild);
            this.Controls.Add(this.btnDoitLater);
            this.Controls.Add(this.btnUpgradeNow);
            this.Controls.Add(this.Label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormUpgradePrompt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Point of Sale Available";
            this.Load += new System.EventHandler(this.FormUpgradePrompt_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label lblInfo;
        internal System.Windows.Forms.Button btnIgnoreBuild;
        internal System.Windows.Forms.Button btnDoitLater;
        internal System.Windows.Forms.Button btnUpgradeNow;
        internal System.Windows.Forms.Label Label1;
    }
}