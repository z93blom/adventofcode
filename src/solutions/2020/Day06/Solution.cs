using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day06 {

    class Solution : ISolver {

        public string GetName() => "Custom Customs";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var groups = input.Split(new [] {"\n\n"}, StringSplitOptions.None)
                .Select(s => 
                {
                    var hs = new HashSet<char>();
                    foreach(var c in s.Where(c => char.IsLetter(c))) {
                        hs.Add(c);
                    }
                    return hs;
                });

            return groups.Sum(hs => hs.Count);
        }

        object PartTwo(string input) {
            var chars = Enumerable.Range(0, 26).Select(i => (char)('a' + i)).ToArray();
            var groups = input.Split(new [] {"\n\n"}, StringSplitOptions.None)
                .Select(s => 
                {
                    var hs = new HashSet<char>(chars);
                    foreach(var q in s.Split('\n'))
                    {
                        foreach(var c in hs.ToArray().Where(c => !q.Contains(c)))
                        {
                            hs.Remove(c);
                        }
                    }
                    return hs;
                });

            return groups.Sum(hs => hs.Count);
        }
    }
}