using System;
using System.Runtime.InteropServices;

namespace ACManager
{
    /// <summary>
    /// Class to be used for simulating any user input not able to be implemented through Decal itself.
    /// </summary>
    public static class Simulate
    {
        /// <summary>
        /// Imports the user32.dll from Windows to access unmanaged code.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hwnd, uint msg, IntPtr wparam, UIntPtr lparam);

        /// <summary>
        /// Windows value to simulate mouse movement.
        /// </summary>
        public const int WM_MOUSEMOVE = 0x0200;

        /// <summary>
        /// Windows value to simulate left mouse button down.
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x0201;

        /// <summary>
        /// Windows value to simulate left mouse button up.
        /// </summary>
        public const int WM_LBUTTONUP = 0x0202;

        /// <summary>
        /// Sends a mouse movement to desired window coordinates and left clicks.
        /// </summary>
        public static void MouseClick(IntPtr hwnd, int x, int y)
        {
            int loc = (y * 0x10000) + x;

            PostMessage(hwnd, WM_MOUSEMOVE, (IntPtr)0x00000000, (UIntPtr)loc);
            PostMessage(hwnd, WM_LBUTTONDOWN, (IntPtr)0x00000001, (UIntPtr)loc);
            PostMessage(hwnd, WM_LBUTTONUP, (IntPtr)0x00000000, (UIntPtr)loc);
        }
    }
}
