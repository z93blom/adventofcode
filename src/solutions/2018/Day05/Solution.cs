using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2018.Day05 {

    class Solution : ISolver {

        public string GetName() => "Alchemical Reduction";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            // Scan the input for pairs of letters
            // Note: they can be either:
            return React(input).Length;
        }

        private static string React(string input)
        {
            var result = input;
            var replaced = true;
            while (replaced)
            {
                replaced = false;
                for (var i = 0; i < result.Length - 1; i++)
                {
                    var current = result[i];
                    var next = result[i + 1];
                    if (current != next &&
                        char.ToUpperInvariant(current) == char.ToUpperInvariant(next))
                    {
                        result = result.Substring(0, i) + result.Substring(i + 2);
                        replaced = true;
                        break;
                    }
                }
            }

            return result;
        }

        object PartTwo(string input) {
            var lowest = int.MaxValue;
            for(var i = (int)'A'; i <= (int)'Z'; i++)
            {
                var text = input.Replace(((char)i).ToString(), string.Empty);
                text = text.Replace(char.ToLowerInvariant((char)i).ToString(), string.Empty);
                var result = React(text);
                lowest = Math.Min(lowest, result.Length);
            }
            return lowest;
        }
    }
}