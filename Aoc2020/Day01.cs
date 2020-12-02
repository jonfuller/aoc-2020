using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using MoreLinq;
using static System.Console;

namespace Aoc2020
{
    [Command(name: "day-02")]
    public class Day02 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var validSamplePasswords = File
               .ReadLines("input/02-sample")
               .Select(PasswordEntry.Parse)
               .Count(pe => pe.IsValid);

            var part1 = File
               .ReadLines("input/02-input")
               .Select(PasswordEntry.Parse)
               .Count(pe => pe.IsValid);

            WriteLine($"Valid sample passwords: {validSamplePasswords}");
            WriteLine($"Part 1: {part1}");
        }

        public record PasswordEntry(int Min, int Max, char Letter, string Password)
        {
            public static PasswordEntry Parse(string line)
            {
                var pattern = new Regex("^(?<min>\\d*)-(?<max>\\d*) (?<letter>[a-z]): (?<password>[a-z]*)$", RegexOptions.Compiled);
                var match = pattern.Match(line);

                return new PasswordEntry(
                    int.Parse(match.Groups["min"].Value),
                    int.Parse(match.Groups["max"].Value),
                    match.Groups["letter"].Value.First(),
                    match.Groups["password"].Value);

            }
            public bool IsValid
            {
                get
                {
                    var actual = Password.Count(c => c == Letter);
                    return Min <= actual && actual <= Max;
                }
            }
        }
    }

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