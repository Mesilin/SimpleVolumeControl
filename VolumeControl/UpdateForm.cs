using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace VolumeControl
{
    public partial class UpdateForm : Form
    {
        private string fileUrl;
        public UpdateForm()
        {
            InitializeComponent();
        }
        public UpdateForm(Updater updater)
        {
            InitializeComponent();
            this.CurrentVersionLabel.Text = updater.currentVersion.ToString();
            this.LastVersionLabel.Text = updater.lastVersion.ToString();
            this.richTextBox1.Text = updater.note.ToString();
            this.fileUrl = updater.newVersionPath;
        }

        private void CloseUpdFormBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            label6.Visible = true;
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new System.Uri(fileUrl), System.IO.Path.GetTempPath() + "~VolumeControlInstaller.exe");
            }
            void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                progressBar1.Value = e.ProgressPercentage;
            }
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Скачивание новой версии завершено. приложение будет закрыто для установки");
            Process.Start(System.IO.Path.GetTempPath() + "~VolumeControlInstaller.exe");
            Application.Exit();
        }
    }
}
