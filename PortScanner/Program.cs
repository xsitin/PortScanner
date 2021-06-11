using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using IcmpDotNet;

namespace PortScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineOptions options = null;
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(opt => options = opt);
            var tcpChecks = new List<Task<TcpClient>>();
            var udpSends = new List<Task<int>>();
            if (options.Tcp)
                for (var port = options.BeginPort; port < options.EndPort; port++)
                {
                    var port1 = port;
                    tcpChecks.Add(Task.Run(async () =>
                    {
                        var client = new TcpClient();
                        try
                        {
                            await client.ConnectAsync(options.Ip, port1);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                        Console.WriteLine("tcp"+port1);
                        return client;
                    }));
                }

            var icmpAnswers = new List<ICMPv4Packet>();
            if (options.Upd)
            {
                // Task.Run(function: async () =>
                // {
                //     EndPoint myEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //     var socket = new Socket(SocketType.Raw, ProtocolType.Icmp);
                //     socket.Bind(myEndPoint);
                //     var result = ArraySegment<byte>.Empty;
                //     while (true)
                //     {
                //         await socket.ReceiveFromAsync(result, SocketFlags.None, myEndPoint);
                //         var packet = new ICMPv4Packet();
                //         packet.parsePacket(result.ToArray());
                //         lock (icmpAnswers) icmpAnswers.Add(packet);
                //     }
                // });
                for (var port = options.BeginPort; port < options.EndPort; port++)
                {
                    var port1 = port;
                    udpSends.Add(Task.Run(() =>
                    {
                        var udpClient = new UdpClient(options.Ip, port1);
                        Byte[] messagebyte = Encoding.Default.GetBytes("hi".ToCharArray());
                        udpClient.Send(messagebyte, messagebyte.Length);
                        return port1;
                    }));
                }
            }

            Task.WaitAll(tcpChecks.ToArray());
            Task.WaitAll(udpSends.ToArray());
            if (options.Tcp)
                foreach (var tcpCheck in tcpChecks)
                    if (tcpCheck.Result.Connected)
                        Console.Write($"TCP {(tcpCheck.Result.Client.RemoteEndPoint as IPEndPoint).Port}");
            if (options.Upd)
            {
                var unreachablePorts = new HashSet<int>();
                unreachablePorts.UnionWith(icmpAnswers.Where(x => x.code == 3)
                    .Select(x => BitConverter.ToInt32(x.data[23..25])));
                foreach (var port in udpSends)
                    if (!unreachablePorts.Contains(port.Result))
                        Console.WriteLine($"UDP {port.Result}");
            }
        }
    }
}