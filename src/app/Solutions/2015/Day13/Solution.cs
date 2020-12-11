using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day13 {

    class Solution : ISolver {

        public string GetName() => "Knights of the Dinner Table";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        int PartOne(string input)
        {
            var values = input
                .Lines()
                .Match(@"(\w+) would (lose|gain) (\d+) .+ to (\w+)\.")
                .Groups()
                .Select(g => new {Person = g.Elements[0], Neighbor = g.Elements[3], Happiness = int.Parse(g.Elements[2]) * (g.Elements[1] == "gain" ? 1 : -1)})
                .ToList();

            var persons = values.Select(d => d.Person).Distinct().ToList();
            var lookup = values.ToDictionary(d => d.Person + "->" + d.Neighbor, d => d.Happiness);

            var bestScore = int.MinValue;
            foreach (var permutation in persons.Permutate())
            {
                var scores = permutation.Select((p, index) => new
                    {
                        Name = p,
                        Score = lookup[p + "->" + permutation[(index == 0 ? permutation.Length : index) - 1]] + lookup[p + "->" + permutation[(index + 1) % permutation.Length]]
                    })
                    .ToImmutableArray();
                var score = scores.Sum(v => v.Score);
                if (score > bestScore)
                {
                    bestScore = score;
                }
            }

            return bestScore;
        }

        object PartTwo(string input) {
            var values = input
                .Lines()
                .Match(@"(\w+) would (lose|gain) (\d+) .+ to (\w+)\.")
                .Groups()
                .Select(g => new {Person = g.Elements[0], Neighbor = g.Elements[3], Happiness = int.Parse(g.Elements[2]) * (g.Elements[1] == "gain" ? 1 : -1)})
                .ToList();

            var persons = values.Select(d => d.Person).Distinct().ToList();
            foreach (var person in persons)
            {
                values.Add(new {Person = "Me", Neighbor = person, Happiness = 0});
                values.Add(new {Person = person, Neighbor = "Me", Happiness = 0});
            }

            persons.Add("Me");

            var lookup = values.OrderBy(d => d.Person).ThenBy(d => d.Neighbor).ToDictionary(d => d.Person + "->" + d.Neighbor, d => d.Happiness);

            var bestScore = int.MinValue;
            foreach (var permutation in persons.Permutate())
            {
                var scores = permutation.Select((p, index) => new
                    {
                        Name = p,
                        Score = lookup[p + "->" + permutation[(index == 0 ? permutation.Length : index) - 1]] + lookup[p + "->" + permutation[(index + 1) % permutation.Length]]
                    })
                    .ToImmutableArray();
                var score = scores.Sum(v => v.Score);
                if (score > bestScore)
                {
                    bestScore = score;
                }
            }

            return bestScore;
        }
    }
}