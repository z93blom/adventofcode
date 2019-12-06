using System;
using System.Collections;
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
            var outerToInner = new Dictionary<string, string>();
            foreach (var edge in edges)
            {
                graph.AddVertex(edge.Source);
                if (!graph.ContainsVertex(edge.Target))
                {
                    graph.AddVertex(edge.Target);
                }

                graph.AddEdge(edge);
                outerToInner[edge.Target] = edge.Source;
            }

            var youPath = new List<string>();
            var current = "YOU";
            do
            {
                youPath.Add(current);
                current = outerToInner[current];
            } while (current != "COM");

            var sanPath = new List<string>();
            current = "SAN";
            do
            {
                sanPath.Add(current);
                current = outerToInner[current];
            } while (current != "COM");


            // Find the last common instance between the two paths
            var outermostCommon = string.Empty;
            foreach (var c in youPath)
            {
                if (sanPath.Contains(c))
                {
                    outermostCommon = c;
                    break;
                }
            }

            return youPath.IndexOf(outermostCommon) + sanPath.IndexOf(outermostCommon) - 2;
        }
    }
}