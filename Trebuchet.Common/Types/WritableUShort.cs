using System;
using System.IO;

namespace Trebuchet.Common.Types
{
    public class WritableUShort : Streamable
    {
        private ushort _value;

        public WritableUShort(ushort value)
        {
            _value = value;
        }

        public WritableUShort()
        {
        }

        public ushort Value => (ushort) (_value == null ? 0 : _value);

        public override void Read(Stream stream)
        {
            // ushort are 2 bytes, so this is really easy (compared to strings, which i just made)

            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            _value = BitConverter.ToUInt16(buffer, 0);
        }

        public override void Write(Stream stream)
        {
            var buffer = BitConverter.GetBytes(_value);
            stream.Write(buffer, 0, 2);
        }

        // implicit conversion to ushort
        public static implicit operator ushort(WritableUShort w)
        {
            return w.Value;
        }

        // implicit conversion from ushort
        public static implicit operator WritableUShort(ushort u)
        {
            return new WritableUShort(u);
        }
    }
}