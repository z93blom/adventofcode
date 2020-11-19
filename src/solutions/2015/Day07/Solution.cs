using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day07 {

    class Solution : ISolver {

        public string GetName() => "Some Assembly Required";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {

            var circuit = GetCircuit(input);
            var realizedValues = new Dictionary<string, int>();
            var value = GetValue(circuit, realizedValues, "a");
            return value;
        }

        object PartTwo(string input) {
            var circuit = GetCircuit(input);
            var realizedValues = new Dictionary<string, int>();
            var value = GetValue(circuit, realizedValues, "a");
            circuit["b"] = lookup => value;
            realizedValues.Clear();
            value = GetValue(circuit, realizedValues, "a");
            return value;
        }

        private Dictionary<string, Func<Func<string, int>, int>> GetCircuit(string input)
        {
            var circuit = new Dictionary<string, Func<Func<string, int>, int>>();
            foreach (var line in input.Lines())
            {
                ImmutableArray<string> values;
                if (Matches(line, @"(\w+) RSHIFT (\d+) -> (\w+)", out values))
                {
                    var shifts = int.Parse(values[1]);
                    circuit.Add(values.Last(), lookup => lookup(values[0]) >> shifts);
                }
                else if (Matches(line, @"(\w+) LSHIFT (\d+) -> (\w+)", out values))
                {
                    var shifts = int.Parse(values[1]);
                    circuit.Add(values.Last(), lookup => lookup(values[0]) << shifts);
                }
                else if (Matches(line, @"(\w+) OR (\w+) -> (\w+)", out values))
                {
                    circuit.Add(values.Last(), lookup => lookup(values[0]) | lookup(values[1]));
                }
                else if (Matches(line, @"(\w+) AND (\w+) -> (\w+)", out values))
                {
                    circuit.Add(values.Last(), lookup => lookup(values[0]) & lookup(values[1]));
                }
                else if (Matches(line, @"NOT (\w+) -> (\w+)", out values))
                {
                    circuit.Add(values.Last(), lookup => ~lookup(values[0]));
                }
                else if (Matches(line, @"(\w+) -> (\w+)", out values))
                {
                    circuit.Add(values.Last(), lookup => lookup(values[0]));
                }
                else
                {
                    throw new Exception($"Unrecognized line: '{line}'.");
                }
            }

            return circuit;
        }

        private int GetValue(Dictionary<string, Func<Func<string, int>, int>> circuit, Dictionary<string, int> realizedValues, string variableOrValue)
        {
            int result;
            if (realizedValues.ContainsKey(variableOrValue))
            {
                result = realizedValues[variableOrValue];
                realizedValues[variableOrValue] = result;
            }
            else if (!int.TryParse(variableOrValue, out result))
            {
                var lookup = circuit[variableOrValue];
                result = lookup(v => GetValue(circuit, realizedValues, v));
                realizedValues[variableOrValue] = result;
            }

            return result;
        }

        bool Matches(string text, string pattern, out ImmutableArray<string> groupMatches)
        {
            var match = Regex.Match(text, pattern);
            groupMatches = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToImmutableArray();
            return match.Success;
        }
    }
}