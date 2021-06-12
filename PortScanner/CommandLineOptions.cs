using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CommandLine;

namespace PortScanner
{
    public class CommandLineOptions
    {
        public int BeginPort = 0;
        public int EndPort = 65535;

        public string Ports
        {
            set
            {
                var values = value
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x.Trim())).ToArray();
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

        public string Ip { get; set; }

        public static CommandLineOptions ParseArgs(string[] args)
        {
            if (args.Any(x => x is "-h" or "--help")) PrintHelp();
            var options = new CommandLineOptions();
            var argsQueue = new Queue<string>(args.Where(x => x != "-t"));
            {
                if (argsQueue.Count < 1) PrintHelp();
                var address = argsQueue.Dequeue().ToLowerInvariant();
                //Проверка является ли адресс корректным url или ipv4
                if (!(Regex.IsMatch(address,
                          @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$")
                      ||
                      Regex.IsMatch(address,
                          @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")))
                {
                    Console.WriteLine("Incorrect host address!");
                }

                options.Ip = address;
            }
            {
                if (argsQueue.Dequeue() != "-p")
                {
                    Console.WriteLine("Ports not selected!");
                    Environment.Exit(1);
                }

                var begin = -1;
                var end = -1;
                if (!int.TryParse(argsQueue.Dequeue(), out begin) ||
                    (argsQueue.Count > 0 && !int.TryParse(argsQueue.Dequeue(), out end)))
                {
                    Console.WriteLine("Port should be a number");
                    Environment.Exit(1);
                }

                if (end != -1 && begin > end)
                {
                    Console.WriteLine("End port should be same or more than begin port!");
                    Environment.Exit(1);
                }

                if (end != -1) options.EndPort = end;
                options.BeginPort = begin;
            }

            return options;
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"Its script for check tcp port.
            Usage: PortScanner.exe *address* -p *begin port* [end port]
                default end port is 65535");
            Environment.Exit(1);
        }
    }
}