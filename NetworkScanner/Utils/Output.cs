namespace NetworkScanner.Utils
{
    public class Output
    {
        public enum MessageType
        {
            Error,
            Information,
            Notify,
            Debug,
            None
        }
        public static void Message(string message, MessageType type)
        {
            string symbol;
            switch (type)
            {
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
                case MessageType.Debug:
                    if(Program.Debug)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        symbol = "[*] ";
                    } else { return; }
                    break;
                default:
                    symbol = "";
                    break;
            };
            Console.Write(symbol);
            Console.ResetColor();
            Console.WriteLine(message);
        }
        public static void DisplayHelp()
        {
            Output.Message($"SYNTAX\n\n.\\{AppDomain.CurrentDomain.FriendlyName}.exe ip_address [options]\n\n" +
                    $"OPTIONS\n\n" +
                    $"--help,-h       Displays this help message.\n" +
                    $"-pN             Perform a portscan without caring about ping replies.\n" +
                    $"-d, --debug     Enables debugging for the application session.\n" +
                    $"-p[1-65535]     [DEFAULT: 1-1000] Supplies a port range for scanning. Accepts a range (#-#), single port (#), or comma seperated (\"#,#,#\")\n" +
                    $"/[24...31]      Supplies a subnet mask for ip range scanning.\n"
                    , Output.MessageType.None);
        }
    }
}
