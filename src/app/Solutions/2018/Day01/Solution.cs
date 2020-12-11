using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2018.Day01 {

    class Solution : ISolver {

        public string GetName() => "Chronal Calibration";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var result = input.Lines()
                .Select(int.Parse)
                .Aggregate(0, (s, v) => s + v);

            return result;
        }

        object PartTwo(string input) {
            var changes = input.Lines()
                .Select(int.Parse).ToArray();
            var seenFrequencies = new HashSet<int>();
            seenFrequencies.Add(0);
            var current = 0;
            while(true)
            {
                foreach(var change in changes)
                {
                    current += change;
                    if (seenFrequencies.Contains(current))
                    {
                        return current;
                    }

                    seenFrequencies.Add(current);
                }
            }

            throw new Exception("No duplicate frequency found.");
        }
    }
}