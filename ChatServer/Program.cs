using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace ChatServer
{
    class Program
    {
        public static Hashtable Client_List = new Hashtable();

        static void Main(string[] args)
        {
            TcpClient clientSocket = default(TcpClient);
            TcpListener serverSocket = new TcpListener(8888);
            int count;

            serverSocket.Start();
            Console.WriteLine("Chat Server Started ...");
            count = 0;

            while (true)
            {
                count++;
                clientSocket = serverSocket.AcceptTcpClient();

                byte[] byteFrom = new byte[10025];
                string ClientData = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(byteFrom, 0, (int)clientSocket.ReceiveBufferSize);
                ClientData = System.Text.Encoding.ASCII.GetString(byteFrom);
                ClientData = ClientData.Substring(0, ClientData.IndexOf("$"));

                Client_List.Add(ClientData, clientSocket);

                Broadcast($"{ClientData} Joined", ClientData, false);
                Console.WriteLine($"{ClientData} joined this chat!!!");
                handleClient client = new handleClient();
                client.startClient(clientSocket, ClientData, Client_List);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("Exiting Chat Server.");
            Console.ReadLine();
        }

        public static void Broadcast(string msg, string UserName, bool flag)
        {
            foreach (DictionaryEntry Item in Client_List)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag)
                    broadcastBytes = Encoding.ASCII.GetBytes($"{UserName}: {msg}"); //TO DO: ADD Time??
                else
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
    }
}
