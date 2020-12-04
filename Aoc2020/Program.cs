using System.Threading.Tasks;
using CliFx;

namespace Aoc2020
{
    class Program
    {
        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
               .AddCommandsFromThisAssembly()
               .Build()
               .RunAsync();
    }
}
