using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PortScanner
{
    public class CommandLineOptions
    {
        public int BeginPort = 0;
        public int EndPort = 65535;

        public string Ip { get; set; }

        public static CommandLineOptions ParseArgs(string[] args)
        {
            if (args.Any(x => x is "-h" or "--help")) PrintHelp(0);
            var options = new CommandLineOptions();
            var argsQueue = new Queue<string>(args);
            {
                if (argsQueue.Count < 1) PrintHelp(1);
                var address = ParseAddress(argsQueue);
                options.Ip = address;
            }
            ParsePorts(argsQueue, out var begin, out var end);

            if (end != -1) options.EndPort = end;
            options.BeginPort = begin;

            return options;
        }

        private static void ParsePorts(Queue<string> argsQueue, out int begin, out int end)
        {
            if (argsQueue.Dequeue() != "-p")
            {
                Console.WriteLine("Ports not selected!");
                Environment.Exit(1);
            }

            begin = -1;
            end = -1;
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
        }

        private static string ParseAddress(Queue<string> argsQueue)
        {
            var address = argsQueue.Dequeue().ToLowerInvariant();
            //Проверка является ли адресс корректным url или ipv4
            if (!(Regex.IsMatch(address, @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$")
                  ||
                  Regex.IsMatch(address,
                      @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")))
            {
                Console.WriteLine("Incorrect host address!");
            }

            return address;
        }


        private static void PrintHelp(int exitCode)
        {
            Console.WriteLine(@"Script for check tcp port.
            Usage: PortScanner.exe *address* -p *begin port* [end port]
                default end port is 65535");
            Environment.Exit(exitCode);
        }
    }
}