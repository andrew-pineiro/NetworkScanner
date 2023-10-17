using System.Net.NetworkInformation;
using System.Text;
using NetworkScanner.Utils;

namespace NetworkScanner.Tools
{
    public class Ping
    {
        public static void SendPing(string address)
        {
            System.Net.NetworkInformation.Ping sender = new();
            PingOptions options = new();

            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = sender.Send(address, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                Program.ValidIPs.Add(address);
                Output.Message($"Successful ping reply from {address}", Output.MessageType.Notify);
            }
        }
    }
}
