using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using MoreLinq;

namespace Aoc2020
{
    [Command(name: "day-05")]
    public class Day05 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var samples = new[]
            {
                "FBFBBFFRLR",
                "BFFFBBFRRR",
                "FFFBBBFRRR",
                "BBFFBBFRLL"
            };

            samples
               .Select(s => new BoardingPass(s))
               .ForEach(x => console.Output.WriteLine(x));

            var passes = File.ReadLines("input/05-input")
               .Select(s => new BoardingPass(s))
               .OrderBy(p => p.SeatId)
               .ToList();

            console.Output.WriteLine(passes.MaxBy(b => b.SeatId).Single());

            passes
               .Lead(1, passes.First(),
                    (a, b) => (diff: b.SeatId-a.SeatId, id: b.SeatId-1))
               .First(x => x.diff != 1)
               .Then(x => console.Output.WriteLine(x));
        }
    }

    public record BoardingPass(string Encoded)
    {
        private const int NumRows = 128;
        private const int NumCols = 8;

        private int Row => Encoded.Take(7)
           .Aggregate(Enumerable.Range(0, NumRows),
                (acc, letter) => letter == 'F' ? acc.FrontHalf() : acc.BackHalf(),
                acc => acc.Single());

        private int Column => Encoded.Skip(7)
           .Aggregate(Enumerable.Range(0, NumCols),
                (acc, letter) => letter == 'L' ? acc.FrontHalf() : acc.BackHalf(),
                acc => acc.Single());

        public int SeatId => (Row * 8) + Column;
    }
}