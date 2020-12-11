using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day10 {

    class Solution : ISolver {

        public string GetName() => "Adapter Array";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var values = input.Lines()
                .Select(int.Parse)
                .OrderBy(v => v)
                .ToArray();
            var oneSteps = 0;
            var threeSteps = 1; // The last
            var current = 0;
            for(int index = 0; index < values.Length; index++)
            {
                oneSteps += values[index] - current == 1 ? 1 : 0;
                threeSteps += values[index] - current == 3 ? 1 : 0;
                current = values[index];
            }
            return oneSteps * threeSteps;
        }

        object PartTwo(string input) {
            var values = input.Lines()
                .Select(int.Parse)
                .OrderBy(v => v)
                .ToArray();
            var result = values
                .Select((v, i) => i == 0 ? v - 0 : v - values[i-1]) // The difference between each position
                .GroupConsecutive()
                .Where(g => g.Key == 1 && g.Count() > 1) // Only select the ones where there are consecutive 1:s.
                .Select(g => g.Count() + 1) // There is actually an extra 1 in the group.
                .Select(v => v switch { 3 => 2, 4 => 4, 5 => 7, _ => 1})
                .Aggregate(1L, (a, v) => a*v);
            return result;
        }
    }
}