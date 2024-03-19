// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;

namespace HawKeys
{
    public partial class HawKeysForm : Form
    {
        public string ProgramName
        {
            get
            {
                if (null == _programName)
                {
                    Version version = Assembly.GetExecutingAssembly().GetName().Version;

                    string versionString = string.Format("{0}.{1}", version.Major, version.Minor);

                    if (version.Revision == 0 && version.Build > 0)
                    {
                        versionString += string.Format(".{0}", version.Build);
                    }
                    else if (version.Revision > 0)
                    {
                        versionString += string.Format(".{0}.{1}", version.Build, version.Revision);
                    }

                    _programName = string.Format("{0} v{1}", Assembly.GetExecutingAssembly().GetName().Name, versionString);
                }

                return _programName;
            }
        }
        private string _programName = null;

        public string ProgramCopyright
        {
            get
            {
                return _programCopyright ?? (_programCopyright = ((AssemblyCopyrightAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0])).Copyright);
            }
        }
        private string _programCopyright = null;

        public string CopyrightUrl => "https://github.com/jonthysell/HawKeys";

        public bool StartMinimized
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(Settings.GetValue("StartMinimized", 0));
                }
                catch (Exception ex)
                {
                    HandleException(new Exception("Unable to load StartMinimized setting.", ex));
                }

                return false;
            }
            set
            {
                try
                {
                    Settings.SetValue("StartMinimized", value, RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    HandleException(new Exception("Unable to save StartMinimized setting.", ex));
                }
            }
        }

        public RegistryKey Settings
        {
            get
            {
                if (_settings is null)
                {
                    _settings = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("JonThysell").CreateSubKey("HawKeys");
                }
                return _settings;
            }
        }
        private RegistryKey _settings = null;

        public HotKeyManager HotKeyManager { get; private set; }

        public HawKeysForm()
        {
            InitializeComponent();

            InitHotKeys();
            InitLabels();
            InitSettings();
        }

        private void InitHotKeys()
        {
            HotKeyManager = new HotKeyManager();

            HotKeyManager.RegisterHotKey(Keys.Alt, Keys.OemQuotes, "ʻ", "ʻ");
            HotKeyManager.RegisterHotKey(Keys.Alt | Keys.Shift, Keys.OemQuotes, "ʻ", "ʻ");

            HotKeyManager.RegisterHotKey(Keys.Alt, Keys.A, "ā", "Ā");
            HotKeyManager.RegisterHotKey(Keys.Alt | Keys.Shift, Keys.A, "Ā", "ā");

            HotKeyManager.RegisterHotKey(Keys.Alt, Keys.E, "ē", "Ē");
            HotKeyManager.RegisterHotKey(Keys.Alt | Keys.Shift, Keys.E, "Ē", "ē");

            HotKeyManager.RegisterHotKey(Keys.Alt, Keys.I, "ī", "Ī");
            HotKeyManager.RegisterHotKey(Keys.Alt | Keys.Shift, Keys.I, "Ī", "ī");

            HotKeyManager.RegisterHotKey(Keys.Alt, Keys.O, "ō", "Ō");
            HotKeyManager.RegisterHotKey(Keys.Alt | Keys.Shift, Keys.O, "Ō", "ō");

            HotKeyManager.RegisterHotKey(Keys.Alt, Keys.U, "ū", "Ū");
            HotKeyManager.RegisterHotKey(Keys.Alt | Keys.Shift, Keys.U, "Ū", "ū");
        }

        private void InitLabels()
        {
            Text = ProgramName;
            topLabel.Text = ProgramName;
            copyrightLinkLabel.Text = ProgramCopyright;
        }

        private void InitSettings()
        {
            startMinimizedCheckBox.Checked = StartMinimized;
        }

        private void HandleException(Exception ex)
        {
            MessageBox.Show(ex.Message, "HawKeys", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool PromptForConfirmation(string prompt)
        {
            return MessageBox.Show(prompt, "HawKeys", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void OnMinimizeWindow()
        {
            ShowInTaskbar = false;
            notifyIcon.Visible = true;
        }

        private void OnOpenWindow()
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private void HawKeysForm_Resize(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    OnMinimizeWindow();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                OnOpenWindow();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OnOpenWindow();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void copyrightLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (PromptForConfirmation("This will open the HawKeys website in your browser. Do you want to continue?"))
                {
                    Process.Start(CopyrightUrl);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void startMinimizedCheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                bool startMinimized = StartMinimized;

                if (startMinimized || PromptForConfirmation("This will set HawKeys to start automatically minimized in the System Tray. Do you want to continue?"))
                {
                    // Toggling off OR confirmed toggling on
                    startMinimizedCheckBox.Checked = (StartMinimized = !startMinimized);

                    if (!startMinimized && PromptForConfirmation("Would you like to minimize HawKeys right now?"))
                    {
                        // Minimizing 
                        WindowState = FormWindowState.Minimized;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void HawKeysForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (StartMinimized)
                {
                    WindowState = FormWindowState.Minimized;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    OnOpenWindow();
                }
                else
                {
                    Activate();
                }
            }
            base.WndProc(ref message);
        }
    }
}
