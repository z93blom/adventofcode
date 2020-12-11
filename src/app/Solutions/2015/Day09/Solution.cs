using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AdventOfCode.Utilities;
using QuikGraph;
using QuikGraph.Algorithms;

namespace AdventOfCode.Y2015.Day09 {

    class Solution : ISolver {

        public string GetName() => "All in a Single Night";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var routeCosts = GetRouteCosts(input);
            var lowestCost = routeCosts.Min(pair => pair.Value);
            return lowestCost;
        }

        object PartTwo(string input) {
            var routeCosts = GetRouteCosts(input);
            var highestCost = routeCosts.Max(pair => pair.Value);
            return highestCost;
        }

        private static Dictionary<string[], int> GetRouteCosts(string input)
        {
            var distances = input
                .Lines()
                .Match(@"(\w+) to (\w+) = (\d+)")
                .Groups()
                .Select(g => new EdgeWithCost<string>(g.Elements[0], g.Elements[1], int.Parse(g.Elements[2])))
                .WithReverseEdge<string, EdgeWithCost<string>>();

            var graph = distances.ToBidirectionalGraph<string, EdgeWithCost<string>>();
            var routeCosts = new Dictionary<string[], int>();
            foreach (var permutation in graph.Vertices.Permutate())
            {
                var cost = 0;
                var isValidRoute = true;
                for (var i = 0; i < permutation.Length - 1; i++)
                {
                    if (!graph.TryGetEdge(permutation[i], permutation[i + 1], out var edge))
                    {
                        isValidRoute = false;
                        break;
                    }

                    cost += edge.Cost;
                }

                if (isValidRoute)
                {
                    routeCosts.Add(permutation, cost);
                }
            }

            return routeCosts;
        }
    }
}