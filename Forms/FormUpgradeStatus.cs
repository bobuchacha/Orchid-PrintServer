using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalonManager
{
    public class ProgressEventArgsEx
    {
        public int Percentage { get; set; }
        public string Text { get; set; }
    }

    public partial class FormUpgradeStatus : Form
    {
        public Uri DownloadURI { get; set; }             // download URI
        public string SaveTarget { get; set; }              // file location where downloader saves to
        public event EventHandler onDownloadComplete;       // event handler when download complete

        public FormUpgradeStatus()
        {
            InitializeComponent();
        }

        private void FormUpgradeStatus_Load(object sender, EventArgs e)
        {

        }

        public async static Task<string> DownloadStraingAsyncronous(string url, IProgress<ProgressEventArgsEx> progress)
        {
            WebClient c = new WebClient();
            byte[] buffer = new byte[1024];
            var bytes = 0;
            var all = String.Empty;
            using (var stream = await c.OpenReadTaskAsync(url))
            {
                int total = -1;
                Int32.TryParse(c.ResponseHeaders[HttpRequestHeader.ContentLength], out total);
                for (; ; )
                {
                    int len = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (len == 0)
                        break;
                    string text = c.Encoding.GetString(buffer, 0, len);

                    bytes += len;
                    all += text;
                    if (progress != null)
                    {
                        var args = new ProgressEventArgsEx();
                        args.Percentage = (total <= 0 ? 0 : (100 * bytes) / total);
                        progress.Report(args);
                    }
                }
            }
            return all;
        }
        public void Download()
        {
            DownloadAsync();
        }
        public void DownloadAsync()
        {
            if (DownloadURI == null)
            {
                throw (new Exception("Download URI must be set before calling Download method"));
            } else if (SaveTarget == "")
            {
                throw (new Exception("Download Target must be set before calling Download method"));
            } else if (onDownloadComplete == null)
            {
                throw (new Exception("Download Complete handler must be set before calling Download method"));
            }
            else
            {
                this.Show();
                Thread thread = new Thread(() => {
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(DownloadURI, SaveTarget);
                });
                thread.Start();
                
           

            }

            
        }
    

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                if (totalBytes > 0)
                {
                    double percentage = bytesIn / totalBytes * 100;
                    lblProgress.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
                    ProgressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
                }else
                {
                    lblProgress.Text = "Downloaded " + e.BytesReceived;
                    ProgressBar1.Value = 50;
                }
            });
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate {
                lblProgress.Text = "Completed";
            });
            if (onDownloadComplete != null)
            {
                onDownloadComplete(sender, e);
            }
        }

    }
}
