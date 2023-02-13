using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32.TaskScheduler;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace VolumeControl
{
    public partial class SettingsForm2 : Form
    {
        public Updater updater;
        public SettingsForm2()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            this.Text= $"SimpleVolumeControl v. {fileVersionInfo.FileVersion} - Настройки";
            //this.label1.Text = "E. Mesilin, 2023";
            this.AutoRunCheckBox.Checked = Properties.Settings.Default.AutoRun;
            this.MuteOnScrollLockCheckBox.Checked = Properties.Settings.Default.MuteOnScrollLock;
            this.UpdateCheckBox.Checked = Properties.Settings.Default.CheckUpdate;

            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            this.showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            this.exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            updater = new Updater();


            if (Properties.Settings.Default.CheckUpdate)
            {
                var t = new Timer();
                t.Interval = 20000;
                t.Tick += T_Tick;
                t.Start();
            }
        }
        private void T_Tick(object sender, EventArgs e)
        {
            VersionInfo lastVersionInfo = GetLastVersionInfo();

            if (updater.currentVersion < lastVersionInfo.version)
            {
                ((Timer)sender).Stop();
                updater.note = lastVersionInfo.note;
                updater.newVersionPath = lastVersionInfo.path;
                updater.lastVersion = lastVersionInfo.version;

                var updForm = new UpdateForm(updater);
                updForm.Show();
            }
        }
        struct VersionInfo
        {
            public Version version;
            public string note;
            public string path;
        }
        private VersionInfo GetLastVersionInfo()
        {
            try
            {
                XmlDocument historyXml = new XmlDocument();
                historyXml.Load("https://raw.githubusercontent.com/Mesilin/SimpleVolumeControl/master/VolumeControl/history.xml");
                XmlNodeList nodes = historyXml.GetElementsByTagName("Release");

                if (nodes.Count > 0)
                {
                    var ver = nodes[0].Attributes.GetNamedItem("version").Value;
                    var note = nodes[0].Attributes.GetNamedItem("note").Value;
                    var path = nodes[0].Attributes.GetNamedItem("path").Value;
                    return new VersionInfo { note = note, path = path, version = new Version(ver) };
                }
            }
            catch
            {
                return new VersionInfo { note = "", path = "", version = new Version("0.0.0.0") };
            }
            return new VersionInfo { note = "", path = "", version = new Version("0.0.0.0") };
        }

        //public async Task<bool> HasNewVersion()
        //{
        //    updater = new Updater();
        //    updater.lastVersion = await updater.GetLastVersion();
        //    return updater.currentVersion < updater.lastVersion;
        //}

        private bool allowVisible;     // ContextMenu's Show command used
        private bool allowClose;       // ContextMenu's Exit command used

        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                this.Hide();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowVisible = true;
            Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowClose = true;
            Application.Exit();
        }

        /// <summary>
        /// Удалить задачу по автозапуску из планировщика заданий
        /// </summary>
        private void DeleteTask()
        {
            using (TaskService ts = new TaskService())
            {
                if (ts.GetTask("SimpleVolumeControlAutoRunTask") != null)
                    ts.RootFolder.DeleteTask("SimpleVolumeControlAutoRunTask");
            }
        }

        /// <summary>
        /// Создает задачу по автозапуску с повышенными правами в планировщике заданий
        /// </summary>
        private void CreateTask()
        {
            try
            {
                string TaskName = "SimpleVolumeControlAutoRunTask";
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        using (TaskService ts = new TaskService())
                        {
                            var tasks = ts.RootFolder.GetTasks(new Regex(TaskName));
                            DeleteTask();

                            TaskDefinition td = ts.NewTask();
                            td.Settings.MultipleInstances = TaskInstancesPolicy.StopExisting;
                            td.Settings.DisallowStartIfOnBatteries = false;
                            td.RegistrationInfo.Description = "Launch SimpleVolumeControl when user login";
                            td.Principal.RunLevel = TaskRunLevel.Highest;

                            LogonTrigger lt = new LogonTrigger { Enabled = true };
                            td.Triggers.Add(lt);
                            td.Actions.Add(new ExecAction(
                                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VolumeControl.exe"), null, AppDomain.CurrentDomain.BaseDirectory));
                            ts.RootFolder.RegisterTaskDefinition(TaskName, td);
                        }
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CheckUpdate = UpdateCheckBox.Checked;
            Properties.Settings.Default.MuteOnScrollLock = MuteOnScrollLockCheckBox.Checked;
            Properties.Settings.Default.AutoRun = AutoRunCheckBox.Checked;
            if (Properties.Settings.Default.AutoRun)
                CreateTask();
            else
                DeleteTask();
            Properties.Settings.Default.Save();

            Close();
        }

        private void CancelButton__Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            allowVisible = true;
            Show();
        }
    }
}
