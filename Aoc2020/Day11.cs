using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            console.Output.WriteLine(SettleLayout(sampleLayout).NumOccupied);
            console.Output.WriteLine(SettleLayout(inputLayout).NumOccupied);
        }

        static Layout SettleLayout(Layout currentLayout)
        {
            Layout newLayout;
            while (currentLayout.NumOccupied != (newLayout = currentLayout.DoRoundPart1()).NumOccupied)
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
               .Then(s => new Layout(s));

            Seat[,] LoadSeats(string[] lines)
            {
                var seats = new Seat[lines[0].Length, lines.Length];

                for (var i = 0; i < lines.Length; i++)
                for (var j = 0; j < lines[i].Length; j++)
                    seats[j, i] = new Seat(Seat.GetState(lines[i][j]));

                return seats;
            }
        }
    }

    public record Layout(Seat[,] Seats)
    {
        private static readonly (int, int)[] NeighborDiffs = new[] { (-1, -1), (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1) };

        public Layout DoRoundPart1()
        {
            var newSeats = new Seat[Seats.GetLength(0), Seats.GetLength(1)];
            for (var i = 0; i < Seats.GetLength(0); i++)
            for (var j = 0; j < Seats.GetLength(1); j++)
                newSeats[i, j] = new Seat(NextState1((Seats[i, j].State, NumOccupiedNeighbors1(i, j))));

            return new Layout(newSeats);
        }
        public Layout DoRoundPart2()
        {
            var newSeats = new Seat[Seats.GetLength(0), Seats.GetLength(1)];
            for (var i = 0; i < Seats.GetLength(0); i++)
            for (var j = 0; j < Seats.GetLength(1); j++)
                newSeats[i, j] = new Seat(NextState2((Seats[i, j].State, NumOccupiedNeighbors2(i, j))));

            return new Layout(newSeats);
        }

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

        IEnumerable<(int i, int j, Seat seat)> All
        {
            get
            {
                for (var i = 0; i < Seats.GetLength(0); i++)
                for (var j = 0; j < Seats.GetLength(1); j++)
                    yield return (i, j, Seats[i, j]);
            }
        }

        public int NumOccupied => All
           .Select(x => x.seat.State)
           .Count(x => x == Seat.SeatState.Occupied);

        int NumOccupiedNeighbors1(int i, int j) => NeighborsFor(i, j)
           .Count(n => n.State == Seat.SeatState.Occupied);
        int NumOccupiedNeighbors2(int i, int j) => NeighborsFor(i, j)
           .Count(n => n.State == Seat.SeatState.Occupied);

        IEnumerable<Seat> NeighborsFor(int i, int j) => NeighborDiffs
           .Select((d) => (i + d.Item1, j + d.Item2))
           .Where(d => d.Item1 >= 0 && d.Item1 < Seats.GetLength(0))
           .Where(d => d.Item2 >= 0 && d.Item2 < Seats.GetLength(1))
           .Select(p => Seats[p.Item1, p.Item2]);

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < Seats.GetLength(0); i++)
            {
                for (var j = 0; j < Seats.GetLength(1); j++)
                    builder.Append(Seats[i, j].StateChar);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }
    
    public record Seat(Seat.SeatState State)
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