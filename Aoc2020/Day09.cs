using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using MoreLinq;

namespace Aoc2020
{
    [Command(name: "day-09")]
    public class Day09 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var sampleData = LoadFile("input/09-sample");
            var inputData = LoadFile("input/09-input");

            var sampleAnswerPart1 = FirstInvalidEncoded(sampleData, 5);
            var inputAnswerPart1 = FirstInvalidEncoded(inputData, 25);

            var sampleAnswerPart2 = FindWeakness(sampleData, sampleAnswerPart1);
            var inputAnswerPart2 = FindWeakness(inputData, inputAnswerPart1);

            console.Output.WriteLine(sampleAnswerPart1);
            console.Output.WriteLine(inputAnswerPart1);
            console.Output.WriteLine(sampleAnswerPart2);
            console.Output.WriteLine(inputAnswerPart2);

            static IList<long> LoadFile(string filename) => File
               .ReadAllLines(filename)
               .Select(long.Parse)
               .ToList();

            static long FindWeakness(IList<long> input, long part1Answer) => Enumerable
               .Range(2, 1000)
               .SelectMany(x => input.Window(x).Select(window => (min: window.Min(), max: window.Max(), sum: window.Sum())))
               .First(x => x.sum == part1Answer)
               .Then(x => x.max + x.min);

            static long FirstInvalidEncoded(IList<long> input, int preambleSize) => input
               .Window(preambleSize)
               .Zip(input.Skip(preambleSize))
               .First(x => !HasValidPre(x.First, x.Second))
               .Then(x => x.Second);

            static bool HasValidPre(IList<long> preamble, long number) => preamble
               .Cartesian(preamble, (a, b) => (a, b, sum: a + b))
               .Where(x => x.a != x.b)
               .Any(x => x.sum == number);
        }
    }
}