using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NGDtuanh.Auth.Desktop {
    public static partial class LoopbackListenHelper {
        public static HttpListener CreateAndStart(out string redirectUri) {
            int port;

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                port = ((IPEndPoint)socket.LocalEndPoint).Port;
            }

            redirectUri = $"http://{IPAddress.Loopback}:{port}/";
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri);
            listener.Start();

            return listener;
        }

        public static void SendHtmlResponse(HttpListenerResponse response) {
            byte[] buffer = Encoding.UTF8.GetBytes(@"
                <html>
                <head>
                    <style>
                        :root {
                            color-scheme: light dark;
                        }
                        body {
                            background-color: Canvas;
                            color: CanvasText;
                            font-family: system-ui, -apple-system, sans-serif;
                            text-align: center;
                            padding-top: 50px;
                            transition: background-color 0.3s, color 0.3s;
                        }
                        h1 {
                            color: #4CAF50;
                        }
                    </style>
                </head>
                <body>
                    <h1>Authentication Successful!</h1>
                    <p>You can now close this window and return to the game.</p>
                </body>
                </html>");
            response.ContentLength64 = buffer.Length;
            try {
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            } catch { /* Browser might close early */
            }
        }

        public static partial void TryBringGameToFront();
    }
}