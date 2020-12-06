using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using MoreLinq;

namespace Aoc2020
{
    [Command(name: "day-06")]
    public class Day06 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine(CountUniqueAnswers("input/06-sample"));
            console.Output.WriteLine(CountUniqueAnswers("input/06-input"));
        }

        int CountUniqueAnswers(string filename)
        {
            return File.ReadAllLines(filename)
                .Split(string.IsNullOrWhiteSpace)
                .Select(s => s.ToDelimitedString(string.Empty).Trim())
                .Select(answers => answers.Distinct())
                .Select(distinctAnswers => distinctAnswers.Count())
                .Pipe(x =>
                {
                    Console.WriteLine(x);
                })
                .Sum();
        }
    }
}