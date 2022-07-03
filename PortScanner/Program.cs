using System;
using System.Threading.Tasks;

namespace PortScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CommandLineOptions.ParseArgs(args);

            var checker = new PortChecker(options.Ip);
            var tasks = new Task<bool>[options.EndPort - options.BeginPort + 1];

            try
            {
                for (var i = options.BeginPort; i <= options.EndPort; i++)
                    tasks[i - options.BeginPort] = checker.CheckTcpAsync(i);
                Console.WriteLine("Please wait, check started...");
                Task.WaitAll(tasks);
                for (var i = 0; i < tasks.Length; i++)
                    if (tasks[i].Result)
                        Console.WriteLine($"TCP {i + options.BeginPort}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while checking..." + "\n" + $"{ex.Message}");
            }
        }
    }
}