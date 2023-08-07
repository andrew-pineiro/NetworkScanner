using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkScanner
{
    internal class Program
    {
        public static List<string> ValidIPs { get; set; } = new List<string>();
        static void Main(string[] args)
        {

            var address = args[0];
            var maxThreads = 2000;
            List<string> validAddresses = new List<string>();
            if (args.Length > 1)
            {
                var mask = args[1];
                var ipCount = mask switch
                {
                    "24" => 255,
                    "25" => 128,
                    "26" => 64,
                    "27" => 32,
                    "28" => 16,
                    "29" => 8,
                    "30" => 4,
                    "31" => 2,
                    _ => 0
                };
                if(ipCount == 0 )
                {
                    Console.WriteLine("[!] Invalid subnet mask supplied. [Range 24-31]");
                    return;
                }
                var startNum = int.Parse(address.Substring(address.LastIndexOf('.') + 1));
                for (int i=startNum; i < ipCount; i++)
                {
                    
                    var workingAddress = address.Substring(0, address.LastIndexOf('.'));
                    workingAddress += string.Format(".{0}", Convert.ToString(i));
                    Thread t = new Thread(() => SendPing(workingAddress));
                    t.Start();
                    while(Process.GetCurrentProcess().Threads.Count > maxThreads)
                    {
                        continue;
                    }
                }
            }
            else
            {
                SendPing(address);
            }
            Console.WriteLine("[+] {0} total active IPs found.", ValidIPs.Count());

        }
        static void SendPing(string address)
        {
            Ping sender = new();
            PingOptions options = new();

            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = sender.Send(address, timeout, buffer, options);
            if(reply.Status == IPStatus.Success)
            {
                ValidIPs.Add(address);
                Console.WriteLine("[+] Successful ping reply from {0}", address);
            }
            
        }
    }
}