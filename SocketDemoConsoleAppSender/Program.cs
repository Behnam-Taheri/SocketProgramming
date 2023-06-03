using System.Net.Sockets;
using System.Net;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            try
            {
                Send();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }

    private static void Send()
    {
        string serverIP = "127.0.0.1";
        int serverPort = 8000;

        Console.WriteLine("Write Your Message");
        string message = Console.ReadLine();

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress ipAddress = IPAddress.Parse(serverIP);
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, serverPort);
        socket.Connect(remoteEP);

        byte[] data = Encoding.ASCII.GetBytes(message);
        socket.Send(data);

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}