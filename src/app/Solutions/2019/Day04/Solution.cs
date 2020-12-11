using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day04 {

    class Solution : ISolver {

        public string GetName() => "Secure Container";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var range = input.Split('-').Select(int.Parse).ToArray();
            var validPasswords = 0;
            for (var i = range[0]; i <= range[1]; i++)
            {
                if (AlwaysIncrease(i) && TwoAdjacent(i))
                {
                    validPasswords++;
                }
            }

            return validPasswords;
        }

        bool TwoAdjacent(int v)
        {
            return v.ToString().GroupConsecutive().Count() != 6;
        }

        bool AlwaysIncrease(int v)
        {
            var t = v.ToString();
            var current = t[0];
            for (var i = 1; i < t.Length; i++)
            {
                if (t[i] < current)
                {
                    return false;
                }

                current = t[i];
            }

            return true;
        }

        bool HasTwoAdjacent(int v)
        {
            return v.ToString().GroupConsecutive().Any(g => g.Elements.Length == 2);
        }

        object PartTwo(string input) {
            var range = input.Split('-').Select(int.Parse).ToArray();
            var validPasswords = 0;
            for (var i = range[0]; i <= range[1]; i++)
            {
                if (AlwaysIncrease(i) && TwoAdjacent(i) && HasTwoAdjacent(i))
                {
                    validPasswords++;
                }
            }

            return validPasswords;
        }
    }
}