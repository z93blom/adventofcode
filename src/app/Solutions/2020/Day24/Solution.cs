using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day24 {

    class Solution : ISolver {

        public string GetName() => "Lobby Layout";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var origo = new Hex(0, 0, 0);
            var locations = input.Lines()
                .Select(s => ToHexDirections(s).Aggregate(origo, (a, d) => a.Neighbor(d)))
                .ToArray();
            
            var blackCount = locations.GroupBy(h => h).Count(g => g.Count() % 2 == 1);
            
            return blackCount;
        }

        public IEnumerable<int> ToHexDirections(string s)
        {
            var i = 0;
            while(i < s.Length)
            {
                switch(s[i])
                {
                    case 'e' :
                        yield return 0;
                        i += 1;
                        break;
                    case 'n' :
                        if (i+1 < s.Length && s[i+1] == 'e')
                            yield return 1;
                        else if (i+1 < s.Length && s[i+1] == 'w')
                            yield return 2;
                        else    
                            throw new Exception("Unexpected input.");
                        i += 2;
                        break;
                    case 'w' :
                        yield return 3;
                        i += 1;
                        break;
                    case 's' :
                        if (i+1 < s.Length && s[i+1] == 'w')
                            yield return 4;
                        else if (i+1 < s.Length && s[i+1] == 'e')
                            yield return 5;
                        else    
                            throw new Exception("Unexpected input.");
                        i += 2;
                        break;
                }
            }
        }

        object PartTwo(string input) {
            return 0;
        }
    }
}