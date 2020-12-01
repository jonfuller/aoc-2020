using System;
using System.IO;
using System.Linq;
using MoreLinq;

namespace Aoc2020
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = File
               .ReadAllLines("input")
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

            Console.WriteLine(part1.prod);
            Console.WriteLine(part2.prod);
        }
    }
}
