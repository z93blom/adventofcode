using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day14 {

    class Solution : ISolver {

        public string GetName() => "Docking Data";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var lines = input.Lines();
            ulong zeroes = ulong.MaxValue;
            ulong ones = ulong.MinValue;
            var memory = new Dictionary<ulong, ulong>();
            var maskMatcher = new Regex(@"^mask = ([01X]{36})$");
            var memMatcher = new Regex(@"^mem\[(\d+)\] = (\d+)$");
            foreach(var l in lines)
            {
                if (maskMatcher.IsMatch(l))
                {
                    var m = maskMatcher.Match(l).Groups[1].Value;

                    var zeroString = m.Replace('X', '1');
                    zeroes = Convert.ToUInt64(zeroString, 2);
                    var oneString = m.Replace('X', '0');
                    ones = Convert.ToUInt64(oneString, 2);
                }
                else if (memMatcher.IsMatch(l))
                {
                    var match = memMatcher.Match(l);
                    var address = ulong.Parse(match.Groups[1].Value);
                    var value = ulong.Parse(match.Groups[2].Value);
                    
                    // Apply the masks to the value.
                    value = value & zeroes;
                    value = value | ones;
                    memory[address] = value;
                }
                else
                {
                    throw new Exception("Unexpected input");
                }
            }

            return memory.Sum(kvp => (decimal)kvp.Value);
        }



        object PartTwo(string input) {
            var lines = input.Lines();
            ulong zeroes = ulong.MaxValue;
            ulong ones = ulong.MinValue;
            int[] unknownBits = new int[0];
            var memory = new Dictionary<ulong, ulong>();
            var maskMatcher = new Regex(@"^mask = ([01X]{36})$");
            var memMatcher = new Regex(@"^mem\[(\d+)\] = (\d+)$");
            foreach(var l in lines)
            {
                if (maskMatcher.IsMatch(l))
                {
                    var m = maskMatcher.Match(l).Groups[1].Value;

                    var zeroString = m.Replace('X', '1');
                    zeroes = Convert.ToUInt64(zeroString, 2);
                    var oneString = m.Replace('X', '0');
                    ones = Convert.ToUInt64(oneString, 2);
                    var bit = 0;
                    var set = new HashSet<int>();
                    foreach(var c in m.Reverse())
                    {
                        if (c == 'X')
                            set.Add(bit);
                        bit++;
                    }
                    unknownBits = set.ToArray();
                }
                else if (memMatcher.IsMatch(l))
                {
                    var match = memMatcher.Match(l);
                    var address = ulong.Parse(match.Groups[1].Value);
                    var value = ulong.Parse(match.Groups[2].Value);
                    
                    // Apply the masks to the address.
                    address = address | ones;

                    IEnumerable<ulong> Permutations(int[] unknowns, ulong address)
                    {
                        if (unknowns.Length == 0)
                        {
                            yield break;
                        }

                        var bit = unknowns[0];
                        var lowerPermutations = Permutations(unknowns.Skip(1).ToArray(), address);
                        foreach(var a in lowerPermutations)
                        {

                            // Turn this bit to 0
                            yield return a & ~(1UL << bit);

                            // Turn this bit to 1
                            yield return a | 1UL << bit;
                        }

                        yield return address & ~(1UL << bit);
                        yield return address | 1UL << bit;
                    }

                    var addressPermutations = Permutations(unknownBits.ToArray(), address).Distinct().ToArray();
                    foreach (var a in addressPermutations)
                    {
                        memory[a] = value;
                    }
                }
                else
                {
                    throw new Exception("Unexpected input");
                }
            }

            return memory.Sum(kvp => (decimal)kvp.Value);       
        }

    }
}