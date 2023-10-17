using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using NetworkScanner.Tools;
using NetworkScanner.Utils;

namespace NetworkScanner
{
    internal class Program
    {
        public static List<string> ValidIPs { get; set; } = new List<string>();
        public static bool Debug = false;

        private static string IP { get; set; } = string.Empty;
        private static string PortRange { get; set; } = "1-1000";

        static void Main(string[] args)
        {
            if (args.Contains("--help") || args.Contains("-h") || (args.Length == 0 && Debug == false))
            {
                Output.DisplayHelp();
                return;
            }

            //default settings
            const int maxThreads = 2000;
            int mask = 0;
            bool considerPing = true;
            bool singleIP = false;

            //arg parsing
            if (args.Length > 0) {
                foreach(var arg in args)
                {
                    if (IPAddress.TryParse(arg, out _))
                    {
                        IP = arg;
                    } else 
                    if (Regex.IsMatch(arg, "^-p\\d.+"))
                    {
                        PortRange = arg[2..];
                    } else 
                    if (arg == "-d" || arg == "--debug")
                    {
                        Debug = true;
                        Output.Message("Debugging has been enabled.", Output.MessageType.Debug);
                    } else
                    if (arg == "-pN")
                    {
                        considerPing = false;
                    } else 
                    if (arg.StartsWith('/'))
                    {
                        mask = Convert.ToInt32(arg.Substring(1));
                    } else
                    if (arg.Contains('/'))
                    {
                        if (IPAddress.TryParse(arg.Split('/')[0], out _)) {
                            IP = arg.Split('/')[0];
                            mask = Convert.ToInt32(arg.Split('/')[1]);
                        }
                    }
                }
            }
            if(string.IsNullOrEmpty(IP))
            {
                Output.Message("Invalid IP address supplied.", Output.MessageType.Error);
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
                   Output.Message("Invalid subnet mask supplied. [Range 24-31]", Output.MessageType.Error);
                   return;
                }
                var startNum = int.Parse(IP.Substring(IP.LastIndexOf('.') + 1));
                if(startNum == 0) { startNum = 1; }
                for (int i=startNum; i < ipCount; i++)
                {
                    var workingAddress = IP.Substring(0, IP.LastIndexOf('.'));
                    workingAddress += string.Format(".{0}", Convert.ToString(i));
                    if (considerPing)
                    {
                        Thread t = new Thread(() => Ping.SendPing(workingAddress));
                        t.Start();
                        while (Process.GetCurrentProcess().Threads.Count > maxThreads)
                        {
                            continue;
                        }
                    } else { ValidIPs.Add(workingAddress); }

                }
            }
            else
            {
                singleIP = true;
                if(considerPing)
                {
                    Ping.SendPing(IP);
                } else { ValidIPs.Add(IP); }
            }
            if(PortRange.Length > 0 && ValidIPs.Count > 0)
            {
                if(!singleIP)
                {
                    Output.Message($"{ValidIPs.Count()} total IPs found. Recursively checking for open port(s): {PortRange}", Output.MessageType.Debug);
                }
                var ports = new List<int>();
                try
                {
                    if (PortRange.Contains('-'))
                    {
                        var bounds = new List<int>();
                        foreach (string p in PortRange.Split('-'))
                        {
                            bounds.Add(Convert.ToInt32(p));
                        }
                        for (int x = bounds[0]; x <= bounds[1]; x++)
                        {
                            ports.Add(x);
                        }
                    }
                    else if (PortRange.Contains(','))
                    {
                        foreach (string p in PortRange.Split(","))
                        {
                            ports.Add(Convert.ToInt32(p));
                        }
                    }
                    else
                    {
                        ports.Add(Convert.ToInt32(PortRange));
                    }
                }
                catch (FormatException)
                {

                    Output.Message($"Invalid port range supplied", Output.MessageType.Error);
                    return;
                }
                catch (Exception e) 
                {
                    Output.Message(e.Message, Output.MessageType.Error);
                    return;
                }

                if (ports.Count == 0 || (ports.Max() > 65535 || ports.Min() < 0))
                {
                    Output.Message("Invalid port range supplied", Output.MessageType.Error);
                    return;
                }

                foreach (var ip in ValidIPs)
                {
                    Output.Message($"Checking open ports on {ip}...", Output.MessageType.Debug);
                    foreach(var port in ports)
                    {
                        Thread t = new Thread(() => PortScanner.Scan(IPAddress.Parse(ip), port));
                        t.Start();
                        while (Process.GetCurrentProcess().Threads.Count > maxThreads)
                        {
                            continue;
                        }

                    }
                    
                }
            }
        }

        

        
    }
}