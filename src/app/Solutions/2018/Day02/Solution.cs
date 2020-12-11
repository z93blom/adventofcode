using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2018.Day02 {

    class Solution : ISolver {

        public string GetName() => "Inventory Management System";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var lines = input.Lines().ToArray();
            var twos = lines
                .Where(l => l.GroupBy(c => c).Where(g => g.Count() == 2).Any())
                .Count();
            var threes = lines
                .Where(l => l.GroupBy(c => c).Where(g => g.Count() == 3).Any())
                .Count();
            
            return twos * threes;
        }

        object PartTwo(string input) {
            var lines = input.Lines().ToArray();
            var differences = new List<int>();
            for(var ii = 0; ii < lines.Length; ii++)
            {
                var first = lines[ii];
                for(var jj = ii; jj < lines.Length; jj++)
                {
                    var second = lines[jj];
                    if (first.Length == second.Length)
                    {
                        differences.Clear();
                        for(var charIndex = 0; charIndex < first.Length; charIndex++)
                        {
                            if (first[charIndex] != second[charIndex])
                            {
                                differences.Add(charIndex);
                            }
                        }

                        if (differences.Count == 1)
                        {
                            return first.Slice(0, differences[0]) + first.Substring(differences[0] + 1);
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}