// Simple test for handshaking packets


using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Trebuchet.Common;
using Trebuchet.Common.Types;
using Trebuchet.Common.Types.Respond;
using Vlingo.Xoom.UUID;

namespace Trebuchet.Server
{
    public class Program
    {
        public const int ProtocolVersion = 757;

        public Program(TcpClient client)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Client connected");
            var stream = client.GetStream();
            // read handshake
            var length_of_rest = client.Read<VarInt>() - 5; // 5 is the length of the Packet Id, which is another VarInt
            // We can ignore the length, as we know what types we need to
            // look for and their associated lengths (given the packet id
            // is 0x00/handshake)
            int packet_id = client.Read<VarInt>();
            if (packet_id != 0x00) // only accepting first packet as state: Handshaking
            {
                Console.WriteLine("Invalid packet id, hexa: " + packet_id.ToString("X"));
                client.Close();
                return;
            }

            var protocol_version = client.Read<VarInt>();
            var server_address = client.Read<WritableString>();
            var server_port = client.Read<WritableUShort>();
            var next_state = client.Read<VarInt>();
            if (server_port < 0 || server_port > 65535)
                Console.WriteLine("bro HOW (Invalid port: " + server_port + ")");
            Console.WriteLine("Protocol version: " + (int) protocol_version);
            Console.WriteLine("Server address: " + server_address);
            Console.WriteLine("Server port: " + (ushort) server_port);
            Console.WriteLine("Next state: " + (int) next_state);
            if (protocol_version != ProtocolVersion)
            {
                Console.WriteLine("Invalid protocol version: " + (int) protocol_version);
                client.Close();
                return;
            }

            switch (next_state)
            {
                case 1:
                    // set state to Status
                    break; // This is the only thing happening, so we can just return
                case 2:
                    // close, as we don't support login
                    Console.WriteLine("Login not supported, closing client connection");
                    client.Close();
                    return;
            }
            // State is now Status

            Console.WriteLine("Swapped to state:status in " + stopwatch.ElapsedMilliseconds + "ms");
            // read Request packet
            length_of_rest = client.Read<VarInt>();
            packet_id = client.Read<VarInt>();
            if (packet_id != 0x00)
            {
                Console.WriteLine("Invalid packet id, hexa: " + packet_id.ToString("X"));
                client.Close();
                return;
            }

            // return Response packet with ServerListPing object
            var response = new MemoryStream();
            new VarInt(0x00).Write(response);
            ServerListPing ping = new(
                new VersionData(
                    "1.18.1",
                    ProtocolVersion),
                new PlayerData(6969, 69, new[]
                {
                    new SmallUserRecord("jai", new RandomBasedGenerator().GenerateGuid().ToString())
                }),
                new Description("Running on the Trebuchet Server"));
            Console.WriteLine("Encoding ping, watch this...");
            Console.WriteLine(ping.Encode());
            new WritableString(ping.Encode()).Write(response);
            Console.WriteLine("Encoded ping, done!");
            VarInt length = new((int) response.Length);
            length.Write(stream);
            response.WriteTo(stream);
            stream.Flush();
            Console.WriteLine("Sent ping response in " + stopwatch.ElapsedMilliseconds + "ms");
            // get Ping packet
            length_of_rest = client.Read<VarInt>();
            Console.WriteLine("Got ping packet, length: " + length_of_rest);
            packet_id = client.Read<VarInt>();
            Console.WriteLine("Packet id: " + packet_id);
            if (packet_id != 0x01)
            {
                Console.WriteLine("Invalid packet id, hexa: " + packet_id.ToString("X"));
                client.Close();
                return;
            }

            // we know that the Ping is a VarLong, so we can just read it
            var ping_time = client.Read<VarLong>();
            Console.WriteLine("Ping time: " + ping_time);
            // send Pong packet
            var pong = new MemoryStream();
            new VarInt(0x01).Write(pong);
            ping_time.Write(pong);
            new VarInt((int) pong.Length).Write(pong);
            pong.WriteTo(stream);
            stream.Flush();
            Console.WriteLine("Sent pong in " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Client handled perfectly in " + stopwatch.ElapsedMilliseconds + "ms");
            client.Close();
            stopwatch.Stop();
        }

        public static void Main(string[] args)
        {
            // setup tcp listener
            TcpListener listener = new(IPAddress.Any, 8080);
            listener.Start();
            Console.WriteLine("Listening on port 8080");
            Console.WriteLine("Running Trebuchet Server");
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Accepted connection from " + client.Client.RemoteEndPoint);
                new Program(client);
            }
        }
    }
}