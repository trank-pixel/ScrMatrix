using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management; // Не забудь добавить /r:System.Management.dll при сборке

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
                ChangeWmiBrightness(delta > 0 ? 5 : -5);
                return (IntPtr)1;
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static void ChangeWmiBrightness(int offset) {
        try {
            var scope = new ManagementScope("root\\wmi");
            using (var searcher = new ManagementObjectSearcher(scope, new SelectQuery("WmiMonitorBrightness"))) {
                foreach (ManagementObject obj in searcher.Get()) {
                    int current = (byte)obj["CurrentBrightness"];
                    int target = Math.Max(0, Math.Min(100, current + offset));
                    using (var classInstance = new ManagementClass(scope, new ManagementPath("WmiMonitorBrightnessMethods"), null)) {
                        foreach (ManagementObject methodObj in classInstance.GetInstances()) {
                            methodObj.InvokeMethod("WmiSetBrightness", new object[] { 1, (byte)target });
                        }
                    }
                }
            }
        } catch { /* err if WMI not supported */ }
    }

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
