using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day09 {

    class Solution : ISolver {

        public string GetName() => "Sensor Boost";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var computer = new IntCode(input);
            computer.ProvideInput(1);
            computer.Run();

            Console.WriteLine($"Outputs: {string.Join(", ", computer.AllOutputs)}");
            return computer.ReadOutput();
        }

        object PartTwo(string input) {
            var computer = new IntCode(input);
            computer.ProvideInput(2);
            computer.Run();

            Console.WriteLine($"Outputs: {string.Join(", ", computer.AllOutputs)}");
            return computer.ReadOutput();
        }
    }
}