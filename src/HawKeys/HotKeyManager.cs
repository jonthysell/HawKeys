// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HawKeys
{
    // Adapted from https://social.msdn.microsoft.com/Forums/vstudio/en-US/c061954b-19bf-463b-a57d-b09c98a3fe7d/assign-global-hotkey-to-a-system-tray-application-in-c
    public sealed class HotKeyManager : NativeWindow, IDisposable
    {
        private readonly Dictionary<Keys, HotKeyEntry> _keysMap = new Dictionary<Keys, HotKeyEntry>();
        private readonly Dictionary<int, HotKeyEntry> _idMap = new Dictionary<int, HotKeyEntry>();

        public HotKeyManager()
        {
            CreateHandle(new CreateParams());
        }

        public void RegisterHotKey(Keys modifiers, Keys key, string outputCapsOff, string outputCapsOn)
        {
            Keys hotKey = modifiers | key;

            if (_keysMap.ContainsKey(hotKey))
            {
                throw new Exception(string.Format("The hotkey {0}+{1} is already registered.", modifiers.ToString(), key.ToString()));
            }

            int id = ID_BASE + _idMap.Count;

            NativeMethods.HotKeyModifier hotKeyModifiers = NativeMethods.HotKeyModifier.None;
            hotKeyModifiers |= ((modifiers & Keys.Alt) == Keys.Alt) ? NativeMethods.HotKeyModifier.Alt : NativeMethods.HotKeyModifier.None;
            hotKeyModifiers |= ((modifiers & Keys.Control) == Keys.Control) ? NativeMethods.HotKeyModifier.Control : NativeMethods.HotKeyModifier.None;
            hotKeyModifiers |= ((modifiers & Keys.Shift) == Keys.Shift) ? NativeMethods.HotKeyModifier.Shift : NativeMethods.HotKeyModifier.None;

            if (!NativeMethods.RegisterHotKey(Handle, id, (int)hotKeyModifiers, (int)key))
            {
                throw new Exception(string.Format("Unable to register the hotkey {0}+{1}. It's already in use.", modifiers.ToString(), key.ToString()));
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
                    ExecuteHotKey(entry);
                }
            }

            base.WndProc(ref m);
        }

        // Adapted from https://stackoverflow.com/a/8885228/1653267
        private void ExecuteHotKey(HotKeyEntry entry)
        {
            ModifierKeys modifierKeys = GetModifierKeys();

            string output = Control.IsKeyLocked(Keys.CapsLock) ? entry.OutputCapsOn : entry.OutputCapsOff;
            
            List<NativeMethods.INPUT> inputs = new List<NativeMethods.INPUT>();

            inputs.AddRange(PauseModifiers(modifierKeys, true));

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

            inputs.AddRange(PauseModifiers(modifierKeys, false));

            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(NativeMethods.INPUT)));
        }

        private ModifierKeys GetModifierKeys()
        {
            ModifierKeys mk = ModifierKeys.None;

            mk |= NativeMethods.GetKeyState(NativeMethods.VK_LCONTROL) < 0 ? ModifierKeys.LControl : ModifierKeys.None;
            mk |= NativeMethods.GetKeyState(NativeMethods.VK_RCONTROL) < 0 ? ModifierKeys.RControl : ModifierKeys.None;
            mk |= NativeMethods.GetKeyState(NativeMethods.VK_LSHIFT) < 0 ? ModifierKeys.LShift : ModifierKeys.None;
            mk |= NativeMethods.GetKeyState(NativeMethods.VK_RSHIFT) < 0 ? ModifierKeys.RShift : ModifierKeys.None;
            mk |= NativeMethods.GetKeyState(NativeMethods.VK_LMENU) < 0 ? ModifierKeys.LAlt : ModifierKeys.None;
            mk |= NativeMethods.GetKeyState(NativeMethods.VK_RMENU) < 0 ? ModifierKeys.RAlt : ModifierKeys.None;

            return mk;
        }

        private IEnumerable<NativeMethods.INPUT> PauseModifiers(ModifierKeys modifierKeys, bool keyUp)
        {
            bool laltDown = (modifierKeys & ModifierKeys.LAlt) == ModifierKeys.LAlt;
            bool raltDown = (modifierKeys & ModifierKeys.RAlt) == ModifierKeys.RAlt;

            // We need to mask alts with controls so that menus aren't activated
            bool maskWithCtrl = ((modifierKeys & (ModifierKeys.LControl | ModifierKeys.RControl)) == ModifierKeys.None);

            if (laltDown)
            {
                if (keyUp)
                {
                    if (maskWithCtrl)
                    {
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, false);
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, true);
                    }

                    yield return GetModifierInput(NativeMethods.VK_LMENU, keyUp);
                }
                else
                {
                    if (maskWithCtrl)
                    {
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, false);
                    }

                    yield return GetModifierInput(NativeMethods.VK_LMENU, keyUp);

                    if (maskWithCtrl)
                    {
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, true);
                    }
                }
            }

            if (raltDown)
            {
                if (keyUp)
                {
                    if (maskWithCtrl)
                    {
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, false);
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, true);
                    }

                    yield return GetModifierInput(NativeMethods.VK_RMENU, keyUp);
                }
                else
                {
                    if (maskWithCtrl)
                    {
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, false);
                    }

                    // To fix issues with the right-alt virtual key, we use the scan code instead
                    yield return new NativeMethods.INPUT
                    {
                        type = NativeMethods.INPUT_KEYBOARD,
                        u = new NativeMethods.InputUnion
                        {
                            ki = new NativeMethods.KEYBDINPUT
                            {
                                wVk = 0,
                                wScan = NativeMethods.SCANCODE_RMENU,
                                dwFlags = NativeMethods.KEYEVENTF_EXTENDEDKEY | NativeMethods.KEYEVENTF_SCANCODE,
                                time = 0,
                                dwExtraInfo = NativeMethods.GetMessageExtraInfo(),
                            }
                        }
                    };

                    if (maskWithCtrl)
                    {
                        yield return GetModifierInput(NativeMethods.VK_CONTROL, true);
                    }
                }
            }

            if ((modifierKeys & ModifierKeys.LControl) == ModifierKeys.LControl)
            {
                yield return GetModifierInput(NativeMethods.VK_LCONTROL, keyUp);
            }

            if ((modifierKeys & ModifierKeys.RControl) == ModifierKeys.RControl)
            {
                yield return GetModifierInput(NativeMethods.VK_RCONTROL, keyUp);
            }

            if ((modifierKeys & ModifierKeys.LShift) == ModifierKeys.LShift)
            {
                yield return GetModifierInput(NativeMethods.VK_LSHIFT, keyUp);
            }

            if ((modifierKeys & ModifierKeys.RShift) == ModifierKeys.RShift)
            {
                yield return GetModifierInput(NativeMethods.VK_RSHIFT, keyUp);
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

        [Flags]
        private enum ModifierKeys
        {
            None     = 0x00,
            LShift   = 0x01,
            RShift   = 0x02,
            LControl = 0x04,
            RControl = 0x08,
            LAlt     = 0x10,
            RAlt     = 0x20,
        }
    }

    internal partial class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        public enum HotKeyModifier
        {
            None    = 0x0,
            Alt     = 0x1,
            Control = 0x2,
            Shift   = 0x4,
        }

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        public const ushort VK_SHIFT    = 0x10;
        public const ushort VK_CONTROL  = 0x11;
        public const ushort VK_ALT      = 0x12;

        public const ushort VK_LWIN     = 0x5B;
        public const ushort VK_RWIN     = 0x5C;
        public const ushort VK_LSHIFT   = 0xA0;
        public const ushort VK_RSHIFT   = 0xA1;
        public const ushort VK_LCONTROL = 0xA2;
        public const ushort VK_RCONTROL = 0xA3;
        public const ushort VK_LMENU    = 0xA4;
        public const ushort VK_RMENU    = 0xA5;

        public const ushort SCANCODE_RMENU = 0x38;

        // The following adapted from https://stackoverflow.com/a/8885228/1653267
        public const int INPUT_KEYBOARD = 1;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_UNICODE = 0x0004;
        public const uint KEYEVENTF_SCANCODE = 0x0008;

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
