using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Aoc2020
{
    [Command(name: "day-02")]
    public class Day02 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var sample = Analyze("input/02-sample");
            console.Output.WriteLine($"Part 1 sample: {sample.Count(s => s.IsValidPart1)}");
            console.Output.WriteLine($"Part 2 sample: {sample.Count(s => s.IsValidPart2)}");

            var actual = Analyze("input/02-input");
            console.Output.WriteLine($"Part 1: {actual.Count(a => a.IsValidPart1)}");
            console.Output.WriteLine($"Part 2: {actual.Count(a => a.IsValidPart2)}");

            IEnumerable<PasswordEntry> Analyze(string filename) => File
               .ReadLines(filename)
               .Select(PasswordEntry.Parse)
               .ToList();
        }

        public record PasswordEntry(int Num1, int Num2, char Letter, string Password)
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
            public bool IsValidPart1
            {
                get
                {
                    var actual = Password.Count(c => c == Letter);
                    return Num1 <= actual && actual <= Num2;
                }
            }

            public bool IsValidPart2 => Part2Letters.Count(c => c == Letter) == 1;
            public string Part2Letters => new(new [] {Password[Num1 - 1], Password[Num2 - 1]});
        }
    }
}