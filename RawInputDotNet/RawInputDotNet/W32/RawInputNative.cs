using System;
using System.Runtime.InteropServices;

namespace RawInputDotNet.W32
{
    internal class RawInputNative
    {
        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645565(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum RawInputRegisterDeviceFlags : uint
        {
            RIDEV_REMOVE = 0x00000001,
            RIDEV_EXCLUDE = 0x00000010,
            RIDEV_PAGEONLY = 0x00000020,
            RIDEV_NOLEGACY = 0x00000030,
            RIDEV_INPUTSINK = 0x00000100,
            RIDEV_CAPTUREMOUSE = 0x00000200,
            RIDEV_APP_KEYS = 0x00000400,
            RIDEV_EXINPUTSINK = 0x00001000,
            RIDEV_DEVNOTIFY = 0x00002000
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645597(v=vs.85).aspx
        /// </summary>
        public enum RawInputDeviceInfoCommand : uint
        {
            RIDI_PREPARSEDATA = 0x20000005,
            RIDI_DEVICENAME = 0x20000007,
            RIDI_DEVICEINFO = 0x2000000b
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645581(v=vs.85).aspx
        /// </summary>
        public enum RawInputDeviceType : uint
        {
            RIM_TYPEMOUSE = 0,  // Usage Page 1, Usage 2 
            RIM_TYPEKEYBOARD = 1,  // Usage Page 1, Usage 6
            RIM_TYPEHID = 2   // Usage Page x, Usage y (Defined in struct)
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645578(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum RawMouseFlags : ushort
        {
            MOUSE_MOVE_RELATIVE = 0x00,
            MOUSE_MOVE_ABSOLUTE = 0x01,
            MOUSE_VIRTUAL_DESKTOP = 0x02,
            MOUSE_ATTRIBUTE_CHANGED = 0x04
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645578(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum RawMouseButtonFlags : ushort
        {
            RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001,
            RI_MOUSE_BUTTON_1_DOWN = 0x0001,

            RI_MOUSE_LEFT_BUTTON_UP = 0x0002,
            RI_MOUSE_BUTTON_1_UP = 0x0002,

            RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004,
            RI_MOUSE_BUTTON_2_DOWN = 0x0004,

            RI_MOUSE_RIGHT_BUTTON_UP = 0x0008,
            RI_MOUSE_BUTTON_2_UP = 0x0008,

            RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010,
            RI_MOUSE_BUTTON_3_DOWN = 0x0010,

            RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020,
            RI_MOUSE_BUTTON_3_UP = 0x0020,

            RI_MOUSE_BUTTON_4_DOWN = 0x0040,

            RI_MOUSE_BUTTON_4_UP = 0x0080,

            RI_MOUSE_BUTTON_5_DOWN = 0x0100,

            RI_MOUSE_BUTTON_5_UP = 0x0200,

            RI_MOUSE_WHEEL = 0x0400
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645575(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum RawKeyboardFlags : ushort
        {
            RI_KEY_MAKE = 0x00,
            RI_KEY_BREAK = 0x01,
            RI_KEY_E0 = 0x02,
            RI_KEY_E1 = 0x04
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645596(v=vs.85).aspx
        /// </summary>
        public enum RawInputDataCommand
        {
            RID_INPUT = 0x10000003,
            RID_HEADER = 0x10000005
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645565(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputDevice
        {
            public ushort usagePage;
            public ushort usage;
            public RawInputRegisterDeviceFlags flags;
            public IntPtr windowHandleTarget;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645589(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputDeviceInfoMouse
        {
            public uint id;
            public uint numberOfButtons;
            public uint sampleRate;
            public bool hasHorizontalWheel;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645587(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputDeviceInfoKeyboard
        {
            public uint type;
            public uint subType;
            public uint keyboardMode;
            public uint numberOfFunctionKeys;
            public uint numberOfIndicators;
            public uint numberOfKeysTotal;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645584(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputDeviceInfoHID
        {
            public uint vendorId;
            public uint productId;
            public uint versionNumber;
            public ushort usagePage;
            public ushort usage;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645581(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct RawInputDeviceInfo
        {
            [FieldOffset(0)] public uint cbSize;
            [FieldOffset(4)] public RawInputDeviceType type;
            [FieldOffset(8)] public RawInputDeviceInfoMouse mouse;
            [FieldOffset(8)] public RawInputDeviceInfoKeyboard keyboard;
            [FieldOffset(8)] public RawInputDeviceInfoHID hid;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645571(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputHeader
        {
            public RawInputDeviceType type;
            public uint size;
            public IntPtr deviceHandle;
            public IntPtr wParam;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645568(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputDeviceList
        {
            public IntPtr deviceHandle;
            public RawInputDeviceType type;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645578(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct RawMouseData
        {
            [FieldOffset(0)] public uint buttons;
            [FieldOffset(0)] public RawMouseButtonFlags buttonFlags;
            [FieldOffset(2)] public ushort buttonData;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645578(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawMouse
        {
            public RawMouseFlags flags;
            public RawMouseData data;
            public uint rawButtons;
            public int lastX;
            public int lastY;
            public uint extraInformation;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645575(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawKeyboard
        {
            public ushort makeCode;
            public RawKeyboardFlags flags;
            public ushort reserved;
            public ushort vKey;
            public uint message;
            public uint extraInformation;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645549(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawHID
        {
            public uint sizeHID;
            public uint count;
            //public IntPtr rawData;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645562(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct RawInputData
        {
            [FieldOffset(0)] public RawMouse mouse;
            [FieldOffset(0)] public RawKeyboard keyboard;
            [FieldOffset(0)] public RawHID hid;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645562(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RawInput
        {
            public RawInputHeader header;
            public RawInputData data;
        }

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645600(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern bool RegisterRawInputDevices(RawInputDevice[] rawInputDevices, uint numDevices, uint cbSize);


        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645597(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern uint GetRawInputDeviceInfo
        (
            [In, Optional]      IntPtr deviceHandle,
            [In]                RawInputDeviceInfoCommand command,
            [In, Out, Optional] IntPtr data,
            [In, Out]           ref uint cbSize
        );

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645598(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern uint GetRawInputDeviceList
        (
            [Out, Optional] IntPtr rawInputDeviceList,
            [In, Out]       ref uint numDevices,
            [In]            uint cbSize
        );
        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645598(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern uint GetRawInputDeviceList
        (
            [Out, Optional] RawInputDeviceList[] rawInputDeviceList,
            [In, Out]       ref uint numDevices,
            [In]            uint cbSize
        );

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms645596(v=vs.85).aspx
        /// </summary>
        [DllImport("User32.dll")]
        public static extern uint GetRawInputData
        (
            [In]            IntPtr rawInputHandle,
            [In]            RawInputDataCommand command,
            [Out, Optional] IntPtr data,
            [In, Out]       ref uint cbSize,
            [In]            uint cbSizeHeader
        );
    }
}
