using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RawInputDotNet.W32
{
    internal class WindowNative
    {
        public delegate uint WindowProcedure(
               [In] IntPtr windowHandle,
               [In] uint message,
               [In] uint wParam,
               [In] uint lParam
           );

        public struct WindowClass
        {
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)] public WindowProcedure windowProcedure;
            public int classExtraBytes;
            public int windowExtraBytes;
            public IntPtr instanceHandle;
            public IntPtr iconHandle;
            public IntPtr cursorHandle;
            public IntPtr brushHandleBackground;
            public string menuName;
            public string className;
        }

        [DllImport("User32.dll")]
        public static extern uint DefWindowProc(
                [In] IntPtr windowHandle,
                [In] uint message,
                [In] uint wParam,
                [In] uint lParam
            );

        [DllImport("User32.dll")]
        public static extern ushort RegisterClass(
                [In] ref WindowClass windowClass
            );

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms644899(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern bool UnregisterClass(
                [In]            string className,
                [In, Optional]  IntPtr instanceHandle
            );

        [DllImport("User32.dll")]
        public static extern IntPtr CreateWindowEx(
                [In, Optional]  uint extendedStyle,
                [In, Optional]  string className,
                [In, Optional]  string windowName,
                [In]            uint style,
                [In]            int x,
                [In]            int y,
                [In]            int width,
                [In]            int height,
                [In, Optional]  IntPtr parentWindowHandle,
                [In, Optional]  IntPtr menuHandle,
                [In, Optional]  IntPtr instanceHandle,
                [In, Optional]  IntPtr param
            );

        [DllImport("User32.dll")]
        public static extern bool DestroyWindow(
                [In] IntPtr windowHandle
            );

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms644950(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern bool SendMessage(
                [In] IntPtr windowHandle,
                [In] uint message,
                [In] uint wParam,
                [In] uint lParam
            );

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms644936(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern int GetMessage(
                [In, Out]       ref Message message,
                [In, Optional]  IntPtr windowHandle,
                [In]            uint messageFilterMin,
                [In]            uint messageFilterMax
            );

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms644955(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern bool TranslateMessage(
                [In, Out] ref Message message
            );

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms644934%28v=vs.85%29.aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern bool DispatchMessage(
                [In, Out] ref Message message
            );
    }
}
