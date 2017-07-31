using RawInputDotNet.W32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawInputDotNet
{
    public class RawInput : IDisposable
    {
        private IntPtr processHandle;

        private Thread windowThread;
        private ushort windowClassAtom;
        private string windowClassName;

        private IntPtr windowHandle;

        private WindowNative.WindowProcedure procedure;
        private MouseKeys mouseKeysDown;

        public delegate void MouseEventHandler(object sender, MouseEventArgs e);
        public event MouseEventHandler MouseEvent;

        public RawInput()
        {
            mouseKeysDown = 0;

            windowThread = new Thread(InitializeWindow);
            windowThread.Start();

            var devices = GetDevices();

            Debug.WriteLine($"RawInput:{SizeOf<RawInputNative.RawInput>().ToString("X2")}");
            Debug.WriteLine($"RawHeader:{SizeOf<RawInputNative.RawInputHeader>().ToString("X2")}");
            Debug.WriteLine($"RawMouse:{SizeOf<RawInputNative.RawMouse>().ToString("X2")}");
            Debug.WriteLine($"RawKeyboard:{SizeOf<RawInputNative.RawKeyboard>().ToString("X2")}");
            Debug.WriteLine($"RawHID:{SizeOf<RawInputNative.RawHID>().ToString("X2")}");

            foreach (var item in devices)
            {
                var deviceInfo = GetDeviceInfo(item);
                Debug.WriteLine("...........");
                Debug.WriteLine($"DeviceInfo[{item.deviceHandle}_{deviceInfo.type}]");

                switch (deviceInfo.type)
                {
                    case RawInputNative.RawInputDeviceType.RIM_TYPEMOUSE:
                        Debug.WriteLine($"  id:                 0x{deviceInfo.mouse.id.ToString("X5")}");
                        Debug.WriteLine($"  numberOfButtons:    {deviceInfo.mouse.numberOfButtons}");
                        Debug.WriteLine($"  sampleRate:         {deviceInfo.mouse.sampleRate}");
                        Debug.WriteLine($"  hasHorizontalWheel: {deviceInfo.mouse.hasHorizontalWheel}");
                        break;
                    case RawInputNative.RawInputDeviceType.RIM_TYPEKEYBOARD:
                        Debug.WriteLine($"  type:                   0x{deviceInfo.keyboard.type.ToString("X5")}");
                        Debug.WriteLine($"  subType:                0x{deviceInfo.keyboard.subType.ToString("X5")}");
                        Debug.WriteLine($"  keyboardMode:           0x{deviceInfo.keyboard.keyboardMode.ToString("X5")}");
                        Debug.WriteLine($"  numberOfFunctionKeys:   {deviceInfo.keyboard.numberOfFunctionKeys}");
                        Debug.WriteLine($"  numberOfIndicators:     {deviceInfo.keyboard.numberOfIndicators}");
                        Debug.WriteLine($"  numberOfKeysTotal:      {deviceInfo.keyboard.numberOfKeysTotal}");
                        break;
                    case RawInputNative.RawInputDeviceType.RIM_TYPEHID:
                        Debug.WriteLine($"  vendorId:       0x{deviceInfo.hid.vendorId.ToString("X5")}");
                        Debug.WriteLine($"  productId:      0x{deviceInfo.hid.productId.ToString("X5")}");
                        Debug.WriteLine($"  versionNumber:  0x{deviceInfo.hid.versionNumber.ToString("X5")}");
                        Debug.WriteLine($"  usagePage:      {deviceInfo.hid.usagePage}");
                        Debug.WriteLine($"  usage:          {deviceInfo.hid.usage}");
                        break;
                }

                Debug.WriteLine($"RegisterDevice: {RegisterDevice(deviceInfo, windowHandle)}");
            }

        }

        private void InitializeWindow()
        {
            this.procedure = new WindowNative.WindowProcedure(WindowProcedure);

            var process = Process.GetCurrentProcess();
            this.processHandle = process.Handle;
            this.windowClassName = $"RawInputDotNetHostWindowClass{DateTime.Now.ToFileTime().ToString("X")}";

            WindowNative.WindowClass windowClass = new WindowNative.WindowClass();
            windowClass.windowProcedure = this.procedure;
            windowClass.instanceHandle = processHandle;
            windowClass.className = this.windowClassName;
            windowClassAtom = WindowNative.RegisterClass(ref windowClass);

            windowHandle = WindowNative.CreateWindowEx(0, windowClass.className, "RawInput.NET Host", 0x80000000, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, processHandle, IntPtr.Zero);

            Message message = new Message();
            int result;
            while((result = WindowNative.GetMessage(ref message, this.windowHandle, 0, 0)) != 0)
            {
                WindowNative.TranslateMessage(ref message);
                WindowNative.DispatchMessage(ref message);
            }
        }

        private uint WindowProcedure(IntPtr windowHandle, uint message, uint wParam, uint lParam)
        {
            if (message == 0x0400)
                WindowNative.DestroyWindow(this.windowHandle);

            ProcessRawInput(new Message()
            {
                HWnd = windowHandle,
                Msg = (int)message,
                WParam = (IntPtr)wParam,
                LParam = (IntPtr)lParam
            });

            return WindowNative.DefWindowProc(windowHandle, message, wParam, lParam);
        }

        private void ProcessRawInput(Message message)
        {
            if (message.Msg == 0x00FF)
            {
                var rawInput = GetInputData(message.LParam);
                
                switch (rawInput.header.type)
                {
                    case RawInputNative.RawInputDeviceType.RIM_TYPEMOUSE:
                        if (MouseEvent == null)
                            break;

                        var mouse = rawInput.data.mouse;
                        var mouseData = mouse.data;
                        MouseKeys mouseKey = 0;
                        MouseFlags mouseFlags = 0;

                        switch (mouseData.buttonFlags)
                        {
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_1_DOWN:
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_1_UP:
                                mouseKey |= MouseKeys.ButtonLeft;
                                break;
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_2_DOWN:
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_2_UP:
                                mouseKey |= MouseKeys.ButtonRight;
                                break;
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_3_DOWN:
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_3_UP:
                                mouseKey |= MouseKeys.ButtonMiddle;
                                break;
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_4_DOWN:
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_4_UP:
                                mouseKey |= MouseKeys.ButtonFour;
                                break;
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_5_DOWN:
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_BUTTON_5_UP:
                                mouseKey |= MouseKeys.ButtonFive;
                                break;
                            case RawInputNative.RawMouseButtonFlags.RI_MOUSE_WHEEL:
                                mouseKey |= MouseKeys.Wheel;
                                break;
                        }

                        if (((ushort)mouseData.buttonFlags & 0b0000_0001_0101_0101) != 0)
                        {
                            mouseFlags |= MouseFlags.Down;
                            this.mouseKeysDown |= mouseKey;
                        }
                        else if (((ushort)mouseData.buttonFlags & 0b0000_0010_1010_1010) != 0)
                        {
                            mouseFlags |= MouseFlags.Up;
                            this.mouseKeysDown &= ~mouseKey;
                        }

                        if (mouse.flags.HasFlag(RawInputNative.RawMouseFlags.MOUSE_ATTRIBUTE_CHANGED))
                            mouseFlags |= MouseFlags.AttributesChanged;

                        if (mouse.flags.HasFlag(RawInputNative.RawMouseFlags.MOUSE_VIRTUAL_DESKTOP))
                            mouseFlags |= MouseFlags.VirtualDesktop;

                        if (mouse.lastX != 0 || mouse.lastY != 0)
                        {
                            mouseFlags |= MouseFlags.Move;

                            if (mouse.flags.HasFlag(RawInputNative.RawMouseFlags.MOUSE_MOVE_ABSOLUTE))
                                mouseFlags |= MouseFlags.MoveAbsolute;
                            else
                                mouseFlags |= MouseFlags.MoveRelative;
                        }

                        MouseEventArgs args = new MouseEventArgs(mouseKey, this.mouseKeysDown, mouseFlags);
                        MouseEvent(this, args);
                        break;
                    case RawInputNative.RawInputDeviceType.RIM_TYPEKEYBOARD:
                        Debug.WriteLine($"  makeCode:           {rawInput.data.keyboard.makeCode}");
                        Debug.WriteLine($"  flags:              {rawInput.data.keyboard.flags}");
                        Debug.WriteLine($"  vKey:               {(Keys)rawInput.data.keyboard.vKey}/{rawInput.data.keyboard.vKey}");
                        Debug.WriteLine($"  message:            {rawInput.data.keyboard.message}");
                        Debug.WriteLine($"  extraInformation:   {rawInput.data.keyboard.extraInformation}");
                        break;
                }
            }
        }

        private RawInputNative.RawInput GetInputData(IntPtr inputHandle)
        {
            RawInputNative.RawInput rawInput = new RawInputNative.RawInput();
            uint rawInputHeaderSize = SizeOf<RawInputNative.RawInputHeader>();
            uint dataSize = 0;
            RawInputNative.GetRawInputData(inputHandle, RawInputNative.RawInputDataCommand.RID_INPUT, IntPtr.Zero, ref dataSize, rawInputHeaderSize);

            IntPtr data = Marshal.AllocHGlobal((int)dataSize);
            try
            {
                var size = RawInputNative.GetRawInputData(inputHandle, RawInputNative.RawInputDataCommand.RID_INPUT, data, ref dataSize, rawInputHeaderSize);
                if (size == dataSize)
                    rawInput = Marshal.PtrToStructure<RawInputNative.RawInput>(data);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Marshal.FreeHGlobal(data);
            }

            return rawInput;
        }

        private bool RegisterDevice(RawInputNative.RawInputDeviceInfo deviceInfo, IntPtr windowHandle)
        {
            RawInputNative.RawInputDevice[] device = new RawInputNative.RawInputDevice[1];
            switch (deviceInfo.type)
            {
                case RawInputNative.RawInputDeviceType.RIM_TYPEMOUSE:
                    device[0].usagePage = 1;
                    device[0].usage = 2;
                    break;
                case RawInputNative.RawInputDeviceType.RIM_TYPEKEYBOARD:
                    device[0].usagePage = 1;
                    device[0].usage = 6;
                    break;
                case RawInputNative.RawInputDeviceType.RIM_TYPEHID:
                    device[0].usagePage = deviceInfo.hid.usagePage;
                    device[0].usage = deviceInfo.hid.usage;
                    break;
            }

            device[0].windowHandleTarget = windowHandle;
            device[0].flags = RawInputNative.RawInputRegisterDeviceFlags.RIDEV_INPUTSINK;

            bool result = RawInputNative.RegisterRawInputDevices(device, (uint)device.Length, SizeOf<RawInputNative.RawInputDevice>());

            return result;
        }

        private uint GetDeviceCount()
        {
            uint deviceCount = 0;
            RawInputNative.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, SizeOf<RawInputNative.RawInputDeviceList>());

            return deviceCount;
        }

        private RawInputNative.RawInputDeviceList[] GetDevices()
        {
            var deviceCount = GetDeviceCount();
            RawInputNative.RawInputDeviceList[] devices = new RawInputNative.RawInputDeviceList[deviceCount];
            RawInputNative.GetRawInputDeviceList(devices, ref deviceCount, SizeOf<RawInputNative.RawInputDeviceList>());

            return devices;
        }

        private RawInputNative.RawInputDeviceInfo GetDeviceInfo(IntPtr deviceHandle)
        {
            uint deviceInfoSize = SizeOf<RawInputNative.RawInputDeviceInfo>();
            var deviceInfoPointer = Marshal.AllocHGlobal((int)deviceInfoSize);
            var deviceInfo = new RawInputNative.RawInputDeviceInfo();
            deviceInfo.cbSize = deviceInfoSize;
            try
            {
                Marshal.StructureToPtr(deviceInfo, deviceInfoPointer, false);
                RawInputNative.GetRawInputDeviceInfo(deviceHandle, RawInputNative.RawInputDeviceInfoCommand.RIDI_DEVICEINFO, deviceInfoPointer, ref deviceInfoSize);
                deviceInfo = Marshal.PtrToStructure<RawInputNative.RawInputDeviceInfo>(deviceInfoPointer);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Marshal.FreeHGlobal(deviceInfoPointer);
            }

            return deviceInfo;
        }

        private RawInputNative.RawInputDeviceInfo GetDeviceInfo(RawInputNative.RawInputDeviceList deviceList)
        {
            return GetDeviceInfo(deviceList.deviceHandle);
        }

        private uint SizeOf<T>()
        {
            return (uint)Marshal.SizeOf<T>();
        }

        public void Dispose()
        {
            if (this.windowHandle != IntPtr.Zero)
            {
                WindowNative.SendMessage(this.windowHandle, 0x0400, 0, 0);
                this.windowHandle = IntPtr.Zero;
            }

            if (this.windowClassAtom != 0)
            {
                WindowNative.UnregisterClass(this.windowClassName, this.processHandle);
                this.windowClassAtom = 0;
            }

            if(this.windowThread != null)
            {
                if(!this.windowThread.Join(TimeSpan.FromSeconds(1000.0)))
                    throw new TimeoutException("RawInput Thread.Join timed out");

                this.windowThread = null;
            }
        }
    }
}
