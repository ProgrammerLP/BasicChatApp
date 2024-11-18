using System.Net.Sockets;
using System.Text;

namespace TCPChatServer.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream netS;

        public PacketReader(NetworkStream netS) : base(netS)
        {
            this.netS = netS;
        }

        public string ReadMSG()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            netS.Read(msgBuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }
    }
}
