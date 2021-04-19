using Microsoft.VisualBasic;
using RetroSniffer.Network;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SnifferRetro.Network
{
    public class Mitm
    {
        public ClientConnection ClientTcpConnection;
        public ServerConnection ServerTcpConnection;

        public Mitm(TcpClient clientTcpClient)
        {
            ServerTcpConnection = new ServerConnection(this, "172.65.206.193", 443);
            ClientTcpConnection = new ClientConnection(this, clientTcpClient);
        }

        public void PacketHandler(byte[] buffer, ClientConnection sender) //this should be replaced according to your needs
        {
            string datas = Encoding.ASCII.GetString(buffer);
            string[] array = datas.Split('\0').Where(x => x != "").ToArray();
            foreach (var data in array)
            {
                ServerTcpConnection.SendPacketToServer(data); //receiver
                Program.ConsoleWriteColoredLine($"{DateAndTime.TimeString} - Client => {data.Replace("\n", "")}", ConsoleColor.Cyan);
            }
        }

        public void PacketHandler(byte[] buffer, ServerConnection sender) //this should be replaced according to your needs
        {
            string datas = Encoding.ASCII.GetString(buffer);
            string[] array = datas.Split('\0').Where(x => x != "").ToArray();
            foreach (var data in array)
            {
                ClientTcpConnection.SendPacketToClient(data); //receiver
                Program.ConsoleWriteColoredLine($"{DateAndTime.TimeString} - Server <= {data.Replace("\n", "")}", ConsoleColor.White);
            }
        }
    }
}