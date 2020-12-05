using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day05 {

    class Solution : ISolver {

        public string GetName() => "Binary Boarding";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var rowCol = input.Lines()
                             .Select(s => new string(s.Select(c => c switch { 'F'=>'0', 'B'=>'1', 'L'=>'0', 'R'=>'1', _ => '\0'}).ToArray()))
                             .Select(s => new 
                             {
                                 Input = s, 
                                 Row = Convert.ToInt32(s.Substring(0, 7), 2), 
                                 Col = Convert.ToInt32(s.Substring(7), 2),
                                 Id = Convert.ToInt32(s, 2)
                            })
                            .ToArray();
            var max = rowCol.Max(rc => rc.Row * 8 + rc.Col);
            return max;
        }

        object PartTwo(string input) {
            var rowCol = input.Lines()
                .Select(s => s.Aggregate(0, (v,c) => v * 2 + (c == 'F' || c == 'L' ? 0:1)))
                .OrderByDescending(id => id)
                .ToArray();

            for(var index = 1; index < rowCol.Length; index++)
            {
                if (rowCol[index-1] != rowCol[index] + 1)
                    return rowCol[index] + 1;
            }
            return 0;
        }
    }
}