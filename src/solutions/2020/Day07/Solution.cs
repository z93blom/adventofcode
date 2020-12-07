using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day07 {

    class Solution : ISolver {

        public string GetName() => "Handy Haversacks";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var containedIn = new Dictionary<string, HashSet<string>>();
            var regex = new Regex(@"(.+) bags contain (.*)");
            var r2 = new Regex(@"(\d+) ([^\.,]*) bags?[\.,]");
            foreach (var l in input.Lines())
            {
                var match = regex.Match(l);
                var m2 = r2.Matches(match.Groups[2].Value);
                var container = match.Groups[1].Value;
                foreach(var bagMatch in m2.OfType<Match>())
                {
                    var bag = bagMatch.Groups[2].Value;
                    if (!containedIn.ContainsKey(bag))
                    {
                        containedIn[bag] = new HashSet<string>();
                    }
                    containedIn[bag].Add(container);
                }
            }

            // Assume no loops!
            var containers = new HashSet<string>();
            var q = new Queue<string>();
            q.Enqueue("shiny gold");
            while(q.Count > 0)
            {
                var l = q.Dequeue();
                if (!containedIn.ContainsKey(l))
                    continue;
                foreach(var container in containedIn[l])
                {
                    containers.Add(container);
                    q.Enqueue(container);
                }
            }

            return containers.Count;
        }

        public record BagCount
        {
            public int Count { get; }
            public string Color { get; }

            public BagCount(int count, string color) => (Count, Color) = (count, color);
        }

        object PartTwo(string input) {
            var contains = new Dictionary<string, List<BagCount>>();
            var regex = new Regex(@"(.+) bags contain (.*)");
            var r2 = new Regex(@"(\d+) ([^\.,]*) bags?[\.,]");
            foreach (var l in input.Lines())
            {
                var match = regex.Match(l);
                var m2 = r2.Matches(match.Groups[2].Value);
                var container = match.Groups[1].Value;
                var c = m2.OfType<Match>()
                    .Select(m => new BagCount(int.Parse(m.Groups[1].Value), m.Groups[2].Value))
                    .ToList();
                contains[match.Groups[1].Value] = c;
            }

            var cache = new Dictionary<string, long>();
            var count = BagsInside(contains, cache, "shiny gold");

            return count;
        }

        private long BagsInside(Dictionary<string, List<BagCount>> lookup, Dictionary<string, long> cache, string color)
        {
            if (cache.ContainsKey(color))
            {
                return cache[color];
            }

            long count = 0;

            foreach(var bc in lookup[color])
            {
                var inside = BagsInside(lookup, cache, bc.Color) + 1;
                var t = bc.Count * inside;
                count += t;
            }

            cache[color] = count;
            return count;
        }
    }
}