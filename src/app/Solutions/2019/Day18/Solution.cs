using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode.Utilities;
using AdventOfCode.Y2019.Day15;
using QuikGraph;
using QuikGraph.Algorithms;

namespace AdventOfCode.Y2019.Day18 {

    class Solution : ISolver {

        public string GetName() => "Many-Worlds Interpretation";

        Direction[] _allDirections;

        Point _startingLocation;
        Dictionary<Point, char> _keys;
        Dictionary<Point, char> _doors;
        HashSet<Point> _map;
        Dictionary<char, Area> _areas;

        Quadrant[] _quadrants = Enum.GetValues(typeof(Quadrant)).Cast<Quadrant>().ToArray();

        public IEnumerable<object> Solve(string input) {

            _allDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();
            CreateMap(input, out _map, out _startingLocation, out _keys, out _doors);
            _areas = GetAreas();

            yield return PartOne(input);
            yield return PartTwo(input);
        }

        struct Area
        {
            public char IncomingDoor { get; }
            public Point[] OutgoingDoors { get; }
            public Point[] Keys { get; }
            public HashSet<Point> Points { get; }

            public Area(char incomingDoor, IEnumerable<Point> points, Dictionary<Point, char> keys, Dictionary<Point, char> doors)
            {
                IncomingDoor = incomingDoor;
                Points = new HashSet<Point>(points);
                OutgoingDoors = Points.Where(p => doors.ContainsKey(p)).ToArray();
                Keys = Points.Where(p => keys.ContainsKey(p)).ToArray();
            }

            public Area(char incomingDoor, Point doorLocation, IEnumerable<Point> points, Dictionary<Point, char> keys, Dictionary<Point, char> doors)
            {
                IncomingDoor = incomingDoor;
                Points = new HashSet<Point>(points);
                OutgoingDoors = Points.Where(p => doors.ContainsKey(p) && p != doorLocation).ToArray();
                Keys = Points.Where(p => keys.ContainsKey(p)).ToArray();
            }

        }

        enum Quadrant
        {
            NorthWest,
            NorthEast,
            SouthEast,
            SouthWest
        }

        object PartOne(string input) 
        {
            // Is there any quadrant we can solve with just the keys available at the start and the keys in that quadrant?
            var areaToQuadrantMap = new Dictionary<Area, Quadrant>();
            var lockedKeysPerQuadrant = _quadrants.ToDictionary(q => q, q => new List<char>());
            var doorsNeededPerQuadrant = _quadrants.ToDictionary(q => q, q => new List<char>());
            foreach (var area in _areas.Values.Except(Enumerable.Repeat(_areas['@'], 1)))
            {
                var point = area.Points.First();
                Quadrant quadrant;
                if (point.X < _startingLocation.X && point.Y < _startingLocation.Y)
                {
                    quadrant = Quadrant.NorthWest;
                }
                else if (point.X > _startingLocation.X && point.Y < _startingLocation.Y)
                {
                    quadrant = Quadrant.NorthEast;
                }
                else if (point.X > _startingLocation.X && point.Y > _startingLocation.Y)
                {
                    quadrant = Quadrant.SouthEast;
                }
                else 
                {
                    quadrant = Quadrant.SouthWest;
                }

                areaToQuadrantMap[area] = quadrant;
                lockedKeysPerQuadrant[quadrant].AddRange(area.Keys.Select(p => _keys[p]));
                doorsNeededPerQuadrant[quadrant].Add(area.IncomingDoor);

            }
            
            var keysAvailableAtStart = _areas['@'].Keys.Select(p => _keys[p]).ToArray();

            var keysAvailableToEachQuadrant = _quadrants.ToDictionary(q => q, q => new List<char>(lockedKeysPerQuadrant[q].Concat(keysAvailableAtStart)));

            var solvableQuadrants = _quadrants.Where(q => doorsNeededPerQuadrant[q].All(c => keysAvailableToEachQuadrant[q].Contains(c))).ToArray();

            //Well, that didn't solve anything.

            // Let's see if we can figure out something based on the doors instead.
            var keysBehindDoors = _areas.Keys
                .Except(Enumerable.Repeat('@', 1))
                .Select(c => _areas[c])
                .ToDictionary(a => a.IncomingDoor, a => GetLockedKeys(a).ToArray());

            return 0;
        }

        private IEnumerable<char> GetLockedKeys(Area lockedArea)
        {
            foreach (var outgoingDoor in lockedArea.OutgoingDoors.Select(p => _doors[p]).Where(c => _areas.ContainsKey(c)))
            {
                foreach(var key in GetLockedKeys(_areas[outgoingDoor]))
                {
                    yield return key;
                }
            }

            foreach(var k in lockedArea.Keys.Select(p => _keys[p]))
            {
                yield return k;
            }
        }

