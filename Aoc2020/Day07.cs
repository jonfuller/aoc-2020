using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using QuikGraph;

namespace Aoc2020
{
    [Command(name: "day-07")]
    public class Day07 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var sampleGraph = LoadGraph("input/07-sample");
            var sampleGraph2 = LoadGraph("input/07-sample2");
            var inputGraph = LoadGraph("input/07-input");

            var shinyGold = new BagColor("shiny gold");

            console.Output.WriteLine(AllColorsFor(sampleGraph, shinyGold).ToList().Count);
            console.Output.WriteLine(AllColorsFor(inputGraph, shinyGold).ToList().Count);

            console.Output.WriteLine(CountInsideBags(sampleGraph, shinyGold));
            console.Output.WriteLine(CountInsideBags(sampleGraph2, shinyGold));
            console.Output.WriteLine(CountInsideBags(inputGraph, shinyGold));
        }

        static int CountInsideBags(AdjacencyGraph<BagColor, BagEdge> graph, BagColor toCount) => graph
           .OutEdges(toCount)
           .Select(e => e.Quantity + e.Quantity * CountInsideBags(graph, e.Target))
           .Sum();

        static IEnumerable<BagColor> AllColorsFor(AdjacencyGraph<BagColor, BagEdge> graph, BagColor color)
        {
            return _(graph, color).Distinct();

            static IEnumerable<BagColor> _(AdjacencyGraph<BagColor, BagEdge> graph, BagColor color)
            {
                var parentColors = ColorsFor(graph, color).ToList();

                return parentColors.Concat(parentColors.SelectMany(pc => _(graph, pc)));
            }

            static IEnumerable<BagColor> ColorsFor(AdjacencyGraph<BagColor, BagEdge> graph, BagColor color) => graph
               .Edges
               .Where(e => e.Target == color)
               .Select(e => e.Source);
        }

        static AdjacencyGraph<BagColor, BagEdge> LoadGraph(string filename)
        {
            var sampleGraph = new AdjacencyGraph<BagColor, BagEdge>();

            var bagDefinitions = File
               .ReadLines(filename)
               .Select(ParseBagDefinition)
               .ToList();

            var colors = bagDefinitions
               .SelectMany(def => new[] { def.Outer }.Concat(def.Inner.Select(inner => inner.Item2)))
               .Distinct();

            sampleGraph.AddVertexRange(colors);
            sampleGraph.AddEdgeRange(bagDefinitions.SelectMany(ToEdges));

            return sampleGraph;

            static IEnumerable<BagEdge> ToEdges(BagDefinition bag) =>
                bag.Inner.Select(x => new BagEdge(bag.Outer, x.Item2, x.Item1));
        }

        private static BagDefinition ParseBagDefinition(string line)
        {
            var noInner = new Regex(@"^(?<OuterColor>[a-z ]+) bags contain no other bags.$");
            var hasInner = new Regex(@"^(?<OuterColor>[a-z ]+) bags contain ((?<InnerQuantity>[0-9]{1,2}) (?<InnerColor>[a-z ]+) bag(s)?(, )?)+\.$");

            var noInnerMatch = noInner.Match(line);
            if (noInnerMatch.Success)
            {
                return new BagDefinition(
                    new BagColor(noInnerMatch.Groups["OuterColor"].Value),
                    Enumerable.Empty<(int, BagColor)>());
            }

            var hasInnerMatch = hasInner.Match(line);
            var innerBags = hasInnerMatch.Groups["InnerQuantity"].Captures.Zip(
                hasInnerMatch.Groups["InnerColor"].Captures,
                (quantity, color) => (int.Parse(quantity.Value), new BagColor(color.Value))).ToList();
            return new BagDefinition(
                new BagColor(hasInnerMatch.Groups["OuterColor"].Value),
                innerBags);
        }
    }

    public record BagDefinition(BagColor Outer, IEnumerable<(int, BagColor)> Inner);
    public record BagEdge(BagColor Source, BagColor Target, int Quantity) : IEdge<BagColor> { }
    public record BagColor(string Color);
}