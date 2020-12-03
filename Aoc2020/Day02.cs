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

        public record PasswordEntry
        {
            public int Num1 { get; init; }
            public int Num2 { get; init; }
            public char Letter { get; init; }
            public string Password { get; init; }

            private static readonly Regex parser = new($"^(?<{nameof(Num1)}>\\d*)-(?<{nameof(Num2)}>\\d*) (?<{nameof(Letter)}>[a-z]): (?<{nameof(Password)}>[a-z]*)$");

            public static PasswordEntry Parse(string line) => parser.Deserialize<PasswordEntry>(line);

            public bool IsValidPart1 => WithinRange(Num1, Num2, Password.Count(c => c == Letter));
            public bool IsValidPart2 => Part2Letters.Count(c => c == Letter) == 1;

            string Part2Letters => new(new [] {Password[Num1 - 1], Password[Num2 - 1]});

            static bool WithinRange(int min, int max, int num) => min <= num && num <= max;
        }
    }

}