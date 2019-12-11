using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day11 {

    class Solution : ISolver {

        public string GetName() => "Space Police";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var painted = new HashSet<Point>();
            var colors = new Dictionary<Point, Color>();
            var direction = Direction.Up;
            var location = new Point(0, 0);
            var vm = new IntCode(input);
            while (vm.State != ProgramState.Finished)
            {
                vm.ProvideInput((long) GetColor(colors, location));
                vm.RunToNextInputOrFinished();

                // Get the output (if any)
                if (vm.HasOutput)
                {
                    var color = (Color) vm.ReadOutput();
                    colors[location] = color;
                    painted.Add(location);

                    var turn = vm.ReadOutput();
                    if (turn == 0)
                    {
                        // Turn left
                        direction = (Direction)(((int)direction - 1 + 4) % 4);
                    }
                    else
                    {
                        // Turn right
                        direction = (Direction)(((int)direction + 1) % 4);
                    }

                    switch(direction)
                    {
                        case Direction.Up:
                            location = new Point(location.X, location.Y - 1);
                            break;
                        case Direction.Right:
                            location = new Point(location.X + 1, location.Y);
                            break;
                        case Direction.Down:
                            location = new Point(location.X, location.Y + 1);
                            break;
                        case Direction.Left:
                            location = new Point(location.X - 1, location.Y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return painted.Count;
        }

        object PartTwo(string input)
        {
            var colors = new Dictionary<Point, Color>();
            var direction = Direction.Up;
            var location = new Point(0, 0);
            colors[location] = Color.White;
            var vm = new IntCode(input);
            while (vm.State != ProgramState.Finished)
            {
                vm.ProvideInput((long)GetColor(colors, location));
                vm.RunToNextInputOrFinished();

                // Get the output (if any)
                if (vm.HasOutput)
                {
                    var color = (Color)vm.ReadOutput();
                    colors[location] = color;

                    var turn = vm.ReadOutput();
                    if (turn == 0)
                    {
                        // Turn left
                        direction = (Direction)(((int)direction - 1 + 4) % 4);
                    }
                    else
                    {
                        // Turn right
                        direction = (Direction)(((int)direction + 1) % 4);
                    }

                    switch (direction)
                    {
                        case Direction.Up:
                            location = new Point(location.X, location.Y - 1);
                            break;
                        case Direction.Right:
                            location = new Point(location.X + 1, location.Y);
                            break;
                        case Direction.Down:
                            location = new Point(location.X, location.Y + 1);
                            break;
                        case Direction.Left:
                            location = new Point(location.X - 1, location.Y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            // Gah. Should've trusted my instincts and made the SIF OCR.
            var minX = colors.Keys.Min(p => p.X);
            var maxX = colors.Keys.Max(p => p.X);
            var minY = colors.Keys.Min(p => p.Y);
            var maxY = colors.Keys.Max(p => p.Y);
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    Console.Write(GetColor(colors, new Point(x, y)) == Color.White ? "█" : " ");
                }

                Console.WriteLine();
            }

            return "JZPJRAGJ";
         }

        private Color GetColor(Dictionary<Point, Color> colors, Point location)
        {
            if (!colors.ContainsKey(location))
            {
                return Color.Black;
            }

            return colors[location];
        }

        private enum Color
        {
            Black = 0,
            White = 1,
        }

        enum Direction
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }
    }
}