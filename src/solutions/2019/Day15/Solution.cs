using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using System.Diagnostics;

namespace AdventOfCode.Y2019.Day15 {

    class Solution : ISolver {

        public string GetName() => "Oxygen System";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            //var vm = new IntCode(input);
            //var currentPos = new Point(0, 0);
            //var currentStatus = Status.Empty;
            //var map = new Dictionary<Point, Status>();
            ////var wayBack = new Dictionary<Point, Direction>();
            //var shortestDistance = new Dictionary<Point, int>();
            //shortestDistance[currentPos] = 0;
            //map[currentPos] = currentStatus;
            //var pointsToCheck = new Stack<Point>();
            //pointsToCheck.Push(currentPos);

            //var g = new QuickGraph.BidirectionalGraph<Point, Edge<Point>>();
            //g.AddVertex(currentPos);

            //FloodFill(vm, currentPos, shortestDistance, map, g);

            //var system = map.First(kvp => kvp.Value == Status.System).Key;
            //return shortestDistance[system];

            var vm = new IntCode(input);
            var currentPos = new Point(0, 0);
            var currentStatus = Status.Empty;
            var map = new Dictionary<Point, Status>();
            map[currentPos] = currentStatus;
            var pointsToCheck = new Stack<Point>();
            pointsToCheck.Push(currentPos);

            var g = new BidirectionalGraph<Point, Edge<Point>>();
            g.AddVertex(currentPos);

            FloodFill(vm, ref currentPos, map, g);

            var systemPoint = map.First(kvp => kvp.Value == Status.System).Key;
            g.ShortestPathsDijkstra(e => 1, new Point(0, 0))(systemPoint, out var path);
            return path.Count();
        }

        object PartTwo(string input)
        {
            var vm = new IntCode(input);
            var currentPos = new Point(0, 0);
            var currentStatus = Status.Empty;
            var map = new Dictionary<Point, Status>();
            map[currentPos] = currentStatus;
            var pointsToCheck = new Stack<Point>();
            pointsToCheck.Push(currentPos);

            var g = new BidirectionalGraph<Point, Edge<Point>>();
            g.AddVertex(currentPos);

            FloodFill(vm, ref currentPos, map, g);

            // Now we've got the complete map. Let's just see how far away all other points are from the system point.
            var systemPoint = map.First(kvp => kvp.Value == Status.System).Key;
            var tryFunc = g.ShortestPathsDijkstra(e => 1, systemPoint);

            var maxDistance = map.Where(kvp => kvp.Value == Status.Empty)
                .Select(kvp =>
                {
                    tryFunc(kvp.Key, out var p);
                    return p.Count();
                })
                .Max();

            return maxDistance;
        }

