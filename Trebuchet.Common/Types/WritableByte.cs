using System.IO;

namespace Trebuchet.Common.Types
{
    public class WritableBoolean : Streamable
    {
        private bool _value;

        public WritableBoolean()
        {
        }

        public WritableBoolean(bool value)
        {
            _value = value;
        }

        public override void Read(Stream stream)
        {
            _value = stream.ReadByte() == 0x01;
        }

        public override void Write(Stream stream)
        {
            stream.WriteByte(_value ? (byte) 0x01 : (byte) 0x00);
        }

        public static implicit operator WritableBoolean(bool value)
        {
            return new WritableBoolean(value);
        }

        public static implicit operator bool(WritableBoolean value)
        {
            return value._value;
        }
    }
}