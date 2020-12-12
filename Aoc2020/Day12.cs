using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Aoc2020
{
    [Command(name: "day-12")]
    public class Day12 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var initialFerry = new Ferry(Direction.East, new Point(0, 0));
            var sampleRoute = File
               .ReadAllLines("input/12-sample")
               .Select(ParseInstruction);
            var inputRoute = File
               .ReadAllLines("input/12-input")
               .Select(ParseInstruction);

            NavigateFerry(initialFerry, sampleRoute)
               .Then(ManhattanDistance)
               .Then(d => console.Output.WriteLine(d));
            NavigateFerry(initialFerry, inputRoute)
               .Then(ManhattanDistance)
               .Then(d => console.Output.WriteLine(d));

            Point NavigateFerry(Ferry ferry, IEnumerable<FerryInstruction> route) => route
               .Aggregate(
                    seed: new Ferry(Direction.East, new Point(0, 0)),
                    func: (acc, obj) => acc.Move(obj),
                    resultSelector: ferry => ferry.Position);

            int ManhattanDistance(Point fromOrigin) => Math.Abs(fromOrigin.X) + Math.Abs(fromOrigin.Y);
        }

        private static readonly Regex InstructionPattern = new(@"^(?<Action>[NESWFLR]{1})(?<Amount>[0-9]{1,})$");
        private static FerryInstruction ParseInstruction(string line) =>
            InstructionPattern.Deserialize<FerryInstruction>(line);
    }

    public enum Direction { North, East, South, West}
    public record Ferry(Direction Facing, Point Position)
    {
        public Ferry Move(FerryInstruction toMove) => toMove.Action switch
        {
            "F" => this with { Position = MoveForward(toMove.Amount)},
            "L" => this with { Facing = TurnLeft(toMove.Amount) },
            "R" => this with { Facing = TurnRight(toMove.Amount) },
            "N" => this with { Position = MoveDirection(Direction.North, toMove.Amount) },
            "E" => this with { Position = MoveDirection(Direction.East, toMove.Amount) },
            "S" => this with { Position = MoveDirection(Direction.South, toMove.Amount) },
            "W" => this with { Position = MoveDirection(Direction.West, toMove.Amount) },
        };

        Direction TurnLeft(int degrees) => (Direction)(((int)Facing - degrees / 90 + 4)%4);
        Direction TurnRight(int degrees) => (Direction)(((int)Facing + degrees / 90)%4);
        Point MoveForward(int amount) => MoveDirection(Facing, amount);
        Point MoveDirection(Direction direction, int amount) => direction switch
        {
            Direction.North => new Point(Position.X, Position.Y + amount),
            Direction.East => new Point(Position.X + amount, Position.Y),
            Direction.South => new Point(Position.X, Position.Y - amount),
            Direction.West => new Point(Position.X - amount, Position.Y),
            _ => throw new InvalidOperationException()
        };
    }

    public record FerryInstruction
    {
        public string Action { get; init; }
        public int Amount { get; init; }
    }
}