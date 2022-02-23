using System.IO;
using System.Text;

namespace Trebuchet.Common
{
    public static class StreamAddons
    {
        public static char[] ReadUtf8Char(this Stream stream)
        {
            byte[] bytes = new byte[4];
            var enc = new UTF8Encoding(false, true);
            if (1 != stream.Read(bytes, 0, 1))
                return null;
            if (bytes[0] <= 0x7F) //Single byte character
            {
                return enc.GetChars(bytes, 0, 1);
            }

            var remainingBytes =
                (bytes[0] & 240) == 240 ? 3 : (bytes[0] & 224) == 224 ? 2 : (bytes[0] & 192) == 192 ? 1 : -1;
            if (remainingBytes == -1)
                return null;
            stream.Read(bytes, 1, remainingBytes);
            return enc.GetChars(bytes, 0, remainingBytes + 1);
        }
    }
}