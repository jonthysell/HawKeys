// 
// HawKeysForm.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Reflection;
using System.Windows.Forms;

namespace HawKeys
{
    public partial class HawKeysForm : Form
    {
        public string ProgramName
        {
            get
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

                return string.Format("{0} v{1}", Assembly.GetExecutingAssembly().GetName().Name, versionString);
            }
        }

        public string ProgramCopyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0])).Copyright;
            }
        }

        public HotKeyManager HotKeyManager { get; private set; }

        public HawKeysForm()
        {
            InitializeComponent();
            Text = ProgramName;

            InitHotKeys();
            InitHelpText();
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

        private void InitHelpText()
        {
            mainLabel.Text = string.Join(Environment.NewLine, new string[] {
                ProgramName,
                ProgramCopyright,
                "",
                "Press Alt + ' to insert the ʻokina.",
                "Press Alt + vowel to add a kahakō.",
                "",
                "https://github.com/jonthysell/HawKeys"
            });
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

        private void HawKeysForm_Resize(object sender, System.EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                OnMinimizeWindow();
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnOpenWindow();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnOpenWindow();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
