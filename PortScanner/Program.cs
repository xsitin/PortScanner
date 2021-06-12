using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;

namespace PortScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CommandLineOptions.ParseArgs(args);

            var checker = new PortChecker(options.Ip);
            var tasks = new List<Task<bool>>();

            try
            {
                for (var i = options.BeginPort; i < options.EndPort; i++)
                    tasks.Add(checker.CheckTcpAsync(i));
                Task.WaitAll(tasks.ToArray());
                for (var i = 0; i < tasks.Count; i++)
                    if (tasks[i].Result)
                        Console.WriteLine($"TCP {i + options.BeginPort}");
            }
            catch (Exception)
            {
                Console.WriteLine("Error while checking...");
            }
        }
    }
}