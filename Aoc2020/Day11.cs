using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Aoc2020
{
    [Command(name: "day-11")]
    public class Day11 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var sampleLayout = LoadLayout("input/11-sample");
            var inputLayout = LoadLayout("input/11-input");
            console.Output.WriteLine(SettleLayout(sampleLayout, l => l.DoRoundPart1()).NumOccupied);
            console.Output.WriteLine(SettleLayout(inputLayout, l => l.DoRoundPart1()).NumOccupied);
            console.Output.WriteLine(SettleLayout(sampleLayout, l => l.DoRoundPart2()).NumOccupied);
        }

        static Layout SettleLayout(Layout currentLayout, Func<Layout, Layout> round)
        {
            Layout newLayout;
            while (currentLayout.NumOccupied != (newLayout = round(currentLayout)).NumOccupied)
            {
                currentLayout = newLayout;
            }
            return currentLayout;
        }

        static Layout LoadLayout(string filename)
        {
            return File
               .ReadAllLines(filename)
               .Then(LoadSeats)
               .Then(s => new Layout{Seats = s.ToList()});

            static IEnumerable<Seat> LoadSeats(string[] lines)
            {
                for (var y = 0; y < lines.Length; y++)
                for (var x = 0; x < lines[y].Length; x++)
                    yield return new Seat(Seat.GetState(lines[y][x]), x, y);
            }
        }
    }

    public record Layout
    {
        private static readonly (int, int)[] NeighborDiffs = new[]
            {(-1, -1), (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1)};

        private readonly int _maxX;
        private readonly int _maxY;
        private readonly IList<Seat> _seats;
        private readonly Dictionary<Seat, IList<Seat>> _neighborCache;
        private readonly Dictionary<Seat, IList<Seat>> _nearestCache;

        public IList<Seat> Seats
        {
            get => _seats;
            init
            {
                _seats = value;
                _maxX = _seats.Max(s => s.X);
                _maxY = _seats.Max(s => s.Y);
                var xyLookup = _seats
                   .GroupBy(s => s.X, s => s)
                   .ToDictionary(
                        grp => grp.Key,
                        grp => grp.OrderBy(i => i.Y).ToArray())
                   .Select(x => x.Value)
                   .ToArray();
                
                _neighborCache = _seats.ToDictionary(
                    s => s,
                    s => NeighborsFor(s, xyLookup));
                _nearestCache = _seats.ToDictionary(
                    s => s,
                    s => NearestFor(s, xyLookup));
            }
        }

        public Layout DoRoundPart1() => this with
        {
            Seats = Seats.Select(s => s with
            {
                State = NextState1((s.State, NumOccupiedNeighbors1(s)))
            }).ToList()
        };
        public Layout DoRoundPart2() => this with
        {
            Seats = Seats.Select(s => s with
            {
                State = NextState2((s.State, NumOccupiedNeighbors2(s)))
            }).ToList()
        };

        static Seat.SeatState NextState1((Seat.SeatState current, int numOccupiedNeighbors) x) => x switch
        {
            (Seat.SeatState.Empty, numOccupiedNeighbors: 0) => Seat.SeatState.Occupied,
            (Seat.SeatState.Occupied, numOccupiedNeighbors: >= 4) => Seat.SeatState.Empty,
            _ => x.current
        };
        static Seat.SeatState NextState2((Seat.SeatState current, int numOccupiedNeighbors) x) => x switch
        {
            (Seat.SeatState.Empty, numOccupiedNeighbors: 0) => Seat.SeatState.Occupied,
            (Seat.SeatState.Occupied, numOccupiedNeighbors: >= 5) => Seat.SeatState.Empty,
            _ => x.current
        };

        public int NumOccupied => Seats.Count(seat => seat.State == Seat.SeatState.Occupied);

        int NumOccupiedNeighbors1(Seat seat) => _neighborCache[seat].Count(x => x.State == Seat.SeatState.Occupied);
        int NumOccupiedNeighbors2(Seat seat) => _nearestCache[seat].Count(x => x.State == Seat.SeatState.Occupied);

        IList<Seat> NearestFor(Seat seat, Seat[][] xyLookup)
        {
            return null;
        }

        IList<Seat> NeighborsFor(Seat seat, Seat[][] xyLookup)
        {
            return _().ToList();

            IEnumerable<Seat> _()
            {
                foreach (var (x, y) in NeighborDiffs)
                {
                    var (newX, newY) = (x: seat.X + x, y: seat.Y + y);

                    if (newX < 0) continue;
                    if (newX > _maxX) continue;

                    if (newY < 0) continue;
                    if (newY > _maxY) continue;

                    yield return xyLookup[newX][newY];
                }
            }
        }

        //public override string ToString()
        //{
        //    var builder = new StringBuilder();
        //    for (var i = 0; i < Seats.GetLength(0); i++)
        //    {
        //        for (var j = 0; j < Seats.GetLength(1); j++)
        //            builder.Append(Seats[i, j].StateChar);
        //        builder.Append(Environment.NewLine);
        //    }
        //    return builder.ToString();
        //}
    }
    
    public record Seat(Seat.SeatState State, int X, int Y)
    {
        public enum SeatState { Occupied, Empty, Floor }

        public static SeatState GetState(char c) => c switch
        {
            'L' => SeatState.Empty,
            '.' => SeatState.Floor,
            '#' => SeatState.Occupied,
            _ => throw new InvalidDataException($"bad seat state: {c}"),
        };

        public char StateChar => State switch
        {
            SeatState.Floor => '.',
            SeatState.Occupied => '#',
            SeatState.Empty => 'L',
            _ => throw new InvalidDataException($"bad seat state: {State}")
        };
    }
}