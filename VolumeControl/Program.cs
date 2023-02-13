﻿// This code is distributed under MIT license. 
// Copyright (c) 2010-2018 George Mamaladze
// See license.txt or https://mit-license.org/

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Gma.System.MouseKeyHook;
namespace VolumeControl
{
    internal class Program
    {                                                                                                                       
        private static void Main(string[] args)
        {
            var icon = new NotifyIcon();

            icon.Visible = true;
            Do();
            //Application.Run(new ApplicationContext());
            Application.Run(new SettingsForm2());
        }

        #region перехват нажатия ScrollLock
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (VolumeControl.Properties.Settings.Default.MuteOnScrollLock && (nCode >= 0) && (wParam == (IntPtr)WM_KEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (((Keys)vkCode == Keys.Scroll))
                {
                    keybd_event((byte)Keys.VolumeMute, 0, 0, 0);
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        #endregion перехват нажатия ScrollLock

        static IKeyboardMouseEvents keyboardMouseEvents;

        public static async void Do()
        {

            //mute по нажатию ScrollLock
            _hookID = SetHook(_proc);

            keyboardMouseEvents = Hook.GlobalEvents();
            keyboardMouseEvents.MouseWheelExt += KeyboardMouseEvents_MouseWheelExt; //звук колесом мыши при зажатом капсе
            keyboardMouseEvents.MouseDownExt += KeyboardMouseEvents_MouseDownExt;//Play\Pause при нажатии на колесо мыши с зажатым капсом

            //keyboardMouseEvents.KeyDown += KeyboardMouseEvents_KeyDown;

            //var map = new Dictionary<Sequence, Action>
            //{
            //    //{Sequence.FromString("Control+Z,B"), Console.WriteLine},
            //    //{Sequence.FromString("Control+Z,Z"), Console.WriteLine},
            //    {Sequence.FromString("CapsLock,CapsLock"), () => keybd_event((byte)Keys.MediaNextTrack, 0, 0, 0)}
            //};
            //keyboardMouseEvents.OnSequence(map);
            //var map = new Dictionary<Combination, Action>
            //{
            //    //Specify which key combinations to detect and action - what to do if detected.
            //    //You can create a key combinations directly from string or ...
            //    //{Combination.FromString("A+B+C"), () => Debug.WriteLine(":-)")},
            //    //... or alternatively you can use builder methods
            //    //{Combination.TriggeredBy(Keys.F).With(Keys.E).With(Keys.D), () => Debug.WriteLine(":-D")},
            //    {Combination.TriggeredBy(Keys.Up).With(Keys.CapsLock), () => keybd_event((byte)Keys.VolumeUp, 0, 0, 0)},
            //    {Combination.TriggeredBy(Keys.Down).With(Keys.CapsLock), () => keybd_event((byte)Keys.VolumeDown, 0, 0, 0)},
            //    {Combination.TriggeredBy(Keys.Left).With(Keys.CapsLock), () => keybd_event((byte)Keys.MediaPreviousTrack, 0, 0, 0)},
            //    {Combination.TriggeredBy(Keys.Right).With(Keys.CapsLock), () => keybd_event((byte)Keys.MediaNextTrack, 0, 0, 0)},
            //    //{Combination.FromString("Alt+A"), () => Debug.WriteLine(":-P")},
            //    //{Combination.FromString("Control+Shift+Z"), () => Debug.WriteLine(":-/")},
            //    //{Combination.FromString("Escape"), quit}
            //};
            //keyboardMouseEvents.OnCombination(map);
        }

        //private static void KeyboardMouseEvents_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (!Keyboard.IsKeyDown(Keys.CapsLock))
        //        return;
        //    switch (e.KeyCode)
        //    {
        //        case Keys.Up:
        //            keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
        //            break;
        //        case Keys.Down:
        //            keybd_event((byte)Keys.VolumeDown, 0, 0, 0);
        //            break;
        //        case Keys.Left:
        //            keybd_event((byte)Keys.MediaPreviousTrack, 0, 0, 0);
        //            break;
        //        case Keys.Right:
        //            keybd_event((byte)Keys.MediaNextTrack, 0, 0, 0);
        //            break;
        //    }

        //    e.SuppressKeyPress = true;
        //    //e.Handled = true;
        //    PreventCaps();
        //}

        private static void KeyboardMouseEvents_MouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (!Keyboard.IsKeyDown(Keys.CapsLock))
                return;
            if (e.Button == MouseButtons.Middle && Keyboard.IsKeyDown(Keys.CapsLock))
            {
                keybd_event((byte)Keys.MediaPlayPause, 0, 0, 0);
                e.Handled = true;
                PreventCaps();
            }
            //if (e.Button == MouseButtons.Right && Keyboard.IsKeyDown(Keys.CapsLock))
            //{
            //    keybd_event((byte)Keys.MediaNextTrack, 0, 0, 0);
            //}
            //if (e.Button == MouseButtons.Left && Keyboard.IsKeyDown(Keys.CapsLock))
            //{
            //    keybd_event((byte)Keys.MediaPreviousTrack, 0, 0, 0);
            //}
        }

        private static void KeyboardMouseEvents_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            if (!Keyboard.IsKeyDown(Keys.CapsLock))
                return;

            if (e.Delta > 0)
                keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
            else
                keybd_event((byte)Keys.VolumeDown, 0, 0, 0);
            e.Handled = true;
            PreventCaps();
        }

        static void PreventCaps()
        {
            if (Control.IsKeyLocked(Keys.CapsLock)) // Checks Capslock is on
            {
                const int KEYEVENTF_EXTENDEDKEY = 0x1;
                const int KEYEVENTF_KEYUP = 0x2;
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }
        public abstract class Keyboard
        {
            [Flags]
            private enum KeyStates
            {
                None = 0,
                Down = 1,
                Toggled = 2
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            private static extern short GetKeyState(int keyCode);

            private static KeyStates GetKeyState(Keys key)
            {
                KeyStates state = KeyStates.None;

                short retVal = GetKeyState((int)key);

                //If the high-order bit is 1, the key is down
                //otherwise, it is up.
                if ((retVal & 0x8000) == 0x8000)
                    state |= KeyStates.Down;

                //If the low-order bit is 1, the key is toggled.
                if ((retVal & 1) == 1)
                    state |= KeyStates.Toggled;

                return state;
            }

            public static bool IsKeyDown(Keys key)
            {
                return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
            }

            public static bool IsKeyToggled(Keys key)
            {
                return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
            }
        }

    }
}