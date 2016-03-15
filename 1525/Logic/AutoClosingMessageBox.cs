using System;
using System.Windows;
using System.Runtime.InteropServices;

namespace PDTUtils
{
    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            MessageBox.Show(text, caption);
        }
        
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        
        void OnTimerElapsed(object state)
        {
            var mbWnd = FindWindow(null, _caption);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WClose, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        
        const int WClose = 0x0010;
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
    }
}
