using System.Net;
using System.Net.Sockets;
using TCPChatServer.Net.IO;

namespace TCPChatServer
{
    internal class Program
    {
        static List<Client> clients;
        static TcpListener listener;

        static void Main(string[] args)
        {
            clients = new List<Client>();
            listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 1312);
            listener.Start();

            while (true)
            {
                var client = new Client(listener.AcceptTcpClient());
                clients.Add(client);

                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach (var client in clients)
            {
                foreach (var clt in clients)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMSG(clt.Username);
                    broadcastPacket.WriteMSG(clt.UID.ToString());
                    client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }

        }

        public static void BroadcastMessage(string msg)
        {
            foreach (var client in clients)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMSG(msg);
                client.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedClient = clients.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            clients.Remove(disconnectedClient);

            foreach (var client in clients)
            {
                var uidPacket = new PacketBuilder();
                uidPacket.WriteOpCode(10);
                uidPacket.WriteMSG(uid);
                client.ClientSocket.Client.Send(uidPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{DateTime.Now}]: {disconnectedClient.Username} left the chat!");
        }
    }
}
