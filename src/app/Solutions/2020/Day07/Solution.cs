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

        object PartOne(string input) 
        {
            var bagCanBeContainedIn = new Dictionary<string, HashSet<string>>();
            var outer = new Regex(@"(.+) bags contain (.*)");
            var inner = new Regex(@"(\d+) ([^\.,]*) bags?[\.,]");
            foreach (var l in input.Lines())
            {
                var outerMatch = outer.Match(l);
                var innerMatch = inner.Matches(outerMatch.Groups[2].Value);
                var container = outerMatch.Groups[1].Value;
                foreach(var match in innerMatch.OfType<Match>())
                {
                    var bag = match.Groups[2].Value;
                    if (!bagCanBeContainedIn.ContainsKey(bag))
                        bagCanBeContainedIn[bag] = new HashSet<string>();
                    bagCanBeContainedIn[bag].Add(container);
                }
            }

            var possibleContainers = new HashSet<string>();
            var q = new Queue<string>();
            q.Enqueue("shiny gold");
            while(q.Count > 0)
            {
                var l = q.Dequeue();
                if (!bagCanBeContainedIn.ContainsKey(l))
                    continue;
                foreach(var container in bagCanBeContainedIn[l])
                {
                    possibleContainers.Add(container);
                    q.Enqueue(container);
                }
            }

            return possibleContainers.Count;
        }

        public record BagCount
        {
            public int Count { get; }
            public string Color { get; }
            public BagCount(int count, string color) => (Count, Color) = (count, color);
        }

        object PartTwo(string input)
        {
            var bagContents = new Dictionary<string, List<BagCount>>();
            var outer = new Regex(@"(.+) bags contain (.*)");
            var inner = new Regex(@"(\d+) ([^\.,]*) bags?[\.,]");
            foreach (var l in input.Lines())
            {
                var outerMatch = outer.Match(l);
                var innerMatch = inner.Matches(outerMatch.Groups[2].Value);
                var container = outerMatch.Groups[1].Value;
                var contents = innerMatch.OfType<Match>()
                    .Select(m => new BagCount(int.Parse(m.Groups[1].Value), m.Groups[2].Value))
                    .ToList();
                bagContents[outerMatch.Groups[1].Value] = contents;
            }

            var count = BagsInside(bagContents, new Dictionary<string, long>(), "shiny gold");
            return count;
        }

        private long BagsInside(Dictionary<string, List<BagCount>> lookup, Dictionary<string, long> cache, string color)
        {
            if (cache.ContainsKey(color))
                return cache[color];

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