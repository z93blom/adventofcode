using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Permutate<T>(this IEnumerable<T> source)
        {
            //return Permutations(source, source.Count());
            return Permutate(source, Enumerable.Empty<T>());
            IEnumerable<T[]> Permutate(IEnumerable<T> remainder, IEnumerable<T> prefix) =>
                !remainder.Any() ? new[] { prefix.ToArray() } :
                    remainder.SelectMany((c, i) => Permutate(
                        remainder.Take(i).Concat(remainder.Skip(i + 1)).ToArray(),
                        prefix.Append(c)));
        }

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> source, int size)
        {
            var elements = source.ToArray();
            var permutations = size == 0 
                ? Enumerable.Repeat(Enumerable.Empty<T>(), 1)
                : elements.SelectMany((element, index) => elements.Skip(index + 1)
                        .Permutations(size - 1)
                        .Select(c => Enumerable.Repeat(element, 1).Concat(c)));
            return permutations;
        }

        public static IEnumerable<Grouping<T, T>> GroupConsecutive<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }

                var key = enumerator.Current;
                var count = 1;
                while (enumerator.MoveNext())
                {
                    if (key.Equals(enumerator.Current))
                    {
                        count++;
                    }
                    else
                    {
                        yield return new Grouping<T, T>(key, Enumerable.Repeat(key, count));
                        key = enumerator.Current;
                        count = 1;
                    }
                }

                yield return new Grouping<T, T>(key, Enumerable.Repeat(key, count));
            }
        }
    }
}