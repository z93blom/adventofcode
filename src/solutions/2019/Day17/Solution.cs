using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using AdventOfCode.Y2019.Day15;

namespace AdventOfCode.Y2019.Day17 {

    class Solution : ISolver {

        public string GetName() => "Set and Forget";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var vm = new IntCode(input);
            var result = new string(vm.RunToNextInputOrFinishedCollectOutput()
                .Select(v => (char) v).ToArray());

            var map = new Dictionary<Point, char>();
            var y = 0;
            var startingPoint = new Point(0, 0);
            foreach (var line in result.Lines())
            {
                Console.WriteLine(line);
                var x = 0;
                foreach (var c in line)
                {
                    if (c != '.' && c != '#')
                    {
                        startingPoint = new Point(x, y);
                    }

                    map[new Point(x++, y)] = c;

                }

                y++;
            }

            var crossings = new HashSet<Point>();
            var visited = new HashSet<Point>();
            var currentPoint = startingPoint;
            var previousPoint = new Point(-1, -1);
            visited.Add(currentPoint);
            while (TryGetNewDirection(currentPoint, previousPoint, map, out var direction))
            {
                previousPoint = currentPoint;
                currentPoint = currentPoint.GetPoint(direction);
                if (visited.Contains(currentPoint))
                {
                    crossings.Add(currentPoint);
                }
                else
                {
                    visited.Add(currentPoint);
                }

                // Go in the direction until we can't go any further.
                var nextPoint = currentPoint.GetPoint(direction);
                while (map.ContainsKey(nextPoint) && map[nextPoint] == '#')
                {
                    previousPoint = currentPoint;
                    currentPoint = nextPoint;
                    if (visited.Contains(currentPoint))
                    {
                        crossings.Add(currentPoint);
                    }
                    else
                    {
                        visited.Add(currentPoint);
                    }

                    nextPoint = currentPoint.GetPoint(direction);
                }
            }

            return crossings.Select(p => p.X * p.Y).Sum();
        }

        bool TryGetNewDirection(Point endPoint, Point previousPoint, Dictionary<Point, char> map, out Direction direction)
        {
            foreach (var d in Enum.GetValues(typeof(Direction)).Cast<Direction>())
            {
                var p = endPoint.GetPoint(d);
                if (map.ContainsKey(p) && map[p] == '#' && previousPoint != p)
                {
                    direction = d;
                    return true;
                }
            }

            // End of line.
            direction = Direction.North;
            return false;
        }

        object PartTwo(string input) {
            return 0;
        }
    }
}