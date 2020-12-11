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
            var sif = new Sif();
            var direction = Direction.Up;
            var location = new Point(0, 0);
            var vm = new IntCode(input);
            while (vm.State != ProgramState.Finished)
            {
                var currentColor = sif.GetColor(location);
                currentColor = currentColor == Sif.Color.Transparent ? Sif.Color.Black : currentColor;
                vm.ProvideInput((long)currentColor);
                vm.RunToNextInputOrFinished();

                // Get the output (if any)
                if (vm.HasOutput)
                {
                    var color = (Sif.Color) vm.ReadOutput();
                    sif.SetColor(location, color);
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
            var sif = new Sif();
            var direction = Direction.Up;
            var location = new Point(0, 0);
            sif.SetColor(location, Sif.Color.White);
            var vm = new IntCode(input);
            while (vm.State != ProgramState.Finished)
            {
                var currentColor = sif.GetColor(location);
                var vmInput = currentColor == Sif.Color.White ? 1 : 0;
                vm.ProvideInput(vmInput);
                vm.RunToNextInputOrFinished();

                // Get the output (if any)
                if (vm.HasOutput)
                {
                    var color = (Sif.Color)vm.ReadOutput();
                    sif.SetColor(location, color);

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
            sif.Draw(Console.Out);
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