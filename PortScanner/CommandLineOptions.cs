using System;
using System.Linq;
using CommandLine;

namespace PortScanner
{
    public class CommandLineOptions
    {
        [Option(shortName: 't', longName: "tcp", Required = false, HelpText = "scan tcp",
            Default = true)]
        public bool Tcp { get; set; }

        [Option(shortName: 'u', longName: "upd", Required = false, HelpText = "scan udp", Default = false)]
        public bool Upd { get; set; }

        public int BeginPort = 0;
        public int EndPort = 65535;

        [Option(shortName: 'p', longName: "ports", Required = false, HelpText = "ports range for scan")]
        public string Ports
        {
            set
            {
                var values = value
                    .Split(" ", StringSplitOptions.TrimEntries)
                    .Select(x => int.Parse(x)).ToArray();
                if (values.Length is > 2 or < 1 || values.Any(x => x is < 0 or > 65535))
                {
                    Console.WriteLine("incorrect ports argument");
                    Environment.Exit(1);
                }

                BeginPort = values[0];
                if (values.Length > 1)
                    EndPort = values[1];
            }
        }

        [Value(0)] public string Ip { get; set; }
    }
}