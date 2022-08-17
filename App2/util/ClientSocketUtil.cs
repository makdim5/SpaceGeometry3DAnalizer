using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace App2.util
{
    internal class ClientSocketUtil
    {
        private static Process pr;
        static int port = 8005; 
        static string address = "127.0.0.1"; 
        public static string SendMsgToServer(string message)
        {
            string answer = "";
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                using (var socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp))
                {
                    // подключаемся к удаленному хосту
                    socket.Connect(ipPoint);
                    
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    socket.Send(data);

                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    answer = builder.ToString();

                    
                    socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return answer;
        }

        public static void RunCommandServer()
        {
            pr = Process.Start(@"C:\Users\makan\Desktop\App1\ConsoleApp1\bin\Debug\ConsoleApp1.exe");
        }

        public static void FinishCommandServer()
        {
            pr.Kill();
        }
    }
}
