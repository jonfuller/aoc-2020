using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using static MoreLinq.Extensions.WindowExtension;
using static MoreLinq.Extensions.SubsetsExtension;

namespace Aoc2020
{
    public record JoltageAdapter(int Jolts);
    public record JoltageEdge(JoltageAdapter Source, JoltageAdapter Target) : IEdge<JoltageAdapter>{}

    [Command(name: "day-10")]
    public class Day10 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var sampleData = LoadFile("input/10-sample").ToList();
            var inputData = LoadFile("input/10-input").ToList();

            console.Output.WriteLine(FindAnswerPart1(sampleData));
            console.Output.WriteLine(FindAnswerPart1(inputData));
            console.Output.WriteLine(FindAnswerPart2(sampleData));
            console.Output.WriteLine(FindAnswerPart2(inputData));

            static long FindAnswerPart1(IEnumerable<long> adapters) => adapters
               .OrderBy(x => x)
               .ToList()
               .Then(x => Enumerable.Empty<long>()
                   .Append(0)
                   .Concat(x)
                   .Append(x[Index.FromEnd(1)] + 3))
               .Window(2)
               .Select(x => x.Last() - x.First())
               .GroupBy(x => x)
               .ToDictionary(x => x.Key, x => x.Count())
               .Then(x => x[1] * x[3]);

            static IEnumerable<long> LoadFile(string filename) => File
               .ReadAllLines(filename)
               .Select(long.Parse);
        }

        private static long FindAnswerPart2(IList<long> input)
        {
            input = input.Prepend(0).OrderBy(it => it).ToArray();
            var waysIn = new long[input.Count];

            waysIn[0] = 1;

            for (var i = 0; i < input.Count; ++i)
            {
                var current = input[i];
                var waysIntoCurrent = waysIn[i];
                for (var j = 1; j <= 3; ++j)
                {
                    var next = i + j;
                    if (next >= input.Count || input[next] > current + 3) break;
                    waysIn[next] = waysIn[next] + waysIntoCurrent;
                }
            }

            return waysIn.Last();
        }
    }
}