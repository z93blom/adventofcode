using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AdventOfCode.Utilities;
using AdventOfCode.Y2019.Day15;
using QuikGraph;
using QuikGraph.Algorithms;

namespace AdventOfCode.Y2019.Day20
{

    class Solution : ISolver {

        public string GetName() => "Donut Maze";

        private readonly Dictionary<Point, char> _map = new Dictionary<Point, char>();
        private readonly BidirectionalGraph<Point, Edge<Point>> _graph = new BidirectionalGraph<Point, Edge<Point>>();
        private Point _startingLocation;
        private Point _goal;
        const int levelOffset = 200;

        readonly Direction[] _allDirections;

        public Solution()
        {
            _allDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray(); ;
        }

        public IEnumerable<object> Solve(string input)
        {
            CreateMap(input);

            yield return PartOne(input);
            yield return PartTwo(input);
        }

        struct Portal
        {
            public string Name { get; }
            public Point PortalLocation { get; }
            public Point MapLocation { get; }
            public bool IsOuter { get; }

            public Portal(string name, Point portalLocation, Point mapLocation, bool isOuter)
            {
                Name = name;
                PortalLocation = portalLocation;
                MapLocation = mapLocation;
                IsOuter = isOuter;
            }
        }

        private void CreateGraph()
        {
            _graph.Clear();

            // Add all "normal" points to the graph.
            AddAllNormalPoints(_graph, 0);

            // Get all the portal locations.
            var portals = GetPortals().GroupBy(p => p.Name);

            foreach(var portalPair in portals.Where(g => g.Count() == 2))
            {
                var pair = portalPair.ToArray();

                var portal = portalPair.First();
                var otherPortal = portalPair.Skip(1).First();
                var edge = new Edge<Point>(portal.MapLocation, otherPortal.MapLocation);
                _graph.AddEdge(edge);
                _graph.AddEdge(edge.Reverse);
            }

            _startingLocation = portals.First(p => p.Key == "AA").First().MapLocation;
            _goal = portals.First(p => p.Key == "ZZ").First().MapLocation;
        }

