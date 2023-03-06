// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HawKeys
{
    // Adapted from https://www.codeproject.com/Articles/32908/C-Single-Instance-App-With-the-Ability-To-Restore
    public class SingleInstance
    {
        public const string MutexGuid = "50107C68-3CF9-4604-9831-9FBCF06F4561";

        public static int WM_SHOWFIRSTINSTANCE = NativeMethods.RegisterWindowMessage($"WM_SHOWFIRSTINSTANCE|{MutexGuid}");

        private static string MutexName = string.Format("Local\\{0}", MutexGuid);

        private static Mutex _mutex = new Mutex(true, "MutexName");

        public static bool AlreadyRunning()
        {
            return !_mutex.WaitOne(TimeSpan.Zero, false);
        }

        public static void ShowFirstInstance()
        {
            NativeMethods.SendMessage((IntPtr)NativeMethods.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    internal partial class NativeMethods
    {
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        public const int HWND_BROADCAST = 0xffff;
        public const int SW_SHOWNORMAL = 1;

        [DllImport("user32")]
        public static extern bool SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
    }
}
