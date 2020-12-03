using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using MoreLinq.Extensions;

namespace Aoc2020
{
    [Command(name: "day-03")]
    public class Day03 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine(CountTrees("input/03-sample", (x: 3, y: 1)));
            console.Output.WriteLine(CountTrees("input/03-input", (x: 3, y: 1)));

            static int CountTrees(string inputFile, (int x, int y) slope) => Map
               .Load(inputFile)
               .TraverseSlope(slope)
               .Count(c => c.IsTree);
        }
    }

    public record Cell(bool IsTree);
    public record Map(IEnumerable<IEnumerable<Cell>> Lines)
    {
        public IEnumerable<Cell> TraverseSlope((int x, int y) slope)
        {
            return Lines
               .TakeEvery(slope.y)
               .Aggregate(seed: (currentX: 0, prev: Enumerable.Empty<Cell>()),
                (acc, row) => (
                    currentX: acc.currentX + slope.x,
                            prev: acc.prev.Concat(new []{row.ElementAt(acc.currentX) })),
                resultSelector: x => x.prev);
        }
        public static Map Load(string file)
        {
            var lines = File
               .ReadLines(file)
               .Select(row => row.Select(c => new Cell(c == '#')))
               .Select(row => row.Repeat());

            return new Map(lines);
        }
    }
}