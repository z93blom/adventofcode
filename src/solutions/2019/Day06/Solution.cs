using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using QuickGraph.Algorithms.Search;

namespace AdventOfCode.Y2019.Day06 {

    class Solution : ISolver {

        public string GetName() => "Universal Orbit Map";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var edges = input.Lines()
                .Select(l => new Edge<string>(l.Substring(0, 3), l.Substring(4, 3)));
            var graph = new QuickGraph.BidirectionalGraph<string, Edge<string>>();
            foreach (var edge in edges)
            {
                graph.AddVertex(edge.Source);
                if (!graph.ContainsVertex(edge.Target))
                {
                    graph.AddVertex(edge.Target);
                }

                graph.AddEdge(edge);
            }

            var distanceFromCom = new Dictionary<string, int>();

            var orbits = new Queue<string>();
            orbits.Enqueue("COM");
            distanceFromCom["COM"] = 0;
            int totalOrbits = 0;
            while(orbits.Count > 0)
            {
                var orbit = orbits.Dequeue();
                if (graph.TryGetOutEdges(orbit, out var fromThis))
                {
                    var distanceToThis = distanceFromCom[orbit] + 1;
                    foreach (var outerOrbit in fromThis)
                    {
                        distanceFromCom[outerOrbit.Target] = distanceToThis;
                        orbits.Enqueue(outerOrbit.Target);
                        totalOrbits += distanceToThis;
                    }
                }
            }

            return totalOrbits;
        }

        object PartTwo(string input) {
            var edges = input.Lines()
                .Select(l => new Edge<string>(l.Substring(0, 3), l.Substring(4, 3)));
            var graph = new QuickGraph.BidirectionalGraph<string, Edge<string>>();
            foreach (var edge in edges)
            {
                graph.AddVertex(edge.Source);
                if (!graph.ContainsVertex(edge.Target))
                {
                    graph.AddVertex(edge.Target);
                }

                graph.AddEdge(edge);
            }

            //https://github.com/YaccConstructor/QuickGraph/wiki/Floyd-Warshall-All-Path-Shortest-Path
            var s = new QuickGraph.Algorithms.ShortestPath.FloydWarshallAllShortestPathAlgorithm<string, Edge<string>>(graph, e => 1);
            s.Compute();
            if (!s.TryGetPath("YOU", "SAN", out var path))
            {
                throw new Exception("No path found!");
            }

            return path.Count() - 2;

        }
    }
}