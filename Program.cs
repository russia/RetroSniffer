using SnifferRetro.Helpers;
using SnifferRetro.Network;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SnifferRetro
{
    public class Program
    {
        public static readonly string GamePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ankama\\zaap\\retro\\resources\\app\\retroclient\\Dofus.exe";

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "RetroSniffer";
            StartTcpListening();
            int processId = StartGame();
            StartDllInjection(processId);
            Console.ReadKey();
        }

        public static void StartTcpListening()
        {
            ConsoleWriteColoredLine("Starting Tcp listener..", ConsoleColor.Gray);
            var TcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8541));
            TcpListener.Start();
            var listeningThread = new Thread(new ThreadStart(() =>
            {
                Console.WriteLine("TcpListener waiting to accept tcp client.. ");
                var mitmObj = new Mitm(TcpListener.AcceptTcpClient());
                Console.WriteLine("Client accepted !");
            }));
            listeningThread.Start();
        }

        public static int StartGame()
        {
            ConsoleWriteColoredLine("Starting game..", ConsoleColor.Gray);
            if (!File.Exists(GamePath))
                Environment.FailFast("Can't find game exe file.");
            Process gameProcess = Process.Start(GamePath);
            gameProcess.WaitForInputIdle();
            ConsoleWriteColoredLine("Game started succesfully !", ConsoleColor.Cyan);
            return gameProcess.Id;
        }

        public static void StartDllInjection(int processId)
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\winsock-patcher.dll"))
                Environment.FailFast("Can't find winsock dll file.");
            Console.WriteLine(Injector.Inject(processId, Environment.CurrentDirectory + "\\winsock-patcher.dll"));
        }

        public static void ConsoleWriteColoredLine(string strMessage, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(strMessage);
            Console.ResetColor();
        }
    }
}