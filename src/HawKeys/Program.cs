// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Windows.Forms;

namespace HawKeys
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            if (SingleInstance.AlreadyRunning())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                HawKeysForm form = new HawKeysForm();
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Join(Environment.NewLine,new string[] { "A fatal error has occured and HawKeys will now exit.", ex.Message }), "HawKeys", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
