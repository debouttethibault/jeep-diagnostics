using System;
using System.Collections.Generic;

namespace JeepDiag.WPF
{
    public static class ListExtensions
    {
        private static readonly Random rng = new();

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }
    }
}
