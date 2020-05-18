using System;
using System.Threading.Tasks;

namespace indyClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CliLoop cli = new CliLoop();
            await cli.run();
        }
    }
}
