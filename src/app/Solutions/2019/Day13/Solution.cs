using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day13 {

    class Solution : ISolver {

        public string GetName() => "Care Package";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var vm = new IntCode(input);
            vm.RunToNextInputOrFinished();
            var i = 0;
            var items = new Dictionary<Point, int>();
            while (i < vm.AllOutputs.Count)
            {
                var x = (int)vm.AllOutputs[i++];
                var y = (int)vm.AllOutputs[i++];
                var type = (int)vm.AllOutputs[i++];
                items[new Point(x, y)] = type;
            }

            return items.Count(kvp => kvp.Value == 2);
        }

        object PartTwo(string input) {
            var vm = new IntCode(input);
            vm[0] = 2;

            var score = 0L;
            var scoreLocation = new Point(-1, 0);
            while (vm.State != ProgramState.Finished)
            {
                vm.RunToNextInputOrFinished();
                var i = 0;
                var items = new Dictionary<Point, int>();
                var ballLocation = new Point(0, 0);
                var paddleLocation = new Point(0, 0);
                while (i < vm.AllOutputs.Count)
                {
                    var x = (int) vm.AllOutputs[i++];
                    var y = (int) vm.AllOutputs[i++];
                    var type = (int) vm.AllOutputs[i++];
                    var point = new Point(x, y);

                    if (type == 3)
                    {
                        paddleLocation = point;
                    }
                    else if (type == 4)
                    {
                        ballLocation = point;
                    }

                    items[point] = type;
                }

                // Beat the game by keeping the paddle (#3) under the ball (#4);
                if (ballLocation.X == paddleLocation.X)
                {
                    vm.ProvideInput(0);
                }
                else if (ballLocation.X < paddleLocation.X)
                {
                    vm.ProvideInput(-1);
                }
                else
                {
                    vm.ProvideInput(1);
                }

                if (items.ContainsKey(scoreLocation))
                {
                    score = items[scoreLocation];
                }

                vm.AllOutputs.Clear();
            }

            return score;
        }
    }
}