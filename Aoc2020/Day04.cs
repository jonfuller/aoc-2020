using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using MoreLinq;

namespace Aoc2020
{
    [Command(name: "day-04")]
    public class Day04 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var inputPassports = LoadPassports("input/04-input").ToList();

            console.Output.WriteLine(LoadPassports("input/04-sample").Count(p => p.ValidPart1));
            console.Output.WriteLine(inputPassports.Count(p => p.ValidPart1));

            LoadPassports("input/04-sample-invalid").Assert(p => !p.ValidPart2).Consume();
            LoadPassports("input/04-sample-valid").Assert(p => p.ValidPart2).Consume();

            console.Output.WriteLine(inputPassports.Count(p => p.ValidPart2));
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
        public bool ValidPart1 => RequiredKeys.All(Pairs.ContainsKey);
        public bool ValidPart2 => ValidPart1 && Validators
           .Select(v => (key: v.Key, value: Pairs[v.Key], valid: v.Checker(Pairs[v.Key])))
           .All(v => v.valid);

        record Validator(string Key, Func<string, bool> Checker);

        private static readonly IEnumerable<string> RequiredKeys = new[]
        {
            "byr",
            "iyr",
            "eyr",
            "hgt",
            "hcl",
            "ecl",
            "pid",
        };

        private static readonly IEnumerable<Validator> Validators = new[]
        {
            new Validator(Key: "byr", Checker: val => val.Length == 4 && int.TryParse(val, out var x) && x >= 1920 && x <= 2002),
            new Validator(Key: "iyr", Checker: val => val.Length == 4 && int.TryParse(val, out var x) && x >= 2010 && x <= 2020),
            new Validator(Key: "eyr", Checker: val => val.Length == 4 && int.TryParse(val, out var x) && x >= 2020 && x <= 2030),
            new Validator(Key: "hgt", Checker: ValidHeight),
            new Validator(Key: "hcl", Checker: val => Regex.Match(val, @"^#[0-9a-f]{6}$").Success),
            new Validator(Key: "ecl", Checker: val => ValidEyeColors.Count(v => v == val) == 1),
            new Validator(Key: "pid", Checker: val => Regex.Match(val, @"^[\d]{9}$").Success),
        };
        private static readonly IEnumerable<string> ValidEyeColors = new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };

        private static bool ValidHeight(string heightString)
        {
            var heightRegex = new Regex(@"^(?<value>[\d]*)(?<unit>[a-z]{2})$", RegexOptions.Compiled);
            var match = heightRegex.Match(heightString);

            if (!match.Success)
                return false;

            if (!int.TryParse(match.Groups["value"].Value, out var value)) return false;

            return match.Groups["unit"].Value switch
            {
                "cm" => value >= 150 && value <= 193,
                "in" => value >= 59 && value <= 76,
                _ => false
            };
        }
    }
}