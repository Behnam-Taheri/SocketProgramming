using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SocketDemoSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocketsController : ControllerBase
    {
        [HttpPost("{message}")]
        public void Send([FromRoute ]string message)
        {
            string serverIP = "127.0.0.1";
            int serverPort = 8080;

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
}
