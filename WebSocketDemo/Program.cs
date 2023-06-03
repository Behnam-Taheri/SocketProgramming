using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;

namespace WebSocketServer
{
    class Program
    {
        private static ConcurrentDictionary<string, WebSocket> WebSockets = new();
        private const string ClientId = "HHO";
        static async Task Main(string[] args)
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/");
            httpListener.Start();

            Console.WriteLine("Server started. Listening for incoming connections...");

            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                   await ProcessWebSocketRequest(context);

                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        static async Task ProcessWebSocketRequest(HttpListenerContext context)
        {
            // WebSocketContext webSocketContext = null;

            try
            {
                var webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                Console.WriteLine("WebSocket connection established.");

                byte[] receiveBuffer = new byte[1024];
                WebSocketReceiveResult receiveResult = await webSocketContext.WebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);


                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    OnConnected(webSocketContext.WebSocket, "HHO");
                    await SendResponse(new MessageObject("red", "this is red"), ClientId);
                }

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    await OnDisConnected(webSocketContext.WebSocket, ClientId);

            }
            catch (Exception ex)
            {
                Console.WriteLine("WebSocket connection failed. Error: " + ex.Message);
                context.Response.StatusCode = 500;
                context.Response.Close();
                return;
            }

            //WebSocket webSocket = webSocketContext.WebSocket;

            //while (webSocket.State == WebSocketState.Open)
            //{
            //    try
            //    {
            //        byte[] receiveBuffer = new byte[1024];
            //        WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            //        if (receiveResult.MessageType == WebSocketMessageType.Text)
            //        {
            //            string receivedData = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
            //            Console.WriteLine("Received: " + receivedData);

            //            string responseData = "You sent: " + receivedData;
            //            byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(responseData);
            //            await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            //        }
            //        else if (receiveResult.MessageType == WebSocketMessageType.Close)
            //        {
            //            Console.WriteLine("WebSocket connection closed by client.");
            //            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("WebSocket error. Error: " + ex.Message);
            //    }
            // }
        }


        static void OnConnected(WebSocket webSocket, string clientId)
        {
            WebSockets.AddOrUpdate(clientId, webSocket, (key, oldValue) => webSocket);
        }

        static async Task OnDisConnected(WebSocket webSocket, string clientId)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            WebSockets.TryRemove(clientId, out WebSocket _);
        }

        static async Task SendResponse<T>(T message, string clientId)
        {
            var webSocket = WebSockets.GetValueOrDefault(clientId);
            var jsonMessage = JsonConvert.SerializeObject(message);
            byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(jsonMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

public record MessageObject(string color, string Message);