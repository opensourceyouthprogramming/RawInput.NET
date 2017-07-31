using System;

namespace RawInputDotNet
{
    public class MouseEventArgs : EventArgs
    {
        public MouseKeys Key { get; private set; }
        public MouseKeys AllKeys { get; private set; }
        public MouseFlags Flags { get; private set; }

        public MouseEventArgs(MouseKeys key, MouseKeys allKeys, MouseFlags flags)
            : base()
        {
            Key = key;
            AllKeys = allKeys;
            Flags = flags;
        }
    }
}
