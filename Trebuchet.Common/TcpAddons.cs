using System.Linq;
using System.Net.Sockets;

// ReSharper disable InconsistentNaming

namespace Trebuchet.Common
{
    public static class TcpAddons
    {
        public static T Read<T>(this TcpClient client) where T : Streamable, new()
        {
            var result = new T();
            result.Read(client.GetStream());
            return result;
        }

        public static T Read<T>(this TcpClient client,
            char i_hate_that_method_signatures_dont_take_type_constraints_into_account) where T : struct
        {
            // idk how to fuckin do this lmao
            return new T();
        }

        public static void Write<T>(this TcpClient client, T data) where T : Streamable
        {
            data.Write(client.GetStream()); // :smil:
        }

        public static void Write<T>(this TcpClient client, T data,
            char i_hate_that_method_signatures_dont_take_type_constraints_into_account) where T : struct
        {
            var d = data.ToString().ToCharArray().Cast<byte>();
            // convert to byte[]
            var bytes = d.ToArray();
            client.GetStream().Write(bytes, 0, bytes.Length);
        }
    }
}