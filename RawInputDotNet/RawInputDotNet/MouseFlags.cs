using System;

namespace RawInputDotNet
{
    [Flags]
    public enum MouseFlags : byte
    {
        Down = 0x01,
        Up = 0x02,
        AttributesChanged = 0x04,
        Move = 0x08,
        MoveRelative = 0x10,
        MoveAbsolute = 0x20,
        VirtualDesktop = 0x40
    }
}
