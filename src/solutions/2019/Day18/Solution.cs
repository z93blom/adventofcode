using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Convention;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Utilities;
using AdventOfCode.Y2019.Day15;
using Microsoft.CodeAnalysis.Options;
using QuickGraph;
using QuickGraph.Algorithms;

namespace AdventOfCode.Y2019.Day18 {

    class Solution : ISolver {

        public string GetName() => "Many-Worlds Interpretation";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            CreateMap(input, out var map, out var startingLocation, out var keys, out var doors);

            var graph = BuildGraph(map);
            var keyLocations = new Dictionary<char, Point>();
            foreach (var kvp in keys)
            {
                keyLocations.Add(kvp.Value, kvp.Key);
            }

            var possiblePaths = new List<List<char>>();
            GetPaths(new List<char>(), startingLocation, graph, keys, doors, possiblePaths);

            var distances = new Dictionary<List<char>, int>();

            // Order them by how far we have to travel to collect all the keys in the order specified.
            foreach (var possiblePath in possiblePaths)
            {
                var currentLocation = startingLocation;
                var totalDistance = 0;
                foreach (var key in possiblePath)
                {
                    totalDistance += Distance(currentLocation, keyLocations[key], graph);
                    currentLocation = keyLocations[key];
                }

                distances[possiblePath] = totalDistance;
            }

            return distances.Min(kvp => kvp.Value);
        }


        object PartTwo(string input) {
            return 0;
        }

        private int Distance(Point p1, Point p2, BidirectionalGraph<Point, Edge<Point>> graph)
        {
            graph.ShortestPathsDijkstra(e => 1, p1)(p2, out var edges);
            return edges.Count();
        }

        private BidirectionalGraph<Point, Edge<Point>> BuildGraph(HashSet<Point> map)
        {
            var location = map.First();
            var graph = new QuickGraph.BidirectionalGraph<Point, Edge<Point>>();
            foreach (var point in map)
            {
                graph.AddVertex(point);
            }

            foreach (var point in map)
            {
                foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
                {
                    var target = point.GetPoint(dir);
                    if (map.Contains(target))
                    {
                        graph.AddEdge(new Edge<Point>(point, target));
                    }
                }
            }

            return graph;
        }


        private void GetPaths(List<char> keyCollectionOrder, Point currentPos, BidirectionalGraph<Point, Edge<Point>> graph, Dictionary<Point, char> keys, Dictionary<Point, char> doors, List<List<char>> possiblePaths)
        {
            // Have we collected all the keys?
            if (keys.Count == keyCollectionOrder.Count)
            {
                possiblePaths.Add(keyCollectionOrder);
                return;
            }

            // Which uncollected keys can we reach from where we are right now?
            //var reachablePoints = FloodFill(currentPos, map, doors, keyCollectionOrder);
            //var reachableKeys = keys.Where(p => reachablePoints.Contains(p.Key) && ! keyCollectionOrder.Contains(p.Value));
            var reachableKeys = ReachableUncollectedKeys(currentPos, graph, doors, keyCollectionOrder, keys);
            // Try each and every one of them out.
            Parallel.ForEach(reachableKeys, tuple =>
            {
                var collected = new List<char>(keyCollectionOrder) {tuple.key};
                GetPaths(collected, tuple.point, graph, keys, doors, possiblePaths);
            });
        }

        private void CreateMap(string input, out HashSet<Point> map, out Point startingLocation, out Dictionary<Point, char> keys, out Dictionary<Point, char> doors)
        {
            map = new HashSet<Point>();
            keys = new Dictionary<Point, char>();
            doors = new Dictionary<Point, char>();
            startingLocation = new Point(-1, -1);
            var y = 0;
            foreach (var line in input.Lines())
            {
                var x = 0;
                foreach (var c in line)
                {
                    if (c == '.')
                    {
                        map.Add(new Point(x, y));
                    }
                    else if (c >= 'a' && c <= 'z')
                    {
                        // Key
                        map.Add(new Point(x, y));
                        keys.Add(new Point(x, y), c);
                    }
                    else if (c >= 'A' && c <= 'Z')
                    {
                        // Door
                        map.Add(new Point(x, y));
                        doors.Add(new Point(x, y), char.ToLower(c, CultureInfo.InvariantCulture));
                    }
                    else if (c == '@')
                    {
                        // Starting location
                        map.Add(new Point(x, y));
                        startingLocation = new Point(x, y);
                    }

                    x++;
                }
                y++;
            }
        }

        private IEnumerable<(Point point, char key)> ReachableUncollectedKeys(Point currentPos, BidirectionalGraph<Point, Edge<Point>> graph, Dictionary<Point, char> doors, List<char> ownedKeys, Dictionary<Point, char> keys)
        {
            var tryFunc = graph.ShortestPathsDijkstra(e => 1, currentPos);

            foreach (var point in keys.Where(kvp => !ownedKeys.Contains(kvp.Value)).Select(kvp => kvp.Key))
            {
                if (tryFunc(point, out var edges))
                {
                    var isReachable = true;
                    foreach (var edge in edges)
                    {
                        if (doors.ContainsKey(edge.Target) && !ownedKeys.Contains(doors[edge.Target]))
                        {
                            // Impassable at the moment.
                            isReachable = false;
                            break;
                        }
                    }

                    if (isReachable)
                    {
                        yield return (point, keys[point]);
                    }
                }
            }
        }

        private HashSet<Point> FloodFill(Point currentPos, HashSet<Point> map, Dictionary<Point, char> doors, List<char> ownedKeys)
        {
            // Check all directions from this point.
            var pointsToCheck = new Stack<Point>();
            pointsToCheck.Push(currentPos);



            var checkedPoints = new HashSet<Point>();
            var reachablePoints = new HashSet<Point>();
            while (pointsToCheck.Count > 0)
            {
                var pointToCheck = pointsToCheck.Pop();
                var dirsToCheck = Enum.GetValues(typeof(Direction)).Cast<Direction>();
                foreach (var dir in dirsToCheck)
                {
                    var targetPos = pointToCheck.GetLocation(dir);
                    if (map.Contains(targetPos) && !checkedPoints.Contains(targetPos))
                    {
                        if (!doors.ContainsKey(targetPos) || ownedKeys.Contains(doors[targetPos]))
                        {
                            // We can reach this point.
                            reachablePoints.Add(targetPos);
                            pointsToCheck.Push(targetPos);
                        }
                    }

                    checkedPoints.Add(targetPos);
                }
            }

            return reachablePoints;
        }

    }
}