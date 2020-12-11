using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day01 {

    class Solution : ISolver {

        public string GetName() => "The Tyranny of the Rocket Equation";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            return input.Lines()
                .Select(int.Parse)
                .Select(v => v / 3 - 2)
                .Sum();
        }

        object PartTwo(string input)
        {
            var totalFuel = input.Lines()
                .Select(int.Parse)
                .Select(v => v / 3 - 2).ToList();

            var fuelNeeded = totalFuel.Sum();

            var additional = new Queue<int>(totalFuel);
            while (additional.Count > 0)
            {
                var f = additional.Dequeue();
                var extra = Math.Max(0, f / 3 - 2);
                if (extra > 0)
                {
                    fuelNeeded += extra;
                    additional.Enqueue(extra);
                }
            }

            return fuelNeeded;
        }
    }
}