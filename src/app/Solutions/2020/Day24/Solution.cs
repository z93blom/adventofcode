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

        public class Exhibit
        {
            public HashSet<Hex> Blacks;

            public int Day;
            public Exhibit(IEnumerable<Hex> initial)
            {
                Blacks = new HashSet<Hex>(initial);
                Day = 0;
            }

            public void Next()
            {
                var newBlacks = new HashSet<Hex>();
                foreach(var hex in Relevant().Distinct())
                {
                    if (Blacks.Contains(hex))
                    {
                        // Any black tile with zero or more than 2 black tiles immediately adjacent to it is flipped to white.
                        var blackNeighbors = Neighbors(hex).Count(h => Blacks.Contains(h));
                        if (blackNeighbors == 1 || blackNeighbors == 2)
                            newBlacks.Add(hex);
                    }
                    else
                    {
                        // Any white tile with exactly 2 black tiles immediately adjacent to it is flipped to black.
                        var blackNeighbors = Neighbors(hex).Count(h => Blacks.Contains(h));
                        if (blackNeighbors == 2)
                            newBlacks.Add(hex);
                    }
                }

                Blacks = newBlacks;
                Day += 1;
            }

            private IEnumerable<Hex> Relevant()
            {
                foreach (var hex in Blacks)
                {
                    yield return hex;
                    foreach(var h in Neighbors(hex))
                    {
                        yield return h;
                    }
                }
            }

            private IEnumerable<Hex> Neighbors(Hex hex)
            {
                for(var d = 0; d < 6; d++)
                {
                    yield return hex.Neighbor(d);
                }
            }

        }

        object PartTwo(string input) {
             var origo = new Hex(0, 0, 0);
            var locations = input.Lines()
                .Select(s => ToHexDirections(s).Aggregate(origo, (a, d) => a.Neighbor(d)))
                .ToArray();
            
            var blacks = locations.GroupBy(h => h).Where(g => g.Count() % 2 == 1).Select(g => g.Key);
            var exhibit = new Exhibit(blacks);

            for(int i = 0; i < 100; i++)
            {
                exhibit.Next();
                Console.Out.WriteLine($"Day {exhibit.Day}: {exhibit.Blacks.Count}");
            }
            
            return exhibit.Blacks.Count;
       }
    }
}