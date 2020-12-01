using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day01 {

    class Solution : ISolver {

        public string GetName() => "Report Repair";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var numbers = input.Split(new [] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                 .Select(int.Parse).ToArray();
            // Brute force
            for(int i = 0; i < numbers.Length; i++)
            {
                for(int j = i + 1; j < numbers.Length; j++)
                {
                    if (numbers[i] + numbers[j] == 2020)
                    {
                        return numbers[i] * numbers[j];
                    }
                }
            }

            return -1;
        }

        object PartTwo(string input) {
            var numbers = input.Split(new [] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                 .Select(int.Parse)
                 .OrderBy(i => i)
                 .ToArray();
            
            // Brute force
            for(int i = 0; i < numbers.Length; i++)
            {
                for(int j = i + 1; j < numbers.Length; j++)
                {
                    for(int k = j + 1; k < numbers.Length; k++)
                    {
                        if (numbers[i] + numbers[j] + numbers[k] == 2020)
                            return numbers[i] * numbers[j] * numbers[k];
                    }

                }
            }

            return -1;
        }
    }
}