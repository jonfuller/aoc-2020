using System;

namespace Aoc2020
{
    public static class Extensions
    {
        public static TResult Then<TInput, TResult>(this TInput target, Func<TInput, TResult> f) => f(target);
    }
}