        Dictionary<char, Area> GetAreas()
        {
            var areas = new Dictionary<char, Area>();
            var startingArea = new Area('@', ReachablePoints(_startingLocation), _keys, _doors);
            areas.Add(startingArea.IncomingDoor, startingArea);
            var coveredPoints = new HashSet<Point>(startingArea.Points);

            var areasToCheck = new Queue<Area>();
            areasToCheck.Enqueue(startingArea);
            while (areasToCheck.Count > 0)
            {
                var area = areasToCheck.Dequeue();
                // Get all the areas on the other side of this area.
                foreach (var doorPoint in area.OutgoingDoors)
                {
                    var door = _doors[doorPoint];
                    foreach (var p in _allDirections.Select(dir => doorPoint.GetPoint(dir)).Where(p => !coveredPoints.Contains(p) && _map.Contains(p)))
                    {
                        // p is an uncovered point that is on the map.
                        var newArea = new Area(door, doorPoint, ReachablePoints(p), _keys, _doors);

                        if (newArea.Points.Count > 0)
                        {

                            foreach (var point in newArea.Points)
                            {
                                coveredPoints.Add(point);
                            }

                            if (newArea.Keys.Any() || newArea.OutgoingDoors.Any())
                            {
                                // We should only have a single area behind a door. (Otherwise .Add will fail).
                                areas.Add(newArea.IncomingDoor, newArea);
                            }

                            if (newArea.OutgoingDoors.Any())
                            {
                                areasToCheck.Enqueue(newArea);
                            }
                        }
                    }
                }
            }

            return areas;
        }

        public BidirectionalGraph<Point, Edge<Point>> BuildDoorGraph(Point location)
        {
            var graph = new BidirectionalGraph<Point, Edge<Point>>();
            var keysUnlocked = new Dictionary<char, List<char>>();

            var reachablePoints = ReachablePoints(location).ToArray();
            var doorsReachable = reachablePoints.Where(p => _doors.ContainsKey(p)).ToArray();
            foreach(var source in doorsReachable)
            {
                // Connect them together.
                foreach (var target in doorsReachable)
                {
                    if (source != target)
                    {
                        if (!graph.ContainsVertex(source))
                        {
                            graph.AddVertex(source);
                        }

                        if (!graph.ContainsVertex(target))
                        {
                            graph.AddVertex(target);
                        }

                        graph.AddEdge(new Edge<Point>(source, target));
                    }
                }
            }

            return graph;
        }

        private IEnumerable<Point> ReachablePoints(Point location)
        {
            var pointsToCheck = new Queue<Point>();
            var pointsVisited = new HashSet<Point>();
            pointsToCheck.Enqueue(location);
            pointsVisited.Add(location);
            while (pointsToCheck.Count > 0)
            {
                var source = pointsToCheck.Dequeue();
                foreach (var dir in _allDirections)
                {
                    // Is there a location on the map in this direction?
                    var target = source.GetPoint(dir);
                    if (_map.Contains(target) && !pointsVisited.Contains(target))
                    {
                        yield return target;
                        // Is it a door?
                        if (!_doors.ContainsKey(target))
                        {
                            pointsToCheck.Enqueue(target);
                        }

                        pointsVisited.Add(target);
                    }
                }
            }
        }

        private BidirectionalGraph<Point, Edge<Point>> BuildGraph(HashSet<Point> map, char[] keys, Dictionary<Point, char> doors, Point startingLocation)
        {
            var graph = new BidirectionalGraph<Point, Edge<Point>>();
            graph.AddVertex(startingLocation);
            var pointsToCheck = new Queue<Point>();
            pointsToCheck.Enqueue(startingLocation);
            while (pointsToCheck.Count > 0)
            {
                var source = pointsToCheck.Dequeue();
                foreach (var dir in _allDirections)
                {
                    // Is there a location on the map in this direction?
                    var target = source.GetPoint(dir);
                    if (map.Contains(target) && ! graph.ContainsVertex(target))
                    {
                        // Valid point that we can move to.
                        graph.AddVertex(target);
                        graph.AddEdge(new Edge<Point>(source, target));
                        graph.AddEdge(new Edge<Point>(target, source));

                        // Can we continue past this location?
                        if (!doors.ContainsKey(target) || keys.Contains(doors[target]))
                        {
                            pointsToCheck.Enqueue(target);
                        }
                    }
                }
            }

            return graph;
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
            var graph = new QuikGraph.BidirectionalGraph<Point, Edge<Point>>();
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