        private void FloodFill(IntCode vm, Point startingPoint, Dictionary<Point, int> shortestDistance, Dictionary<Point, Status> map, BidirectionalGraph<Point, Edge<Point>> g)
        {
            // Check all directions from this point.
            var pointsToCheck = new Stack<Point>();
            pointsToCheck.Push(startingPoint);

            var currentPos = startingPoint;
            var pointStatus = new Dictionary<Direction, Status>();
            while (pointsToCheck.Count > 0)
            {
                var pointToCheck = pointsToCheck.Pop();
                Log(string.Empty);
                Log($"Checking new point {pointToCheck}");
                MoveTo(pointToCheck, ref currentPos, g, vm, $"new point to check {pointToCheck}");

                var distanceToAdj = shortestDistance[currentPos] + 1;

                var dirsToCheck = Enum.GetValues(typeof(Direction)).Cast<Direction>();
                pointStatus.Clear();
                foreach (var dir in dirsToCheck)
                {
                    var targetPos = currentPos.GetLocation(dir);
                    if (map.ContainsKey(targetPos))
                    {
                        if (map[targetPos] == Status.Wall)
                        {
                            // It's a wall. We can't move there.
                            continue;
                        }

                        // Already visited. Is this distance shorter?
                        if (shortestDistance[targetPos] > distanceToAdj)
                        {
                            shortestDistance[targetPos] = distanceToAdj;

                            pointsToCheck.Push(targetPos);
                        }

                        continue;
                    }

                    var currentStatus = Move(ref currentPos, dir, vm, $"checking out unknown position.");
                    pointStatus.Add(dir, currentStatus);
                    map[targetPos] = currentStatus;
                    shortestDistance[targetPos] = distanceToAdj;
                    switch (currentStatus)
                    {
                        case Status.Wall:
                            // The robot did not move, but it hit a wall.
                            break;
                        case Status.Empty:
                            // We moved, and are now at the target position
                            if (!g.ContainsVertex(targetPos))
                            {
                                g.AddVertex(targetPos);
                            }

                            g.AddEdge(new Edge<Point>(pointToCheck, targetPos));
                            g.AddEdge(new Edge<Point>(targetPos, pointToCheck));

                            // Add it as a point to check.
                            pointsToCheck.Push(targetPos);

                            // Then go back to the pointToCheck.
                            MoveTo(pointToCheck, ref currentPos, g, vm, $"point checked. Moving back to {pointToCheck}");

                            break;
                        case Status.System:
                            // We moved, and are now at the target position
                            if (!g.ContainsVertex(targetPos))
                            {
                                g.AddVertex(targetPos);
                            }

                            // We found the oxygen system. No need to check any further in this path. (No other way will be shorter than this).
                            // We just go back to the current position.

                            g.AddEdge(new Edge<Point>(pointToCheck, targetPos));
                            g.AddEdge(new Edge<Point>(targetPos, pointToCheck));

                            MoveTo(pointToCheck, ref currentPos, g, vm, $"point checked (System found). Moving back to {pointToCheck}");

                            break;
                    }
                }

                Log($"Finished with {pointToCheck}:");
                foreach(var kvp in pointStatus)
                {
                    Log($"  {kvp.Key}: {kvp.Value}");
                }
            }

        }

        private void FloodFill(IntCode vm, ref Point currentPos, Dictionary<Point, Status> map, BidirectionalGraph<Point, Edge<Point>> g)
        {
            // Check all directions from this point.
            var pointsToCheck = new Stack<Point>();
            pointsToCheck.Push(currentPos);

            var pointStatus = new Dictionary<Direction, Status>();
            while (pointsToCheck.Count > 0)
            {
                var pointToCheck = pointsToCheck.Pop();
                Log(string.Empty);
                Log($"Checking new point {pointToCheck}");
                MoveTo(pointToCheck, ref currentPos, g, vm, $"new point to check {pointToCheck}");

                var dirsToCheck = Enum.GetValues(typeof(Direction)).Cast<Direction>();
                pointStatus.Clear();
                foreach (var dir in dirsToCheck)
                {
                    var targetPos = currentPos.GetLocation(dir);
                    if (map.ContainsKey(targetPos))
                    {
                        continue;
                    }

                    var currentStatus = Move(ref currentPos, dir, vm, $"checking out unknown position.");
                    pointStatus.Add(dir, currentStatus);
                    map[targetPos] = currentStatus;
                    switch (currentStatus)
                    {
                        case Status.Wall:
                            // The robot did not move, but it hit a wall.
                            break;
                        case Status.Empty:
                            // We moved, and are now at the target position
                            if (!g.ContainsVertex(targetPos))
                            {
                                g.AddVertex(targetPos);
                            }

                            g.AddEdge(new Edge<Point>(pointToCheck, targetPos));
                            g.AddEdge(new Edge<Point>(targetPos, pointToCheck));

                            // Add it as a point to check.
                            pointsToCheck.Push(targetPos);

                            // Then go back to the pointToCheck.
                            MoveTo(pointToCheck, ref currentPos, g, vm, $"point checked. Moving back to {pointToCheck}");

                            break;
                        case Status.System:
                            // We moved, and are now at the target position
                            if (!g.ContainsVertex(targetPos))
                            {
                                g.AddVertex(targetPos);
                            }

                            // We found the oxygen system. No need to check any further in this path. (No other way will be shorter than this).
                            // We just go back to the current position.

                            g.AddEdge(new Edge<Point>(pointToCheck, targetPos));
                            g.AddEdge(new Edge<Point>(targetPos, pointToCheck));

                            MoveTo(pointToCheck, ref currentPos, g, vm, $"point checked (System found). Moving back to {pointToCheck}");

                            break;
                    }
                }

                Log($"Finished with {pointToCheck}:");
                foreach (var kvp in pointStatus)
                {
                    Log($"  {kvp.Key}: {kvp.Value}");
                }
            }

        }

