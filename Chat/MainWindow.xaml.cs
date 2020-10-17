using System;
using System.Text;
using System.Windows;
using System.Net.Sockets;
using System.Threading;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string ReadData = null;
        Exception ex = new Exception();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServConnect_Click(object sender, RoutedEventArgs e)
        {
            byte[] outStream = Encoding.ASCII.GetBytes($"{NickName.Text}$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void SendMsg_Click(object sender, RoutedEventArgs e)
        {
            ReadData = "Connected to Chat Server ...";
            msg();
            try
            {

                clientSocket.Connect("127.0.0.1", 8888);
                serverStream = clientSocket.GetStream();

                byte[] outStream = Encoding.ASCII.GetBytes($"{MsgBox.Text}$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Thread ctThread = new Thread(GetMessage);
                ctThread.Start();
            }
            catch
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                ReadData = "" + returndata;
                msg();
            }
        }

        private void msg()
        {
            try
            {
                ChatBox.Text = $"{ChatBox.Text}{Environment.NewLine}>>{ReadData}";
            }
            catch
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
