using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkScanner
{
    internal class Program
    {
        public static List<string> ValidIPs { get; set; } = new List<string>();
        enum MessageType
        {
            Error,
            Information,
            Notify,
            None
        }
        public static bool Debug = false;
        static void Main(string[] args)
        {
            if (args.Contains("--help") || args.Contains("-h") || (args.Length == 0 && Debug == false))
            {
                OutputMessage($"SYNTAX\n\n.\\{AppDomain.CurrentDomain.FriendlyName}.exe ip_address [options]\n\n" +
                    $"OPTIONS\n\n" +
                    $"--help,-h       Displays this help message.\n" +
                    $"-pN             Perform a portscan without caring about ping replies.\n" +
                    $"-p[1-65535]     [DEFAULT: 1-1000] Supplies a port range for scanning. Accepts a range (#-#), single port (#), or comma seperated (#,#,#)\n" +
                    $"/[24...31]      Supplies a subnet mask for ip range scanning.\n"
                    , MessageType.None);
                return;
            }

            var address = string.Empty;
            var maxThreads = 2000;
            var mask = 0;
            string portRange = "1-1000";
            bool considerPing = true;
            //arg parsing
            if (args.Length > 0) {
                foreach(var arg in args)
                {
                    if (IPAddress.TryParse(arg, out _))
                    {
                        address = arg;
                    } else 
                    if (Regex.IsMatch(arg, "^-p\\d"))
                    {
                        portRange = arg.Substring(2);
                    } else 
                    if (arg == "-pN")
                    {
                        considerPing = false;
                    } else 
                    if (arg.StartsWith('/'))
                    {
                        mask = Convert.ToInt32(arg.Substring(1));
                    }
                }
            }
            if(string.IsNullOrEmpty(address))
            {
                OutputMessage("Invalid IP address supplied.", MessageType.Error);
                return;
            }
            List<string> validAddresses = new List<string>();
            if (mask > 0)
            {
                var ipCount = mask switch
                {
                    24 => 255,
                    25 => 128,
                    26 => 64,
                    27 => 32,
                    28 => 16,
                    29 => 8,
                    30 => 4,
                    31 => 2,
                    _ => 0
                };
                if(ipCount == 0)
                {
                   OutputMessage("Invalid subnet mask supplied. [Range 24-31]", MessageType.Error);
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
            if(portRange.Length > 0 && (ValidIPs.Count > 0 || considerPing == false))
            {
                OutputMessage($"{ValidIPs.Count()} total active IPs found. Recursively checking for open port(s): {portRange}", MessageType.Information);
                var ports = new List<int>();
                try
                {
                    if (portRange.Contains("-"))
                    {
                        var bounds = new List<int>();
                        foreach (string p in portRange.Split('-'))
                        {
                            bounds.Add(Convert.ToInt32(p));
                        }
                        for (int x = bounds[0]; x <= bounds[1]; x++)
                        {
                            ports.Add(x);
                        }
                    }
                    else if (portRange.Contains(","))
                    {
                        foreach (string p in portRange.Split(","))
                        {
                            ports.Add(Convert.ToInt32(p));
                        }
                    }
                    else
                    {
                        ports.Add(Convert.ToInt32(portRange));
                    }
                }
                catch (FormatException)
                {

                    OutputMessage($"Invalid port range supplied", MessageType.Error);
                    return;
                }
                if (ports.Count == 0 || (ports.Max() > 65535 || ports.Min() < 0))
                {
                    OutputMessage("Invalid port range supplied", MessageType.Error);
                    return;
                }
                foreach (var ip in ValidIPs)
                {
                    OutputMessage($"Checking open ports on {ip}...", MessageType.Information);
                    foreach(var port in ports)
                    {
                        Thread t = new Thread(() => PortCheck(IPAddress.Parse(ip), port));
                        t.Start();
                        while (Process.GetCurrentProcess().Threads.Count > maxThreads)
                        {
                            continue;
                        }

                    }
                    
                }
            }
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
                OutputMessage($"Successful ping reply from {address}", MessageType.Notify);
            }   
        }

        static void PortCheck(IPAddress IP, int port)
        {
            IPEndPoint remote = new IPEndPoint(IP, port);
            Socket sender = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(remote);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                OutputMessage($"Port {port} is open on {IP}", MessageType.Notify);
            } catch (SocketException)
            {

                return;

            } catch (Exception e)
            {
                OutputMessage($"Unexpected Exception: {e}", MessageType.Error);
                return;
            }

        }

        static void OutputMessage(string message, MessageType type)
        {
            string symbol;
            switch (type) {
                case MessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    symbol = "[!] ";
                    break;
                case MessageType.Information:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    symbol = "[*] ";
                    break;
                case MessageType.Notify:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    symbol = "[+] ";
                    break;
                default:
                    symbol = "";
                    break;
            };
            Console.Write(symbol);
            Console.ResetColor();
            Console.WriteLine(message);
        }
    }
}