        private void Log(string text)
        {
            //Debug.WriteLine(text);
        }



        private void MoveTo(Point target, ref Point currentPos, BidirectionalGraph<Point, Edge<Point>> g, IntCode vm, string because)
        {
            if (target == currentPos)
            {
                return;
            }

            var startingPoint = currentPos;
            var tryFunc = g.ShortestPathsDijkstra(e => 1, currentPos);
            if (!tryFunc(target, out var path))
            {
                throw new Exception($"Unable to find path between {currentPos} and {target}.");
            }

            foreach(var edge in path)
            {
                var direction = edge.Source.GetDirectionTo(edge.Target);
                if (Move(ref currentPos, direction, vm, because) == Status.Wall)
                {
                    throw new Exception("Simulation is wrong somewhere.");
                }
            }
        }

        private Status Move(ref Point current, Direction dir, IntCode vm, string because)
        {
            vm.ProvideInput((long)dir);
            var status = (Status)vm.RunToNextInputOrFinishedCollectOutput()[0];

            var startingPoint = current;
            if (status != Status.Wall)
            {
                current = current.GetLocation(dir);
            }

            var log = $"Moved {dir} from {startingPoint}: {status}. Ended up at {current}, because {because}.";
            Log(log);
            return status;
        }

    }

    struct Point : IComparable<Point>, IEquatable<Point>
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(Point other)
        {
            // Sort them by the x value first, and then by the y value.
            var value = X.CompareTo(other.X);
            if (value == 0)
            {
                value = Y.CompareTo(other.Y);
            }

            return value;
        }

        public bool Equals(Point other)
        {
            return (X == other.X) && (Y == other.Y);
        }

        public static bool operator == (Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point p) return this.Equals(p);
            return false;
                
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    enum Direction
    {
        North = 1,
        South = 2,
        West = 3,
        East = 4,
    }

    enum Status
    {
        Wall = 0,
        Empty = 1,
        System = 2,
    }

    static class Ext
    {
        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                case Direction.East:
                    return Direction.West;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction");
            }
        }

        public static Point GetLocation(this Point point, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(point.X, point.Y - 1);
                case Direction.South:
                    return new Point(point.X, point.Y + 1);
                case Direction.West:
                    return new Point(point.X - 1, point.Y);
                case Direction.East:
                    return new Point(point.X + 1, point.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction");
            }
        }

        public static Direction GetDirectionTo(this Point point, Point adjacentPoint)
        {
            if (point.X == adjacentPoint.X && point.Y == adjacentPoint.Y + 1) return Direction.North;
            if (point.X == adjacentPoint.X && point.Y == adjacentPoint.Y - 1) return Direction.South;
            if (point.X == adjacentPoint.X + 1 && point.Y == adjacentPoint.Y) return Direction.West;
            if (point.X == adjacentPoint.X - 1 && point.Y == adjacentPoint.Y) return Direction.East;
            throw new ArgumentOutOfRangeException($"The two points {point} and {adjacentPoint} are not adjacent to each other.");
        }
    }

}