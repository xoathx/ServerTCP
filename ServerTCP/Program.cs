using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace ServerTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 8888;
            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);


            tcpSocket.Listen(5);


            Console.WriteLine("Start listening...");


            WorkFile fileEdit = new WorkFile("testFile");


            while (true)
            {
                
                var listenner = tcpSocket.Accept();
                byte[] buffer = new byte[256];
                var size = 0;
                StringBuilder data = new StringBuilder();
                string move;
                do
                {
                    size = listenner.Receive(buffer);
                    TcpMessage tcpMessage = JsonConvert.DeserializeObject<TcpMessage>(Encoding.UTF8.GetString(buffer));
                    move = tcpMessage.Move;
                    data.Append(tcpMessage.MessageToAdd);
                }
                while (listenner.Available > 0);


                if (move == "add")
                {
                    fileEdit.StirngAdd(data.ToString());
                    Console.WriteLine("Строка добавлена.");
                }
                if (move == "del")
                    fileEdit.Drop();



                listenner.Shutdown(SocketShutdown.Both);

                listenner.Close();
            }
        }
    }

    class WorkFile
    {
        private string filename;
        public WorkFile(string name)
        {
            filename = name + ".txt";
            if (!(File.Exists(filename)))
            {
                using (StreamWriter sw = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)))
                {
                    sw.WriteLine();
                    sw.Close();
                }
            }

        }

        public void StirngAdd(string str)
        {
            if (File.Exists(filename))
            {
                File.AppendAllText(filename, str + Environment.NewLine);


            }
        }

        public void Drop()
        {
            File.Delete(filename);
            Console.WriteLine("Файл удален.");
        }
    }
}
