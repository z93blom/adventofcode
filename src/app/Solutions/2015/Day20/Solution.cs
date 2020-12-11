using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day20 {

    class Solution : ISolver {

        public string GetName() => "Infinite Elves and Infinite Houses";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var minimumExpectedPresents = int.Parse(input);

            // Lets just count all presents for all houses.
            var houses = new int[minimumExpectedPresents];
            for (var elf = 2; elf < houses.Length; ++elf)
            {
                for (var house = elf - 1; house < houses.Length; house += elf)
                {
                    houses[house] += elf * 10;
                }

                if (houses[elf - 1] >= minimumExpectedPresents)
                {
                    return elf;
                }
            }

            return 0;
        }

        object PartTwo(string input) {
            var minimumExpectedPresents = int.Parse(input);

            // Lets just count all presents for all houses.
            var houses = new int[minimumExpectedPresents];
            for (var elf = 2; elf < houses.Length; ++elf)
            {
                var housesDelivered = 0;
                for (var house = elf - 1; house < houses.Length; house += elf)
                {
                    houses[house] += elf * 11;
                    if (++housesDelivered >= 50)
                    {
                        break;
                    }
                }

                if (houses[elf - 1] >= minimumExpectedPresents)
                {
                    return elf;
                }
            }

            return 0;
        }
    }
}