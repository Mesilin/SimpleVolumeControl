using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;

namespace VolumeControl
{
	public partial class SettingsForm2 : Form
	{
		public SettingsForm2()
		{
			InitializeComponent();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

			Text = $"SimpleVolumeControl v. {fileVersionInfo.FileVersion} - Настройки";
			AutoRunCheckBox.Checked = Properties.Settings.Default.AutoRun;
			IsAlternativeVolumeControlCheckBox.Checked = Properties.Settings.Default.IsAlternativeVolumeControl;

			notifyIcon1.ContextMenuStrip = contextMenuStrip1;
			showToolStripMenuItem.Click += ShowToolStripMenuItem_Click;
			exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;

		}

		private bool _allowVisible;     // ContextMenu's Show command used
		private bool _allowClose;       // ContextMenu's Exit command used

		protected override void SetVisibleCore(bool value)
		{
			if (!_allowVisible)
			{
				value = false;
				if (!this.IsHandleCreated) CreateHandle();
			}
			base.SetVisibleCore(value);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!_allowClose)
			{
				this.Hide();
				e.Cancel = true;
			}
			base.OnFormClosing(e);
		}

		private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_allowVisible = true;
			Show();
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_allowClose = true;
			Application.Exit();
		}

		/// <summary>
		/// Удалить задачу по автозапуску из планировщика заданий
		/// </summary>
		private static void DeleteTask()
		{
			using var ts = new TaskService();
			if (ts.GetTask("SimpleVolumeControlAutoRunTask") != null)
				ts.RootFolder.DeleteTask("SimpleVolumeControlAutoRunTask");
		}

		/// <summary>
		/// Создает задачу по автозапуску с повышенными правами в планировщике заданий
		/// </summary>
		private static void CreateTask()
		{
			try
			{
				var taskName = "SimpleVolumeControlAutoRunTask";
				using var identity = WindowsIdentity.GetCurrent();
				var principal = new WindowsPrincipal(identity);
				if (principal.IsInRole(WindowsBuiltInRole.Administrator))
				{
					using var ts = new TaskService();
					var tasks = ts.RootFolder.GetTasks(new Regex(taskName));
					DeleteTask();

					var td = ts.NewTask();
					td.Settings.MultipleInstances = TaskInstancesPolicy.StopExisting;
					td.Settings.DisallowStartIfOnBatteries = false;
					td.RegistrationInfo.Description = "Launch SimpleVolumeControl when user login";
					td.Principal.RunLevel = TaskRunLevel.Highest;

					var lt = new LogonTrigger { Enabled = true };
					td.Triggers.Add(lt);
					td.Actions.Add(new ExecAction(
						System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VolumeControl.exe"), null, AppDomain.CurrentDomain.BaseDirectory));
					ts.RootFolder.RegisterTaskDefinition(taskName, td);
				}
			}
			catch (Exception ex)
			{ MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.IsAlternativeVolumeControl = IsAlternativeVolumeControlCheckBox.Checked;
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

		private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			_allowVisible = true;
			Show();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
