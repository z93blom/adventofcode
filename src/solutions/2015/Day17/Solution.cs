using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day17 {

    class Solution : ISolver {

        public string GetName() => "No Such Thing as Too Much";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var containers = input.Lines().Select(int.Parse).ToArray();
            var allPermutations = Enumerable.Range(1, containers.Length).SelectMany(i => containers.Permutations(i));
            var permutationsEqualTo150 = allPermutations.Count(p => p.Sum() == 150);
            return permutationsEqualTo150;
        }

        object PartTwo(string input) {
            var containers = input.Lines().Select(int.Parse).ToArray();
            var allPermutations = Enumerable.Range(1, containers.Length).SelectMany(i => containers.Permutations(i));
            var permutationsEqualTo150 = allPermutations.Where(p => p.Sum() == 150).Select(p => p.Count()).OrderBy(c => c).ToArray();
            var permutationsWithMinimumNumberOfContainers =
                permutationsEqualTo150.TakeWhile(c => permutationsEqualTo150[0] == c).Count();

            return permutationsWithMinimumNumberOfContainers;

        }
    }
}