using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SocketDemoReceiver
{
    public class SocketReceiver : BackgroundService
    {
        private int serverPort = 8080;
        private void Receive()
        {
            // ایجاد سوکت و بایند به پورت
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, serverPort);
            listener.Bind(localEP);

            // شروع گوش دادن به درخواست‌ها
            listener.Listen(10);

            // دریافت اطلاعات از سوکت
            Socket handler = listener.Accept();
            byte[] buffer = new byte[1024];
            int bytesRec = handler.Receive(buffer);
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRec);
            handler.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
            // نمایش اطلاعات دریافت شده
            Console.WriteLine("Received: {0}", data);

            // بستن سوکت‌ها
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            listener.Close();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    Receive();
                }
                catch (Exception)
                {
                    
                }
            }
        }
    }
}
