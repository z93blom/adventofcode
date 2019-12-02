using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day10 {

    class Solution : ISolver {

        public string GetName() => "Elves Look, Elves Say";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var text = input;
            for (var count = 0; count < 40; count++)
            {
                text = string.Join("", text
                    .GroupConsecutive()
                    .Select(g => g.Count().ToString() + g.Key));
            }

            return text.Length;
        }

        object PartTwo(string input) {
            var text = input;
            for (var count = 0; count < 50; count++)
            {
                text = string.Join("", text
                    .GroupConsecutive()
                    .Select(g => g.Count().ToString() + g.Key));
            }

            return text.Length;
        }
    }
}