using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

class ScrMatrix {
    private const int WH_MOUSE_LL = 14;
    private const int WM_MOUSEWHEEL = 0x020A;
    private const int VK_MENU = 0x12;    // Alt
    private const int VK_CONTROL = 0x11; // Ctrl
    private const int WM_APPCOMMAND = 0x0319;
    private const int APPCOMMAND_VOLUME_UP = 0xA0000;
    private const int APPCOMMAND_VOLUME_DOWN = 0x90000;

    private static LowLevelMouseProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static int _currentGamma = 128; // Centre

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static void Main() {
        _hookID = SetHook(_proc);
        Application.Run();
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
        if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEWHEEL) {
            int delta = (short)((checked((int)lParam + 8) >> 16) & 0xffff);
            bool altPressed = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;
            bool ctrlPressed = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;

            if (altPressed) {
                IntPtr handle = GetForegroundWindow();
                SendMessageW(handle, WM_APPCOMMAND, handle, (IntPtr)(delta > 0 ? APPCOMMAND_VOLUME_UP : APPCOMMAND_VOLUME_DOWN));
                return (IntPtr)1; 
            }
            if (ctrlPressed) {
                _currentGamma = Math.Max(10, Math.Min(255, _currentGamma + (delta > 0 ? 15 : -15)));
                SetGamma(_currentGamma);
                return (IntPtr)1;
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static void SetGamma(int brightness) {
        IntPtr hdc = GetDC(IntPtr.Zero);
        if (hdc != IntPtr.Zero) {
            ushort[] ramp = new ushort[3 * 256];
            for (int i = 0; i < 256; i++) {
                int val = i * brightness;
                if (val > 65535) val = 65535;
                ramp[i] = ramp[256 + i] = ramp[512 + i] = (ushort)val;
            }
            SetDeviceGammaRamp(hdc, ramp);
            ReleaseDC(IntPtr.Zero, hdc);
        }
    }

    [DllImport("gdi32.dll")]
    private static extern bool SetDeviceGammaRamp(IntPtr hDC, ushort[] lpRamp);
    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    private static IntPtr SetHook(LowLevelMouseProc proc) {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }
}
