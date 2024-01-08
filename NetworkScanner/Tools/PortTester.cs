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
                FtpWebRequest ftp = (FtpWebRequest) WebRequest.Create($"ftp://{host}:{port}");
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                ftp.Credentials = new NetworkCredential("anonymous", "");
                ftp.GetResponse();

                return true;
            } catch
            {
                return false;
            }
            
        }
    }
}
