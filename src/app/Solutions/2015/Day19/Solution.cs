using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day19 {

    class Solution : ISolver {

        public string GetName() => "Medicine for Rudolph";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var replacements = input
                .Lines(StringSplitOptions.None)
                .TakeWhile(l => !string.IsNullOrWhiteSpace(l))
                .Match(@"(.*) => (.*)")
                .Groups()
                .Select(g => new { Input = g[0], Replacement = g[1] })
                .GroupBy(r => r.Input)
                .ToDictionary(group => group.Key, group => group.Select(g => g.Replacement).ToImmutableArray());

            var molecule = input.Lines().Last();
            var possibleMolecules = new HashSet<string>();
            foreach (var kvp in replacements)
            {
                var regex = new Regex(kvp.Key);
                var matches = regex.Matches(molecule);

                foreach (Match match in matches)
                {
                    foreach (var replacement in replacements[kvp.Key])
                    {
                        var result = match.Replace(molecule, replacement);
                        if (!possibleMolecules.Contains(result))
                        {
                            possibleMolecules.Add(result);
                        }
                    }
                }
            }

            return possibleMolecules.Count;
        }

        object PartTwo(string input) {
            return 0;
        }
    }
}