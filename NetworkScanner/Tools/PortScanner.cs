using NetworkScanner.Utils;
using System.Net.Sockets;
using System.Net;

namespace NetworkScanner.Tools
{
    public class PortScanner
    {
        public static void Scan(IPAddress IP, int port)
        {
            IPEndPoint remote = new IPEndPoint(IP, port);
            Socket sender = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(remote);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Output.Message($"Port {port} is open on {IP}", Output.MessageType.Notify);
            }
            catch (SocketException)
            {

                return;

            }
            catch (Exception e)
            {
                Output.Message($"Unexpected Exception: {e}", Output.MessageType.Error);
                return;
            }

        }
    }
}
