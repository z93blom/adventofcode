using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day09 {

    class Solution : ISolver {

        public string GetName() => "Encoding Error";

        public IEnumerable<object> Solve(string input) {
            var part1 = PartOne(input);
            yield return part1;
            yield return PartTwo(input, part1);
        }

        const int CipherLength = 25;

        long PartOne(string input) {
            var numbers = input.Lines()
                .Select(long.Parse)
                .ToArray();
            var lookup = new Dictionary<long, HashSet<int>>();
            for(var index = 0; index < numbers.Length; index++)
            {
                if (!lookup.ContainsKey(numbers[index]))
                {
                    lookup[numbers[index]] = new HashSet<int>();
                }
                lookup[numbers[index]].Add(index);
            }

            var start = 0;
            while(true)
            {
                var numberToLookFor = numbers[start+CipherLength];
                var pairFound = false;
                for(var i = start; i < start+CipherLength - 1; i++)
                {
                    var x = numberToLookFor - numbers[i];
                    if (x < 0)
                        continue;
                    if (lookup.ContainsKey(x))
                    {
                        var set = lookup[x];
                        foreach(var v in set)
                        {
                            if (v != x && v > i && v < start + CipherLength)
                            {
                                pairFound = true;
                                break;
                            }
                        }
                    }

                    if (pairFound)
                        break;
                }

                if (!pairFound)
                {
                    return numberToLookFor;
                }

                start++;
            }
        }

        object PartTwo(string input, long part1) {
            var numbers = input.Lines()
                .Select(long.Parse)
                .ToArray();
            var start = 0;
            while(true)
            {
                var sum = numbers[start];
                var end = start;
                while(sum < part1)
                {
                    end++;
                    sum += numbers[end];
                }

                if (sum == part1)
                {
                    // Add together the smallest and the largest within this range
                    // start .. end
                    var range = numbers.Skip(start)
                        .Take(end - start + 1)
                        .ToArray();
                    var summ = range.Sum();
                    var min = range.Min();
                    var max = range.Max();
                    return min + max;
                }

                start++;
            }
        }
    }
}