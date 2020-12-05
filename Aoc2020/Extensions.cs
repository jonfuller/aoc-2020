using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2020
{
    public static class Extensions
    {
        public static TResult Then<TInput, TResult>(this TInput target, Func<TInput, TResult> f) => f(target);
        public static void Then<TInput>(this TInput target, Action<TInput> f) => f(target);

        public static IEnumerable<T> FrontHalf<T>(this IEnumerable<T> target)
        {
            var listed = target.ToList();
            return listed.GetRange(0, listed.Count / 2);
        }

        public static IEnumerable<T> BackHalf<T>(this IEnumerable<T> target)
        {
            var listed = target.ToList();
            return listed.Skip(listed.Count / 2);
        }
    }
}