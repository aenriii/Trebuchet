using System.IO;
using System.Runtime.InteropServices;

namespace Trebuchet.Common.Types
{
    public class VarInt : Streamable
    {
        private int _value;

        public VarInt()
        {
        }

        public VarInt(int value) // allow for creation of a VarInt from an int
        {
            _value = value;
        }

        public int Value => _value == null ? 0 : _value;

        public override void Read(Stream stream)
        {
            // read VarInt into int _value

            // |> get stream and wait for data
            var value = 0;
            var length = 0;
            byte currentByte;

            while (true)
            {
                currentByte = (byte) stream.ReadByte();
                value |= (currentByte & 0x7F) << (length * 7);

                length += 1;
                if (length > 5) throw new InvalidOleVariantTypeException("VarInt is too big");

                if ((currentByte & 0x80) != 0x80) break;
            }

            _value = value;
        }

        public override void Write(Stream stream)
        {
            var value = _value;
            // convert int to byte array
            byte[] bytes = new byte[5];
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
            stream.Write(bytes, 0, 5); // i think this is the right way to do it but i'm not sure
        }

        public static implicit operator int(VarInt v) // implicit int conversion operator
        {
            return v.Value;
        }

        public static implicit operator VarInt(int v) // implicit int conversion operator
        {
            return new VarInt(v);
        }
    }
}