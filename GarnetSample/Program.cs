using Microsoft.Extensions.Logging;
using Garnet;

namespace GarnetSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var factory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole(options =>
                    {
                        options.LogToStandardErrorThreshold = LogLevel.Trace;
                    });
                });
                using var server = new GarnetServer(args, factory);
                server.Start();
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to initialize server due to exception: {ex.Message}");
            }
        }
    }
}
