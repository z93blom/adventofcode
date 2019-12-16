using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day16 {

    class Solution : ISolver {

        public string GetName() => "Flawed Frequency Transmission";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        IEnumerable<long> ToInts(string input)
        {
            return input.Select(c => long.Parse(c.ToString()));
        }

        IEnumerable<int> GetPattern(int[] start, int element)
        {
            return GetPatternWithoutSkip(start, element).Skip(1);
        }

        IEnumerable<int> GetPatternWithoutSkip(int[] start, int element)
        {
            var repeats = element + 1;
            while (true)
            {
                for (var index = 0; index < start.Length; index++)
                {
                    for (var repeat = 0; repeat < repeats; repeat++)
                    {
                        yield return start[index];
                    }
                }
            }
        }

        object PartOne(string input)
        {
            //var startingPattern = new[] {0, 1, 0, -1};
            //var values = ToInts(input).ToArray();

            //// Get all the patterns (they will remain the same for all iterations
            //var patterns = new int[values.Length][];
            //for (var index = 0; index < values.Length; index++)
            //{
            //    patterns[index] = GetPattern(startingPattern, index).Take(values.Length).ToArray();
            //}

            //for (var iteration = 0; iteration < 100; iteration++)
            //{
            //    var newValues = new long[values.Length];
            //    for (var index = 0; index < values.Length; index++)
            //    {
            //        newValues[index] = values.Zip(patterns[index], (val, patternValue) => val * patternValue)
            //            .Sum().ToString()
            //            .Reverse()
            //            .Take(1)
            //            .Select(c => long.Parse(c.ToString()))
            //            .First();
            //    }

            //    values = newValues;
            //}

            //return string.Join("", values.Take(8).Select(i => i.ToString()));

            var startingPattern = new[] { 0, 1, 0, -1 };
            var values = ToInts(input).ToArray();

            // Get all the patterns (they will remain the same for all iterations
            var patterns = new int[values.Length][];
            for (var index = 0; index < values.Length; index++)
            {
                patterns[index] = new int[values.Length];
                for(var i = 0; i < values.Length; i++)
                {
                    patterns[index][i] = GetPatternValue(startingPattern, index, i);
                }
            }

            for (var iteration = 0; iteration < 100; iteration++)
            {
                var newValues = new long[values.Length];
                for (var index = 0; index < values.Length; index++)
                {
                    newValues[index] = values.Zip(patterns[index], (val, patternValue) => val * patternValue)
                        .Sum()
                        .ToString()
                        .Reverse()
                        .Take(1)
                        .Select(c => long.Parse(c.ToString()))
                        .First();
                }

                values = newValues;
            }

            return string.Join("", values.Take(8).Select(i => i.ToString()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetPatternValue(int[] start, int element, int index)
        {
            var repeats = element + 1;
            var repeatLength = start.Length * repeats;
            var indexInRepeat = index % repeatLength;

            // Skip the first.
            indexInRepeat = (indexInRepeat + 1) % repeatLength;
            var indexInStart = (indexInRepeat / repeats);

            return start[indexInStart];
        }
        
        object PartTwo(string input)
        {

            return 0;
            var startingPattern = new[] { 0, 1, 0, -1 };
            var values = ToInts(input)
                .Repeat(10000)
                .ToArray();

            var offset = int.Parse(string.Join("", values.Take(7)));

            // Get all the patterns (they will remain the same for all iterations
            var patterns = new int[values.Length][];
            for (var index = 0; index < values.Length; index++)
            {
                patterns[index] = GetPattern(startingPattern, index).Take(values.Length).ToArray();
            }

            for (var iteration = 0; iteration < 100; iteration++)
            {
                var newValues = new long[values.Length];
                for (var index = 0; index < values.Length; index++)
                {
                    newValues[index] = values.Zip(patterns[index], (val, patternValue) => val * patternValue)
                        .Sum().ToString()
                        .Reverse()
                        .Take(1)
                        .Select(c => long.Parse(c.ToString()))
                        .First();
                }

                values = newValues;
            }

            return string.Join("", values.Skip(offset).Take(8).Select(i => i.ToString()));
        }
    }
}