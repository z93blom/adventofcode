using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day02 {

    class Solution : ISolver {

        public string GetName() => "1202 Program Alarm";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var memory = input.Split(',').Select(int.Parse).ToList();
            memory[1] = 12;
            memory[2] = 2;
            var instructionPointer = 0;
            while (true)
            {
                var (finished, offset) = Run(memory, instructionPointer);
                if (finished)
                {
                    break;
                }
                instructionPointer += offset;
            }

            return memory[0];
        }

        (bool finished, int offset) Run(List<int> memory, int address)
        {
            var opCode = memory[address];
            var leftPosition = address + 1;
            var rightPosition = address + 2;
            switch (opCode)
            {
                case 1:
                {
                    var left = SafeGetData(memory, leftPosition);
                    var right = SafeGetData(memory, rightPosition);
                    var val = left + right;
                    memory[memory[address + 3]] = val;
                    return (false, 4);
                }

                case 2:
                {
                    var left = SafeGetData(memory, leftPosition);
                    var right = SafeGetData(memory, rightPosition);
                    var val = left * right;
                    memory[memory[address + 3]] = val;
                    return (false, 4);
                }

                case 99:
                {
                    return (true, 1);
                }

                default:
                    throw new InvalidOperationException();
            }
        }

        private static int SafeGetData(List<int> data, int position)
        {
            if (position >= data.Count)
            {
                return 0;
            }

            var index = data[position];
            if (index >= data.Count)
            {
                return 0;
            }

            return data[index];
        }

        object PartTwo(string input) {
            var noun = Enumerable.Range(0, 100);
            var verb = Enumerable.Range(0, 100).ToArray();
            foreach (var n in noun)
            {
                foreach (var v in verb)
                {
                    var memory = input.Split(',').Select(int.Parse).ToList();
                    memory[1] = n;
                    memory[2] = v;
                    var instructionPointer = 0;
                    while (true)
                    {
                        var (finished, offset) = Run(memory, instructionPointer);
                        if (finished)
                        {
                            break;
                        }
                        instructionPointer += offset;
                    }

                    if (memory[0] == 19690720)
                    {
                        return 100 * n + v;
                    }
                }
            }

            //throw new InvalidOperationException();
            return 0;

        }
    }
}