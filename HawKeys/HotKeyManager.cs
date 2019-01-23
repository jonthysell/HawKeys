// 
// HotKeyManager.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace HawKeys
{
    // Adapted from https://social.msdn.microsoft.com/Forums/vstudio/en-US/c061954b-19bf-463b-a57d-b09c98a3fe7d/assign-global-hotkey-to-a-system-tray-application-in-c
    public sealed class HotKeyManager : NativeWindow, IDisposable
    {
        private Dictionary<Keys, HotKeyEntry> _keysMap = new Dictionary<Keys, HotKeyEntry>();
        private Dictionary<int, HotKeyEntry> _idMap = new Dictionary<int, HotKeyEntry>();

        private ConcurrentQueue<HotKeyEntry> _outputBuffer = new ConcurrentQueue<HotKeyEntry>();
        private Task _outputTask = null;
        private CancellationTokenSource _outputCTS = null;

        public HotKeyManager()
        {
            CreateHandle(new CreateParams());
            StartOutput();
        }

        public void RegisterHotKey(Keys modifiers, Keys key, string outputCapsOff, string outputCapsOn)
        {
            Keys hotKey = modifiers | key;

            if (_keysMap.ContainsKey(hotKey))
            {
                throw new Exception("That hotkey already exists.");
            }

            int id = ID_BASE + _idMap.Count;

            NativeMethods.ModifierKeys nativeModifierKeys = NativeMethods.ModifierKeys.None;
            nativeModifierKeys |= ((modifiers & Keys.Alt) == Keys.Alt) ? NativeMethods.ModifierKeys.Alt : NativeMethods.ModifierKeys.None;
            nativeModifierKeys |= ((modifiers & Keys.Control) == Keys.Control) ? NativeMethods.ModifierKeys.Ctrl : NativeMethods.ModifierKeys.None;
            nativeModifierKeys |= ((modifiers & Keys.Shift) == Keys.Shift) ? NativeMethods.ModifierKeys.Shift : NativeMethods.ModifierKeys.None;

            if (!NativeMethods.RegisterHotKey(Handle, id, (int)nativeModifierKeys, (int)key))
            {
                throw new Exception("Unable to register the hotkey.");
            }

            HotKeyEntry hke = new HotKeyEntry()
            {
                ID = id,
                HotKey = hotKey,
                OutputCapsOff = outputCapsOff,
                OutputCapsOn = outputCapsOn,
            };

            _keysMap.Add(hke.HotKey, hke);
            _idMap.Add(hke.ID, hke);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY_MSG_ID)
            {
                int id = m.WParam.ToInt32();

                if (_idMap.TryGetValue(id, out HotKeyEntry entry))
                {
                    _outputBuffer.Enqueue(entry);
                }
            }

            base.WndProc(ref m);
        }

        private void StartOutput()
        {
            _outputCTS = new CancellationTokenSource();
            _outputTask = Task.Factory.StartNew(() =>
            {
                while (!_outputCTS.IsCancellationRequested)
                {
                    if (_outputBuffer.TryDequeue(out HotKeyEntry hke))
                    {
                        ExecuteHotKey(hke);
                    }
                    Thread.Sleep(50);
                }
            });
        }

        // Adapted from https://stackoverflow.com/a/8885228/1653267
        private void ExecuteHotKey(HotKeyEntry entry)
        {
            Keys modiferKeys = Control.ModifierKeys;

            string output = Control.IsKeyLocked(Keys.CapsLock) ? entry.OutputCapsOn : entry.OutputCapsOff;
            
            List<NativeMethods.INPUT> inputs = new List<NativeMethods.INPUT>();

            inputs.AddRange(PauseModifiers(modiferKeys, true));

            foreach (char c in output)
            {
                foreach (bool keyUp in new bool[] { false, true })
                {
                    NativeMethods.INPUT input = new NativeMethods.INPUT
                    {
                        type = NativeMethods.INPUT_KEYBOARD,
                        u = new NativeMethods.InputUnion
                        {
                            ki = new NativeMethods.KEYBDINPUT
                            {
                                wVk = 0,
                                wScan = c,
                                dwFlags = NativeMethods.KEYEVENTF_UNICODE | (keyUp ? NativeMethods.KEYEVENTF_KEYUP : 0),
                                time = 0,
                                dwExtraInfo = NativeMethods.GetMessageExtraInfo(),
                            }
                        }
                    };

                    inputs.Add(input);
                }
            }

            inputs.AddRange(PauseModifiers(modiferKeys, false));

            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(NativeMethods.INPUT)));
        }

        private IEnumerable<NativeMethods.INPUT> PauseModifiers(Keys modifierKeys, bool keyUp)
        {
            if ((modifierKeys & Keys.Shift) == Keys.Shift)
            {
                yield return GetModifierInput(NativeMethods.VK_SHIFT, keyUp);
            }

            if ((modifierKeys & Keys.Control) == Keys.Control)
            {
                yield return GetModifierInput(NativeMethods.VK_CTRL, keyUp);
            }

            if ((modifierKeys & Keys.Alt) == Keys.Alt)
            {
                bool maskWithCtrl = ((modifierKeys & Keys.Control) != Keys.Control);

                if (maskWithCtrl)
                {
                    yield return GetModifierInput(NativeMethods.VK_CTRL, false);
                }

                yield return GetModifierInput(NativeMethods.VK_ALT, keyUp);

                if (maskWithCtrl)
                {
                    yield return GetModifierInput(NativeMethods.VK_CTRL, true);
                }
            }
        }

        private NativeMethods.INPUT GetModifierInput(ushort virtualKey, bool keyUp)
        {
            return new NativeMethods.INPUT
            {
                type = NativeMethods.INPUT_KEYBOARD,
                u = new NativeMethods.InputUnion
                {
                    ki = new NativeMethods.KEYBDINPUT
                    {
                        wVk = virtualKey,
                        wScan = 0,
                        dwFlags = keyUp ? NativeMethods.KEYEVENTF_KEYUP : 0,
                        time = 0,
                        dwExtraInfo = NativeMethods.GetMessageExtraInfo(),
                    }
                }
            };
        }

        public void Dispose()
        {
            _outputCTS?.Cancel();
            _outputTask?.Wait(_outputCTS.Token);

            for (int i = _idMap.Count - 1; i >= 0; i--)
            {
                int id = ID_BASE + i;
                NativeMethods.UnregisterHotKey(Handle, id);
            }

            DestroyHandle();
        }

        private const int WM_HOTKEY_MSG_ID = 0x0312;
        private const int ID_BASE = 100;

        private struct HotKeyEntry
        {
            public int ID;
            public Keys HotKey;
            public string OutputCapsOff;
            public string OutputCapsOn;
        }
    }

    internal partial class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        public enum ModifierKeys
        {
            None = 0x0000,
            Alt = 0x0001,
            Ctrl = 0x0002,
            Shift = 0x0004,
        }

        public const ushort VK_SHIFT = 0x10;
        public const ushort VK_CTRL = 0x11;
        public const ushort VK_ALT = 0x12;

        public const ushort VK_LSHIFT = 0xA0;
        public const ushort VK_RSHIFT = 0xA1;
        public const ushort VK_LCONTROL = 0xA2;
        public const ushort VK_RCONTROL = 0xA3;
        public const ushort VK_LMENU = 0xA4;
        public const ushort VK_RMENU = 0xA5;

        // The following adapted from https://stackoverflow.com/a/8885228/1653267
        public const int INPUT_KEYBOARD = 1;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_UNICODE = 0x0004;

        public struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}
