using System;
using System.Collections.Generic;
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
            var groupsSample = LoadGroups("input/06-sample");
            var groupsInput = LoadGroups("input/06-input");

            console.Output.WriteLine(CountUniqueAnswers(groupsSample));
            console.Output.WriteLine(CountUniqueAnswers(groupsInput));
            console.Output.WriteLine(CountUnanimous(groupsSample));
            console.Output.WriteLine(CountUnanimous(groupsInput));
        }

        int CountUniqueAnswers(IEnumerable<Group> groups) => groups
            .Select(g => g.NumDistinctYes)
            .Sum();

        int CountUnanimous(IEnumerable<Group> groups) => groups
            .Select(g => g.NumUnanimousYes)
            .Sum();

        IEnumerable<Group> LoadGroups(string filename) => File.ReadAllLines(filename)
            .Split(string.IsNullOrWhiteSpace)
            .Select(x => new Group(x));
    }

    public record Group(IEnumerable<string> Answers)
    {
        public int NumDistinctYes => DistinctYes.Count();
        public int NumUnanimousYes => UnanimousYes.Count();

        IEnumerable<char> DistinctYes => Answers
            .ToDelimitedString(string.Empty)
            .Trim()
            .Distinct();

        public IEnumerable<char> UnanimousYes => Enumerable
            .Range(0, 26)
            .Select(i => (char)('a'+i))
            .Where(c => Answers.All(a => a.Contains(c)));
    }
}