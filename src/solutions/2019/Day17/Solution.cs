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
            var startingDirection = Direction.North;
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

        char GetTurn(Direction currentDirection, Direction newDirection)
        {
            if (currentDirection == Direction.North && newDirection == Direction.East) return 'R';
            if (currentDirection == Direction.North && newDirection == Direction.West) return 'L';
            if (currentDirection == Direction.South && newDirection == Direction.East) return 'L';
            if (currentDirection == Direction.South && newDirection == Direction.West) return 'R';
            if (currentDirection == Direction.East && newDirection == Direction.South) return 'R';
            if (currentDirection == Direction.East && newDirection == Direction.North) return 'L';
            if (currentDirection == Direction.West && newDirection == Direction.South) return 'L';
            if (currentDirection == Direction.West && newDirection == Direction.North) return 'R';

            throw new Exception("Unable to turn twice.");
        }

        object PartTwo(string input) {
            var vm = new IntCode(input);
            var result = new string(vm.RunToNextInputOrFinishedCollectOutput()
                .Select(v => (char)v).ToArray());

            var map = new Dictionary<Point, char>();
            var y = 0;
            var startingPoint = new Point(0, 0);
            var startingDirection = Direction.North;
            foreach (var line in result.Lines())
            {
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

            var currentPoint = startingPoint;
            var currentDirection = startingDirection;
            var previousPoint = new Point(-1, -1);
            var sb = new StringBuilder();
            while (TryGetNewDirection(currentPoint, previousPoint, map, out var direction))
            {
                sb.Append(",");
                sb.Append(GetTurn(currentDirection, direction));
                currentDirection = direction;
                previousPoint = currentPoint;
                var movements = 1;
                currentPoint = currentPoint.GetPoint(direction);

                // Go in the direction until we can't go any further.
                var nextPoint = currentPoint.GetPoint(direction);
                while (map.ContainsKey(nextPoint) && map[nextPoint] == '#')
                {
                    movements++;
                    previousPoint = currentPoint;
                    currentPoint = nextPoint;
                    nextPoint = currentPoint.GetPoint(direction);
                }

                sb.Append(",");
                sb.Append(movements);
            }

            var text = sb.ToString();

            //R,6,L,10,R,8,R,8,R,12,L,8,L,10,R,6,L,10,R,8,R,8,R,12,L,10,R,6,L,10,R,12,L,8,L,10,R,12,L,10,R,6,L,10,R,6,L,10,R,8,R,8,R,12,L,8,L,10,R,6,L,10,R,8,R,8,R,12,L,10,R,6,L,10
            // R,6,L,10,R,8,R,8 = A
            // R,12,L,8,L,10 = B
            // R,12,L,10,R,6,L,10 = C
            // A,B,A,C,B,C,A,B,A,C = Main Movement.

            var mainMovement = Encoding.ASCII.GetBytes("A,B,A,C,B,C,A,B,A,C\n");
            var functionA = Encoding.ASCII.GetBytes("R,6,L,10,R,8,R,8\n");
            var functionB = Encoding.ASCII.GetBytes("R,12,L,8,L,10\n");
            var functionC = Encoding.ASCII.GetBytes("R,12,L,10,R,6,L,10\n");
            var video = Encoding.ASCII.GetBytes("n\n");
            vm = new IntCode(input) {[0] = 2};
            foreach (var v in mainMovement.Concat(functionA).Concat(functionB).Concat(functionC).Concat(video))
            {
                vm.ProvideInput(v);
            }

            var finalResult = vm.RunToNextInputOrFinishedCollectOutput();
            return finalResult[finalResult.Length - 1];
        }


    }
}