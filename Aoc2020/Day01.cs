using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using MoreLinq;

namespace Aoc2020
{
    [Command(name: "day-01")]
    public class Day01 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var numbers = File
               .ReadAllLines("input/01")
               .Select(int.Parse)
               .ToList();

            var part1 = numbers
               .Cartesian(
                    numbers,
                    (i, j) => (i, j, sum: i + j, prod: i * j))
               .First(x => x.sum == 2020);

            var part2 = numbers
               .Cartesian(
                    numbers,
                    numbers,
                    (i, j, k) => (i, j, k, sum: i + j + k, prod: i * j * k))
               .First(x => x.sum == 2020);

            console.Output.WriteLine(part1.prod);
            console.Output.WriteLine(part2.prod);
        }
    }
}