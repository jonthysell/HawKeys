﻿namespace HawKeys
{
    partial class HawKeysForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HawKeysForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.helpLabel = new System.Windows.Forms.Label();
            this.topLabel = new System.Windows.Forms.Label();
            this.copyrightLinkLabel = new System.Windows.Forms.LinkLabel();
            this.startMinimizedCheckBox = new System.Windows.Forms.CheckBox();
            this.notifyContextMenuStrip.SuspendLayout();
            this.mainLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.notifyContextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "HawKeys";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // notifyContextMenuStrip
            // 
            this.notifyContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.notifyContextMenuStrip.Name = "notifyContextMenuStrip";
            this.notifyContextMenuStrip.Size = new System.Drawing.Size(104, 48);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // mainLayoutPanel
            // 
            this.mainLayoutPanel.ColumnCount = 1;
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainLayoutPanel.Controls.Add(this.helpLabel, 0, 1);
            this.mainLayoutPanel.Controls.Add(this.topLabel, 0, 0);
            this.mainLayoutPanel.Controls.Add(this.copyrightLinkLabel, 0, 3);
            this.mainLayoutPanel.Controls.Add(this.startMinimizedCheckBox, 0, 2);
            this.mainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            this.mainLayoutPanel.RowCount = 4;
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutPanel.Size = new System.Drawing.Size(384, 211);
            this.mainLayoutPanel.TabIndex = 0;
            // 
            // helpLabel
            // 
            this.helpLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.helpLabel.AutoSize = true;
            this.helpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLabel.Location = new System.Drawing.Point(67, 74);
            this.helpLabel.Margin = new System.Windows.Forms.Padding(10);
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(250, 40);
            this.helpLabel.TabIndex = 1;
            this.helpLabel.Text = "Press Alt + \' to insert the ʻokina.\r\nPress Alt + vowel to add a kahakō.";
            this.helpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // topLabel
            // 
            this.topLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.topLabel.AutoSize = true;
            this.topLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topLabel.Location = new System.Drawing.Point(147, 20);
            this.topLabel.Margin = new System.Windows.Forms.Padding(20);
            this.topLabel.Name = "topLabel";
            this.topLabel.Size = new System.Drawing.Size(89, 24);
            this.topLabel.TabIndex = 0;
            this.topLabel.Text = "HawKeys";
            this.topLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // copyrightLinkLabel
            // 
            this.copyrightLinkLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.copyrightLinkLabel.AutoSize = true;
            this.copyrightLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyrightLinkLabel.Location = new System.Drawing.Point(113, 181);
            this.copyrightLinkLabel.Margin = new System.Windows.Forms.Padding(10);
            this.copyrightLinkLabel.Name = "copyrightLinkLabel";
            this.copyrightLinkLabel.Size = new System.Drawing.Size(158, 17);
            this.copyrightLinkLabel.TabIndex = 2;
            this.copyrightLinkLabel.TabStop = true;
            this.copyrightLinkLabel.Text = "Copyright © Jon Thysell";
            this.copyrightLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.copyrightLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.copyrightLinkLabel_LinkClicked);
            // 
            // startMinimizedCheckBox
            // 
            this.startMinimizedCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.startMinimizedCheckBox.AutoCheck = false;
            this.startMinimizedCheckBox.AutoSize = true;
            this.startMinimizedCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startMinimizedCheckBox.Location = new System.Drawing.Point(56, 134);
            this.startMinimizedCheckBox.Margin = new System.Windows.Forms.Padding(10);
            this.startMinimizedCheckBox.Name = "startMinimizedCheckBox";
            this.startMinimizedCheckBox.Size = new System.Drawing.Size(271, 24);
            this.startMinimizedCheckBox.TabIndex = 3;
            this.startMinimizedCheckBox.Text = "Start minimized in the System Tray";
            this.startMinimizedCheckBox.UseVisualStyleBackColor = true;
            this.startMinimizedCheckBox.Click += new System.EventHandler(this.startMinimizedCheckBox_Click);
            // 
            // HawKeysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.mainLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "HawKeysForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HawKeys";
            this.Load += new System.EventHandler(this.HawKeysForm_Load);
            this.Resize += new System.EventHandler(this.HawKeysForm_Resize);
            this.notifyContextMenuStrip.ResumeLayout(false);
            this.mainLayoutPanel.ResumeLayout(false);
            this.mainLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.ContextMenuStrip notifyContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label helpLabel;
        private System.Windows.Forms.LinkLabel copyrightLinkLabel;
        private System.Windows.Forms.CheckBox startMinimizedCheckBox;
    }
}

