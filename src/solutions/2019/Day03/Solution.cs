using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day03 {

    class Solution : ISolver {

        public string GetName() => "Crossed Wires";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var wires = input.Lines().Select(l => ParsePointsBetween(l).Select(t => t.point)).ToArray();
            var intersection = wires[0].Intersect(wires[1]).ToArray();
            var points = intersection.OrderBy(p => p.ManhattanDistance).Take(2).ToArray();
            return points[1].ManhattanDistance;
        }

        object PartTwo(string input) {
            var wires = input.Lines().Select(l => ParsePointsBetween(l).ToDictionaryIgnoreDuplicates(t => t.point, t => t.steps)).ToArray();
            var intersections = wires[0].Keys.Intersect(wires[1].Keys).ToArray();
            var closestPoints = intersections.OrderBy(i => wires[0][i] + wires[1][i]).Take(2).ToArray();
            return wires[0][closestPoints[1]] + wires[1][closestPoints[1]];
        }

        IEnumerable<(Point point, int steps)> ParsePointsBetween(string text)
        {
            var current = new Point(0, 0);
            var stepsTaken = 0;
            yield return (current, stepsTaken);
            var steps = text.Split(',');
            foreach(var step in steps)
            {
                var distance = int.Parse(step.Substring(1));
                var deltaX = 0;
                var deltaY = 0;
                switch(step[0])
                {
                    case 'U' :
                        deltaY = 1;
                        break;
                    case 'D' :
                        deltaY = -1;
                        break;
                    case 'L' :
                        deltaX = -1;
                        break;
                    case 'R' :
                        deltaX = 1;
                        break;
                    default:
                        throw new NotSupportedException("Unknown direction.");
                }

                for (var p = 1; p <= distance; p++)
                {
                    current = new Point(current.X + deltaX, current.Y + deltaY);
                    yield return (current, ++stepsTaken);
                }
            }
        }

        [DebuggerDisplay("{X}, {Y}")]
        struct Point
        {
            public int X {get;}
            public int Y {get;}

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int ManhattanDistance => Math.Abs(X) + Math.Abs(Y);
        }
    }
}