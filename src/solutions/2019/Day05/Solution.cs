using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Y2019;

namespace AdventOfCode.Y2019.Day05 {

    class Solution : ISolver {

        public string GetName() => "Sunny with a Chance of Asteroids";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var output = new List<int>();
            var computer = new IntCode(input);
            computer.ProvideInput(1);
            computer.Run();
            if (computer.AllOutputs.Take(output.Count - 1).Any(v => v != 0))
            {
                throw new Exception("Something is wrong with the computer.");
            }

            return output[output.Count - 1];
        }

        object PartTwo(string input) {
            var computer = new IntCode(input);
            computer.ProvideInput(5);
            computer.Run();
            if (computer.AllOutputs.Count != 1)
            {
                throw new Exception("Computer doesn't work as expected.");
            }
            return computer.AllOutputs[computer.AllOutputs.Count - 1];
        }
    }
}