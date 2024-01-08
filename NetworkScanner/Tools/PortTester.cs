using NetworkScanner.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetworkScanner.Tools
{
    public static class PortTester
    {
        public static bool TestFTPConnection(IPAddress host, int port)
        {
            try
            {
                var ftpRequest = $"ftp://{host.ToString()}:{port}/";
                
                Output.Message($"Testing ftp port {port} on {host} [{ftpRequest}]", Utils.Output.MessageType.Debug);
                
                FtpWebRequest ftp = (FtpWebRequest) WebRequest.Create(ftpRequest);
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                ftp.Credentials = new NetworkCredential("anonymous", "test@email.com");
                ftp.UsePassive = false;
                ftp.GetResponse();
                
                return true;
            } catch
            {
                return false;
            }
            
        }
    }
}