        private HashSet<Portal> GetPortals()
        {
            var minX = _map.Min(kvp => kvp.Key.X);
            var maxX = _map.Max(kvp => kvp.Key.X);
            var minY = _map.Min(kvp => kvp.Key.Y);
            var maxY = _map.Max(kvp => kvp.Key.Y);

            var portals = new HashSet<Portal>();
            var textLocations = new HashSet<Point>(_map.Where(kvp => kvp.Value != '.').Select(kvp => kvp.Key));
            while (textLocations.Count > 0)
            {
                var point = textLocations.First();
                var adjacent = _allDirections
                    .Select(d => new { Point = point.GetPoint(d), Direction = d })
                    .Where(a => _map.ContainsKey(a.Point) && _map[a.Point] != '.')
                    .Select(a => new { a.Point, a.Direction, Type = _map[a.Point] })
                    .First();

                textLocations.Remove(point);
                textLocations.Remove(adjacent.Point);

                var portalName = string.Empty;
                switch (adjacent.Direction)
                {
                    case Direction.North:
                        portalName = _map[adjacent.Point].ToString() + _map[point];
                        break;
                    case Direction.South:
                        portalName = _map[point].ToString() + _map[adjacent.Point];
                        break;
                    case Direction.West:
                        portalName = _map[adjacent.Point].ToString() + _map[point];
                        break;
                    case Direction.East:
                        portalName = _map[point].ToString() + _map[adjacent.Point];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // The text is "normalized" (so we can find the accompanying portal).
                // What is the location of the portal? It depends on where the only adjacent '.' is to the portal.
                var pointAdjacentToMap = new[] { point, adjacent.Point }
                    .SelectMany(p => _allDirections.Select(d => new { PortalPoint = p, MapPoint = p.GetPoint(d) }))
                    .Where(a => _map.ContainsKey(a.MapPoint) && _map[a.MapPoint] == '.')
                    .ToArray()
                    .First();

                var isOuter = pointAdjacentToMap.PortalPoint.X == minX + 1
                    || pointAdjacentToMap.PortalPoint.X == maxX - 1
                    || pointAdjacentToMap.PortalPoint.Y == minY + 1
                    || pointAdjacentToMap.PortalPoint.Y == maxY - 1;

                var portal = new Portal(portalName, pointAdjacentToMap.PortalPoint, pointAdjacentToMap.MapPoint, isOuter);
                portals.Add(portal);
            }

            return portals;
        }

        private void AddAllNormalPoints(BidirectionalGraph<Point, Edge<Point>> graph, int level)
        {
            var standardPoints = new HashSet<Point>(_map.Where(kvp => kvp.Value == '.')
                .Select(kvp => GetPointForLevel(kvp.Key, level)));
            foreach (var standardPoint in standardPoints)
            {
                graph.AddVertex(standardPoint);
            }

            foreach (var standardPoint in standardPoints)
            {
                foreach (var adj in _allDirections.Select(d => standardPoint.GetPoint(d))
                    .Where(standardPoints.Contains))
                {
                    var edge = new Edge<Point>(standardPoint, adj);
                    graph.AddEdge(edge);
                }
            }
        }

        private static Point GetPointForLevel(Point point, int level)
        {
            return new Point(level * levelOffset + point.X, level * levelOffset + point.Y);
        }

        private void CreateMap(string input)
        {
            _map.Clear();
            var y = 1;
            foreach (var line in input.Lines())
            {
                var x = 1;
                foreach (var c in line)
                {
                    if (c == '.' || (c >= 'A' && c <= 'Z'))
                    {
                        _map.Add(new Point(x, y), c);
                    }
                    else if (c != '#' && c != ' ')
                    {
                        throw new Exception("Unknown map character");
                    }

                    x++;
                }
                y++;
            }
        }

        object PartOne(string input)
        {
            CreateGraph();
            var pathing = _graph.ShortestPathsDijkstra(e => 1, _startingLocation);
            pathing(_goal, out var path);
            

            return path.Count();
        }

        object PartTwo(string input) 
        {
            var portals = GetPortals();

            Portal startingPortal = portals.First(p => p.Name == "AA");
            _startingLocation = startingPortal.MapLocation;
            portals.Remove(startingPortal);

            Portal goalPortal = portals.First(p => p.Name == "ZZ");
            _goal = goalPortal.MapLocation;
            portals.Remove(goalPortal);

            _graph.Clear();
            var level = 0;
            while (true)
            {
                AddAllNormalPoints(_graph, level);
                AddPortalsForLevel(_graph, portals, level);

                var pathing = _graph.ShortestPathsDijkstra(e => 1, _startingLocation);
                if (pathing(_goal, out var path))
                {
                    return path.Count();
                }

                level++;
            }
        }   

        private void AddPortalsForLevel(BidirectionalGraph<Point, Edge<Point>> graph, HashSet<Portal> portals, int level)
        {
            foreach (var portalPair in portals.GroupBy(p => p.Name).Where(g => g.Count() == 2))
            {
                var pair = portalPair.ToArray();

                var outerPortal = portalPair.First(p => p.IsOuter);
                var innerPortal = portalPair.First(p => !p.IsOuter);

                // Connect the inner portal to the next level and the outer portal to to previous level.

                // Connect the inner portal at the level above this level to the outer portal at this level.
                var innerPortalSource = GetPointForLevel(innerPortal.MapLocation, level - 1);
                var innerPortalTarget = GetPointForLevel(outerPortal.MapLocation, level);

                if (graph.ContainsVertex(innerPortalSource) && graph.ContainsVertex(innerPortalTarget))
                {
                    var edge = new Edge<Point>(innerPortalSource, innerPortalTarget);
                    graph.AddEdge(edge);
                    graph.AddEdge(edge.Reverse);
                }

                // Connect the outer portal at this level to the inner portal at the level below this.
                var outerPortalSource = GetPointForLevel(outerPortal.MapLocation, level);
                var outerPortalTarget = GetPointForLevel(innerPortal.MapLocation, level + 1);

                if (graph.ContainsVertex(outerPortalSource) && graph.ContainsVertex(outerPortalTarget))
                {
                    var edge = new Edge<Point>(outerPortalSource, outerPortalTarget);
                    graph.AddEdge(edge);
                    graph.AddEdge(edge.Reverse);
                }
            }
        }
    }
}