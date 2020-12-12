using System;
using System.Collections.Generic;
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
            var initialFerry = new Ferry(Direction.East, Point.Origin);
            var initialFerry2 = new Ferry2(new Point(10, 1), Point.Origin);
            var sampleRoute = File
               .ReadAllLines("input/12-sample")
               .Select(ParseInstruction)
               .ToList();
            var inputRoute = File
               .ReadAllLines("input/12-input")
               .Select(ParseInstruction)
               .ToList();

            console.Output.WriteLine(NavigateFerry(initialFerry, sampleRoute).ManhattanDistanceFromOrigin);
            console.Output.WriteLine(NavigateFerry(initialFerry, inputRoute).ManhattanDistanceFromOrigin);
            console.Output.WriteLine(NavigateFerry(initialFerry2, sampleRoute).ManhattanDistanceFromOrigin);
            console.Output.WriteLine(NavigateFerry(initialFerry2, inputRoute).ManhattanDistanceFromOrigin);

            Point NavigateFerry(IFerry ferry, IEnumerable<FerryInstruction> route) => route
               .Aggregate(
                    seed: ferry,
                    func: (acc, obj) => acc.Move(obj),
                    resultSelector: final => final.Position);
        }

        private static readonly Regex InstructionPattern = new(@"^(?<Action>[NESWFLR]{1})(?<Amount>[0-9]{1,})$");
        private static FerryInstruction ParseInstruction(string line) =>
            InstructionPattern.Deserialize<FerryInstruction>(line);
    }

    public interface IFerry
    {
        IFerry Move(FerryInstruction toMove);
        Point Position { get; }
    }
    public record Ferry2(Point WayPointPosition, Point Position) : IFerry
    {
        public IFerry Move(FerryInstruction toMove) => toMove.Action switch
        {
            "F" => this with { Position = Position.OffsetTimes(toMove.Amount, WayPointPosition) },
            "L" => this with { WayPointPosition = WayPointPosition.RotateLeft(toMove.Amount) },
            "R" => this with { WayPointPosition = WayPointPosition.RotateRight(toMove.Amount) },
            "N" => this with { WayPointPosition = WayPointPosition.OffsetY(toMove.Amount) },
            "E" => this with { WayPointPosition = WayPointPosition.OffsetX(toMove.Amount) },
            "S" => this with { WayPointPosition = WayPointPosition.OffsetY(-toMove.Amount) },
            "W" => this with { WayPointPosition = WayPointPosition.OffsetX(-toMove.Amount) },
            _ => throw new InvalidOperationException($"bad instruction action: {toMove}")
        };
    }
    public record Ferry(Direction Facing, Point Position) : IFerry
    {
        public IFerry Move(FerryInstruction toMove) => toMove.Action switch
        {
            "F" => this with { Position = MoveForward(toMove.Amount) },
            "L" => this with { Facing = Facing.TurnLeft(toMove.Amount) },
            "R" => this with { Facing = Facing.TurnRight(toMove.Amount) },
            "N" => this with { Position = MoveDirection(Direction.North, toMove.Amount) },
            "E" => this with { Position = MoveDirection(Direction.East, toMove.Amount) },
            "S" => this with { Position = MoveDirection(Direction.South, toMove.Amount) },
            "W" => this with { Position = MoveDirection(Direction.West, toMove.Amount) },
            _ => throw new InvalidOperationException($"bad instruction action: {toMove}")
        };

        Point MoveForward(int amount) => MoveDirection(Facing, amount);
        Point MoveDirection(Direction direction, int amount) => direction switch
        {
            Direction.North => Position.OffsetY(amount),
            Direction.East => Position.OffsetX(amount),
            Direction.South => Position.OffsetY(-amount),
            Direction.West => Position.OffsetX(-amount),
            _ => throw new InvalidOperationException()
        };
    }

    public record Point(int X, int Y)
    {
        public static Point Origin => new(0, 0);

        public Point OffsetTimes(int times, Point other) => this with { X = X + other.X*times, Y = Y + other.Y*times };
        public Point Offset(Point other) => this with { X = X + other.X, Y = Y + other.Y };
        public Point OffsetY(int dy) => this with { Y = Y + dy };
        public Point OffsetX(int dx) => this with { X = X + dx };

        public int ManhattanDistanceFromOrigin => Math.Abs(X) + Math.Abs(Y);
        
        public Point RotateLeft(int degrees) => RotateRight(360-degrees);
        public Point RotateRight(int degrees) => (degrees / 90 % 4) switch
        {
            0 => this,
            1 => new Point(Y, -X),
            2 => new Point(-X, -Y),
            3 => new Point(-Y, X),
        };
    }

    public record FerryInstruction
    {
        public string Action { get; init; }
        public int Amount { get; init; }
    }

    public enum Direction { North, East, South, West }
    public static class DirectionExtensions
    {
        public static Direction TurnLeft(this Direction target, int degrees) =>
            (Direction)(((int)target - degrees / 90 + 4) % 4);
        public static Direction TurnRight(this Direction target, int degrees) =>
            (Direction)(((int)target + degrees / 90) % 4);
    }
}