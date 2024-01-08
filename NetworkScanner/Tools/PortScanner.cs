using NetworkScanner.Utils;
using System.Net.Sockets;
using System.Net;

namespace NetworkScanner.Tools
{
    public class PortScanner
    {
        public static void Scan(IPAddress IP, int port, bool aggressive)
        {
            IPEndPoint remote = new IPEndPoint(IP, port);
            Socket sender = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(remote);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Output.Message($"Port {port} is open on {IP}", Output.MessageType.Notify);
                if(aggressive)
                {
                    switch (port)
                    {
                        case 21:
                            var openFtp = PortTester.TestFTPConnection(IP, port);
                            if(openFtp)
                            {
                                Output.Message($"Anonymous FTP access open for {IP} on {port}", Output.MessageType.Notify);
                            }
                            break;
                        //TODO: implement other aggressive port testing
                        default:
                            break;
                    }
                }
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
