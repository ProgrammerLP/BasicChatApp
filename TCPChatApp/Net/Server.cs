using System.Net.Sockets;
using TCPChatApp.Net.IO;

namespace TCPChatApp.Net
{
    class Server
    {
        TcpClient client;
        public PacketReader packetReader;

        public event Action ConnectedEvent;
        public event Action MessageReceivedEvent;
        public event Action DisonnectedEvent;

        public Server()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 1312);
                packetReader = new PacketReader(client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMSG(username);
                    client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opCode = packetReader.ReadByte();
                    switch (opCode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke();
                            break;
                        case 5:
                            MessageReceivedEvent?.Invoke();
                            break;
                        case 10:
                            DisonnectedEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("oh no :(\nsomething went wrong (1)");
                            break;
                    }
                }
            });
        }

        public void SendMSGToServer(string msg)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WriteOpCode(5);
            msgPacket.WriteMSG(msg);
            client.Client.Send(msgPacket.GetPacketBytes());
        }
    }
}
