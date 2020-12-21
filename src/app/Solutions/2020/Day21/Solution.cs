using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day21 {

    class Solution : ISolver {

        public string GetName() => "Allergen Assessment";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var lines = input.Lines()
                .Match(@"(.*) \(contains (.*)\)")
                .Select(m => new { Ingredients =  m.Groups[1].Value.Split(" ").ToArray(), Allergens = m.Groups[2].Value.Split(" ").Select(s => s.TrimEnd(',')).ToArray() })
                .ToArray();

            var allergensMap = new Dictionary<string, HashSet<string>>();
            foreach(var l in lines)
            {
                foreach(var a in l.Allergens)
                {
                    if (!allergensMap.ContainsKey(a))
                    {
                        allergensMap[a] = l.Ingredients.ToHashSet();
                    }
                    else
                    {
                        var intersect = allergensMap[a].Intersect(l.Ingredients).ToHashSet();
                        if (intersect.Count == 0)
                            throw new Exception("Oops?");
                        allergensMap[a] = intersect;
                    }
                }
            }

            var allAllergens = allergensMap.Values.SelectMany(hs => hs).ToHashSet();
            var i = lines.Sum(l => l.Ingredients.Count(s => !allAllergens.Contains(s)));
            
            return i;
        }

        object PartTwo(string input) {
            var lines = input.Lines()
                .Match(@"(.*) \(contains (.*)\)")
                .Select(m => new { Ingredients =  m.Groups[1].Value.Split(" ").ToArray(), Allergens = m.Groups[2].Value.Split(" ").Select(s => s.TrimEnd(',')).ToArray() })
                .ToArray();

            var allergensMap = new Dictionary<string, HashSet<string>>();
            foreach(var l in lines)
            {
                foreach(var a in l.Allergens)
                {
                    if (!allergensMap.ContainsKey(a))
                    {
                        allergensMap[a] = l.Ingredients.ToHashSet();
                    }
                    else
                    {
                        var intersect = allergensMap[a].Intersect(l.Ingredients).ToHashSet();
                        if (intersect.Count == 0)
                            throw new Exception("Oops?");
                        allergensMap[a] = intersect;
                    }
                }
            }

            var allergens = new Dictionary<string, string>();
            while(allergensMap.Count > 0)
            {
                foreach(var kvp in allergensMap.Where(aa => aa.Value.Count == 1).ToArray())
                {
                    var allergen = kvp.Key;
                    var ingredient = allergensMap[kvp.Key].First();
                    allergens[allergen] = ingredient;
                    allergensMap.Remove(kvp.Key);
                    foreach(var kvp2 in allergensMap.Where(aa => aa.Value.Contains(ingredient)))
                        allergensMap[kvp2.Key].Remove(ingredient);
                }
            }

            return string.Join(",", allergens.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));
        }
    }
}