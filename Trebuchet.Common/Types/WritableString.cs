using System;
using System.IO;
using System.Text;

namespace Trebuchet.Common.Types
{
    public class WritableString : Streamable
    {
        private int _length;
        private string _value;

        // REMINDER: strings are encoded as per minecraft protocol spec which states:
        // VarInt length + UTF-8 string
        public WritableString()
        {
        }

        public WritableString(string value)
        {
            _value = value;
            _length = value.Length;
        }

        public string Value => _value == null ? "" : _value;

        public override void Read(Stream stream)
        {
            // read VarInt length
            var varInt = new VarInt();
            varInt.Read(stream);
            _length = varInt.Value;
            if (_length == 0 || _length > 32767) throw new Exception("Invalid string length in packet");

            // read _length UTF-8 chars
            var count = 0;
            string result = "";
            while (count < _length)
            {
                result += string.Join("",stream.ReadUtf8Char()); // extension method made of magic
                count++;
            }

            _value = result;
            // what did i just make???
        }

        public override void Write(Stream stream)
        {
            // write VarInt length
            _length = Encoding.UTF8.GetBytes(_value).Length;
            var varInt = new VarInt(_length);
            varInt.Write(stream);

            // write string using utf8 encoding
            byte[] bytes = Encoding.UTF8.GetBytes(_value);
            stream.Write(bytes, 0, bytes.Length);
        }

        // to and from string
        public static implicit operator string(WritableString s)
        {
            return s.Value;
        }

        public static implicit operator WritableString(string s)
        {
            return new WritableString(s);
        }
    }
}