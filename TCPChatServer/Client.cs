using System.Net.Sockets;
using TCPChatServer.Net.IO;

namespace TCPChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader packetReader;

        public Client(TcpClient client)
        {
            this.ClientSocket = client;
            this.UID = Guid.NewGuid();
            packetReader = new PacketReader(ClientSocket.GetStream());

            var opCode = packetReader.ReadByte();
            Username = packetReader.ReadMSG();

            Console.WriteLine($"[{DateTime.Now.ToLongDateString()}]: {Username} joined the chat!");
            Program.BroadcastMessage($"[{DateTime.Now}]: {Username} joined the chat!");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opCode = packetReader.ReadByte();
                    switch (opCode)
                    {
                        case 5:
                            var msg = packetReader.ReadMSG();
                            Console.WriteLine($"[{DateTime.Now}]: Message received");
                            Program.BroadcastMessage($"[{DateTime.Now}]: {Username}: {msg}");
                            break;
                        default:
                            Console.WriteLine("oh no :(\nsomething went wrong (2)");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID}]: Disconnected");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
