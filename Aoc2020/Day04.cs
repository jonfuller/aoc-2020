using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Aoc2020
{
    [Command(name: "day-04")]
    public class Day04 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine(LoadPassports("input/04-sample").Count(p => p.ValidPart1));
            console.Output.WriteLine(LoadPassports("input/04-input").Count(p => p.ValidPart1));
        }

        private static IEnumerable<Passport> LoadPassports(string filename) => File
           .ReadLines(filename)
           .Aggregate(
                new List<string>() {""},
                (acc, line) =>
                {
                    if (string.IsNullOrWhiteSpace(line))
                        acc.Add("");
                    acc[Index.FromEnd(1)] += " " + line;

                    return acc;
                })
           .Select(ParsePassport);

        private static Passport ParsePassport(string line)
        {
            return new Passport(line
               .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
               .Select(pair => pair.Split(':'))
               .ToDictionary(pair => pair[0], pair => pair[1]));
        }
    }

    public record Passport(Dictionary<string, string> Pairs)
    {
        private readonly IEnumerable<string> _requiredKeys = new[]
        {
            "byr",
            "iyr",
            "eyr",
            "hgt",
            "hcl",
            "ecl",
            "pid",
        };

        public bool ValidPart1 => _requiredKeys.All(Pairs.ContainsKey);
    }
}