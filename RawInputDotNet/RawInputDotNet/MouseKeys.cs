using System;

namespace RawInputDotNet
{
    [Flags]
    public enum MouseKeys : ushort
    {
        ButtonLeft = 0x01,
        ButtonRight = 0x02,
        ButtonMiddle = 0x04,
        ButtonFour = 0x08,
        ButtonFive = 0x10,
        Wheel = 0x20
    }
}
