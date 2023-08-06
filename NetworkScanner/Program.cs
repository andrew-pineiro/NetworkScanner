using System.Net.NetworkInformation;
using System.Text;

namespace NetworkScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = SendPing(args[0]);
            if(result == IPStatus.Success)
            {
                Console.WriteLine("success!");
            } else
            {
                Console.WriteLine(result);
            }
        }
        static IPStatus SendPing(string address)
        {
            Ping sender = new();
            PingOptions options = new();

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = sender.Send(address, timeout, buffer, options);
            return reply.Status;
        }
    }
}