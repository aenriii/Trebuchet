using System.IO;
using System.Runtime.InteropServices;

namespace Trebuchet.Common.Types
{
    public class VarLong : Streamable
    {
        private long _value;

        public VarLong()
        {
        }

        public VarLong(long value) // allow for creation of a VarLong from a long
        {
            _value = value;
        }

        public long Value => _value == null ? 0 : _value;

        public override void Read(Stream stream)
        {
            // read VarLong into int _value

            // |> get stream and wait for data
            long value = 0;
            var length = 0;
            byte currentByte;

            while (true)
            {
                currentByte = (byte) stream.ReadByte();
                value |= (currentByte & 0x7F) << (length * 7);

                length += 1;
                if (length > 10) throw new InvalidOleVariantTypeException("VarLong is too big");

                if ((currentByte & 0x80) != 0x80) break;
            }

            _value = value;
        }

        public override void Write(Stream stream)
        {
            var value = _value;
            // convert int to byte array
            byte[] bytes = new byte[10];
            var index = 0;
            while (true)
            {
                bytes[index] = (byte) (value & 0x7F);
                value >>= 7;
                if (value != 0)
                    bytes[index] |= 0x80;
                else
                    break;
                index += 1;
            }

            // write byte array to stream
            stream.Write(bytes, 0, 10); // i think this is the right way to do it but i'm not sure
        }

        public static implicit operator long(VarLong value)
        {
            return value.Value;
        }

        public static implicit operator VarLong(long value)
        {
            return new VarLong(value);
        }
    }
}