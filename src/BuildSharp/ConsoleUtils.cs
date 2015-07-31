using System;
using System.Runtime.InteropServices;

namespace BuildSharp
{
    public static class ConsoleUtils
    {
        public enum CtrlEventType
        {
            CtrlCEvent = 0,
            CtrlBreakEvent = 1,
            CtrlCloseEvent = 2,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent = 6
        }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventHandler handler,
            bool add);

        public delegate bool ConsoleEventHandler(CtrlEventType sig);

        static bool IsWindows
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return true;
                }

                return false;
            }
        }

        public static void SetupExitHandler(ConsoleEventHandler handler)
        {
            if (!IsWindows)
                return;

            SetConsoleCtrlHandler(handler, add: true);
        }
    }
}
