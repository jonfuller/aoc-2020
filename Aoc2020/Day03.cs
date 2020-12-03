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
            var sampleMap = Map.Load("input/03-sample");
            var inputMap = Map.Load("input/03-input");
            var slopes = new[]
            {
                (x: 1, y: 1),
                (x: 3, y: 1),
                (x: 5, y: 1),
                (x: 7, y: 1),
                (x: 1, y: 2)
            };

            console.Output.WriteLine(CountTrees(sampleMap, (x: 3, y: 1)));
            console.Output.WriteLine(CountTrees(inputMap, (x: 3, y: 1)));
            console.Output.WriteLine(SlopeProduct(sampleMap, slopes));
            console.Output.WriteLine(SlopeProduct(inputMap, slopes));

            static int CountTrees(Map map, (int x, int y) slope) => map
               .TraverseSlope(slope)
               .Count(c => c.IsTree);

            long SlopeProduct(Map map, IEnumerable<(int x, int y)> slopes) => slopes
               .Select(s => CountTrees(map, s))
               .Pipe(x => console.Output.WriteLine(x))
               .Aggregate(seed: (long)1, (seed, count) => seed * count);
        }
    }

    public record Cell(bool IsTree);
    public record Map(IEnumerable<IEnumerable<Cell>> Lines)
    {
        public IEnumerable<Cell> TraverseSlope((int x, int y) slope) => Lines
           .TakeEvery(slope.y)
           .Aggregate(seed: (currentX: 0, prev: Enumerable.Empty<Cell>()),
            (acc, row) => (
                currentX: acc.currentX + slope.x,
                prev: acc.prev.Concat(new []{row.ElementAt(acc.currentX) })),
            resultSelector: x => x.prev);

        public static Map Load(string file) => File
           .ReadLines(file)
           .Select(row => row.Select(c => new Cell(c == '#')))
           .Select(row => row.Repeat())
           .Then(_ => new Map(_));
    }
}