using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day02 {

    class Solution : ISolver {

        public string GetName() => "Password Philosophy";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var lines = input.Lines()
                .Match(@"(\d+)\-(\d+) ([a-zA-Z])\: ([a-zA-Z]+)")
                .Select(m => new 
                {
                    Min = int.Parse(m.Groups[1].Value),
                    Max = int.Parse(m.Groups[2].Value),
                    Char = m.Groups[3].Value[0],
                    Password = m.Groups[4].Value
                }).ToArray();

            var correct = 0;
            foreach(var line in lines)
            {
                var count = line.Password.Count(c => c == line.Char);
                if (line.Min <= count && line.Max >= count)
                    correct++;
            }

            return correct;
        }

        object PartTwo(string input) {
            var lines = input.Lines()
                .Match(@"(\d+)\-(\d+) ([a-zA-Z])\: ([a-zA-Z]+)")
                .Select(m => new 
                {
                    Min = int.Parse(m.Groups[1].Value),
                    Max = int.Parse(m.Groups[2].Value),
                    Char = m.Groups[3].Value[0],
                    Password = m.Groups[4].Value
                }).ToArray();
                        
            var correct = 0;
            foreach(var line in lines)
            {
                if (line.Password.Length < line.Min ||
                    line.Password.Length < line.Max)
                    continue;
                
                var count = line.Password[line.Min - 1] == line.Char ? 1 : 0;
                if (line.Password[line.Max - 1] == line.Char)
                    count++;
                if (count == 1)
                    correct++;
            }

            return correct;
        }
    }
}