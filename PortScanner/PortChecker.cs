using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScanner
{
    public class PortChecker
    {
        private string Host { get; set; }

        public PortChecker(string host)
        {
            Host = host;
        }

        public async Task<bool> CheckTcpAsync(int port)
        {
            var client = new TcpClient();
            try
            {
                await client.ConnectAsync(Host, port);
            }
            catch (Exception)
            {
                return false;
            }


            return client.Connected;
        }
    }
}