using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day16 {

    class Solution : ISolver {

        public string GetName() => "Aunt Sue";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            return MatchingSueIndex(input, true);
        }

        object PartTwo(string input) {
            return MatchingSueIndex(input, false);
        }



        private int MatchingSueIndex(string input, bool useExact)
        {
            bool IsMatch(int v, string op, int expected)
            {
                if (useExact || op == "=")
                {
                    return v == expected;
                }
                else if (op == ">")
                {
                    return v > expected;
                }
                else if (op == "<")
                {
                    return v < expected;
                }
                else
                {
                    throw new Exception("Unknown operator");
                }
            }

            var matchingText =
                "children: = 3\r\ncats: > 7\r\nsamoyeds: = 2\r\npomeranians: = 3\r\nakitas: = 0\r\nvizslas: = 0\r\ngoldfish: < 5\r\ntrees: > 3\r\ncars: = 2\r\nperfumes: = 1";
            var matchingParameters = matchingText
                .Lines()
                .Match(@"(\w+): (.) (\d+)")
                .Groups()
                .Select(g => new
                {
                    text = g[0],
                    op = g[1],
                    val = int.Parse(g[2])
                });

            var sues = Parse(input);
            var matchingSueIndex =
                sues.FindIndex(s => matchingParameters.All(p => !s.ContainsKey(p.text) || IsMatch(s[p.text], p.op, p.val)));

            return matchingSueIndex + 1;
        }

        public List<Dictionary<string, int>> Parse(string input)
        {
            var sues = input
                .Lines()
                .Match(@"Sue \d+: (.*)")
                .Groups()
                .Select(g => g[0])
                .Matches(@"((\w+): (\d+),?\s*)")
                .Select(ms => ms.Groups().ToList())     // All the matching items for each and every Sue. Each row is a grouping with three elements (the full catch, the key and the value).
                .Select(r => r.Select(g => g.Skip(1).ToArray()).ToDictionary(a => a[0], a => int.Parse(a[1])))
                .ToList();
            return sues;
        }
    }
}