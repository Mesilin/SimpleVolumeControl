namespace VolumeControl
{
    partial class SettingsForm2
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm2));
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(components);
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            AutoRunCheckBox = new System.Windows.Forms.CheckBox();
            MuteOnScrollLockCheckBox = new System.Windows.Forms.CheckBox();
            SaveButton = new System.Windows.Forms.Button();
            CancelButton_ = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            IsAlternativeVolumeControlCheckBox = new System.Windows.Forms.CheckBox();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            resources.ApplyResources(notifyIcon1, "notifyIcon1");
            notifyIcon1.MouseDoubleClick += NotifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { showToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(contextMenuStrip1, "contextMenuStrip1");
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            resources.ApplyResources(showToolStripMenuItem, "showToolStripMenuItem");
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            // 
            // AutoRunCheckBox
            // 
            resources.ApplyResources(AutoRunCheckBox, "AutoRunCheckBox");
            AutoRunCheckBox.Name = "AutoRunCheckBox";
            AutoRunCheckBox.UseVisualStyleBackColor = true;
            AutoRunCheckBox.CheckedChanged += AutoRunCheckBox_CheckedChanged;
            // 
            // MuteOnScrollLockCheckBox
            // 
            resources.ApplyResources(MuteOnScrollLockCheckBox, "MuteOnScrollLockCheckBox");
            MuteOnScrollLockCheckBox.Name = "MuteOnScrollLockCheckBox";
            MuteOnScrollLockCheckBox.UseVisualStyleBackColor = true;
            // 
            // SaveButton
            // 
            resources.ApplyResources(SaveButton, "SaveButton");
            SaveButton.Name = "SaveButton";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // CancelButton_
            // 
            resources.ApplyResources(CancelButton_, "CancelButton_");
            CancelButton_.Name = "CancelButton_";
            CancelButton_.UseVisualStyleBackColor = true;
            CancelButton_.Click += CancelButton__Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox1.ForeColor = System.Drawing.Color.FromArgb(0, 0, 64);
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.Name = "textBox1";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            label1.Name = "label1";
            // 
            // IsAlternativeVolumeControlCheckBox
            // 
            resources.ApplyResources(IsAlternativeVolumeControlCheckBox, "IsAlternativeVolumeControlCheckBox");
            IsAlternativeVolumeControlCheckBox.Name = "IsAlternativeVolumeControlCheckBox";
            IsAlternativeVolumeControlCheckBox.UseVisualStyleBackColor = true;
            IsAlternativeVolumeControlCheckBox.CheckedChanged += IsAlternativeVolumeControlCheckBox_CheckedChanged;
            // 
            // SettingsForm2
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(IsAlternativeVolumeControlCheckBox);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(CancelButton_);
            Controls.Add(SaveButton);
            Controls.Add(MuteOnScrollLockCheckBox);
            Controls.Add(AutoRunCheckBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "SettingsForm2";
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.CheckBox AutoRunCheckBox;
        private System.Windows.Forms.CheckBox MuteOnScrollLockCheckBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CancelButton_;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox IsAlternativeVolumeControlCheckBox;
    }
}