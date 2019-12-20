using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using AdventOfCode.Y2019.Day15;
using QuickGraph;
using QuickGraph.Algorithms;

namespace AdventOfCode.Y2019.Day20 {

    class Solution : ISolver {

        public string GetName() => "Donut Maze";

        private readonly Dictionary<Point, char> _map = new Dictionary<Point, char>();
        private readonly BidirectionalGraph<Point, Edge<Point>> _graph = new BidirectionalGraph<Point, Edge<Point>>();
        private Point _startingLocation;
        private Point _goal;
        const int levelOffset = 100;

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
            //var portals = new HashSet<Portal>();
            var unconnectedPortals = new Dictionary<string, Portal>();

            var textLocations = new HashSet<Point>(_map.Where(kvp => kvp.Value != '.').Select(kvp => kvp.Key));
            while (textLocations.Count > 0)
            {
                var point = textLocations.First();
                var adjacent = _allDirections
                    .Select(d => new {Point = point.GetPoint(d), Direction = d})
                    .Where(a => _map.ContainsKey(a.Point) && _map[a.Point] != '.')
                    .Select(a => new {a.Point, a.Direction, Type = _map[a.Point]})
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
                var pointAdjacentToMap = new[] {point, adjacent.Point}
                    .SelectMany(p => _allDirections.Select(d => new {PortalPoint = p, MapPoint = p.GetPoint(d)}))
                    .Where(a => _map.ContainsKey(a.MapPoint) && _map[a.MapPoint] == '.')
                    .ToArray()
                    .First();

                var isOuter = true;

                var portal = new Portal(portalName, pointAdjacentToMap.PortalPoint, pointAdjacentToMap.MapPoint, isOuter);

                // Connect it to the map.
                //_graph.AddVertex(pointAdjacentToMap.PortalPoint);
                //var edge = new Edge<Point>(pointAdjacentToMap.MapPoint, pointAdjacentToMap.PortalPoint);
                //_graph.AddEdge(edge);

                if (unconnectedPortals.ContainsKey(portal.Name))
                {
                    // The other portal has already been found.
                    // Connect them both to the map. Note that the portal is connected to the connected point (not to the other portal).
                    var otherPortal = unconnectedPortals[portalName];
                    var edge = new Edge<Point>(portal.MapLocation, otherPortal.MapLocation);
                    _graph.AddEdge(edge);
                    _graph.AddEdge(edge.Reverse);

                    //portals.Add(portal);
                    unconnectedPortals.Remove(portalName);
                }
                else
                {
                    // Not found.
                    unconnectedPortals[portalName] = portal;
                }
            }

            // We should have two "unconnected" portals: AA && ZZ (meaning the starting location and goal).
            _startingLocation = unconnectedPortals["AA"].MapLocation;
            _goal = unconnectedPortals["ZZ"].MapLocation;
        }

        private void AddAllNormalPoints(BidirectionalGraph<Point, Edge<Point>> graph, int level)
        {
            var standardPoints = new HashSet<Point>(_map.Where(kvp => kvp.Value == '.').Select(kvp => kvp.Key));
            foreach (var standardPoint in standardPoints)
            {
                graph.AddVertex(new Point(level * levelOffset + standardPoint.X, level * levelOffset + standardPoint.Y));
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

        object PartTwo(string input) {
            
            return 0;
        }
    }
}