using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day12 {

    class Solution : ISolver {

        public string GetName() => "The N-Body Problem";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var positions = input.Lines()
                .Matches(@"(-?\d+)")
                .SelectMany(ms => ms)
                .Select(m => int.Parse(m.Captures[0].Value))
                .ToArray();
            var velocities = new int[positions.Length];

            for (var step = 1; step <= 1000; step++)
            {
                var newPositions = new int[positions.Length];
                for (var i = 0; i < positions.Length; i++)
                {
                    // Apply Gravity
                    var thisPosition = positions[i];
                    velocities[i] = velocities[i] +
                                    positions
                                        .Where((val, index) => index % 3 == i % 3)
                                        .Sum(val => val.CompareTo(thisPosition));
                    // Apply velocity
                    newPositions[i] = positions[i] + velocities[i];
                }

                positions = newPositions;
            }

            var totalEnergy = 0;
            for (var i = 0; i < positions.Length; i+=3)
            {
                totalEnergy += positions.Skip(i).Take(3).Sum(Math.Abs) *
                               velocities.Skip(i).Take(3).Sum(Math.Abs);
            }

            return totalEnergy;
        }

        object PartTwo(string input) {
            var originalPositions = input.Lines()
                .Matches(@"(-?\d+)")
                .SelectMany(ms => ms)
                .Select(m => int.Parse(m.Captures[0].Value))
                .ToArray();
            var originalVelocities = new int[originalPositions.Length];

            var count = input.Lines().Count();
            var pos = new int[3][];
            for (var axis = 0; axis < 3; axis++)
            {
                pos[axis] = new int[count * 2];
            }

            for (var i = 0; i < count; i++)
            {
                pos[0][i] = originalPositions[i * 3];
                pos[0][i + count] = originalVelocities[i * 3];

                pos[1][i] = originalPositions[i * 3 + 1];
                pos[1][i + count] = originalVelocities[i * 3 + 1];

                pos[2][i] = originalPositions[i * 3 + 2];
                pos[2][i + count] = originalVelocities[i * 3 + 2];
            }

            var loops = new long[pos.Length];
            for (var axis = 0; axis < pos.Length; axis++)
            {
                var orig = new int[pos[axis].Length];
                Array.Copy(pos[axis], orig, pos[axis].Length);
                do
                {
                    loops[axis]++;

                    var v = pos[axis];

                    // Apply Gravity
                    for (var i = 0; i < count; i++)
                    {
                        var thisPosition = v[i];
                        var deltaV = 0;
                        for (var s = 0; s < count; s++)
                        {
                            deltaV += v[s].CompareTo(thisPosition);
                        }
                        
                        v[i + count] = v[i + count] + deltaV;
                    }

                    // Apply velocity
                    for (var i = 0; i < count; i++)
                    {
                        v[i] += v[i + count];
                    }

                } while (!AreEqual(orig, pos[axis]));
            }

            return GetGCF(loops[0], GetGCF(loops[1], loops[2]));

            //var loop = new long[3];
            //for (var axis = 0; axis < 3; axis++)
            //{
            //    var steps = 0L;
            //    do
            //    {
            //        loop[axis]++;
            //        var newPositions = new int[positions.Length];
            //        for (var i = axis; i < positions.Length; i += 3)
            //        {
            //            // Apply Gravity
            //            var thisPosition = positions[i];
            //            velocities[i] = velocities[i] +
            //                            positions[axis].CompareTo(thisPosition) +
            //                            positions[axis + 3].CompareTo(thisPosition) +
            //                            positions[axis + 6].CompareTo(thisPosition) +
            //                            positions[axis + 9].CompareTo(thisPosition);
            //            // Apply velocity
            //            newPositions[i] = positions[i] + velocities[i];
            //        }

            //        positions = newPositions;
            //    } while (!AreEqual(positions, originalPositions, axis) ||
            //             !AreEqual(velocities, originalVelocities, axis));
            //}

            //return GetGCF(loop[0], GetGCF(loop[1], loop[2]));
            return 0;
        }

        static long GetGCF(long a, long b)
        {
            return (a * b) / GetGCD(a, b);
        }

        static long GetGCD(long a, long b)
        {
            while (a != b)
            {
                if (a < b)
                {
                    b = b - a;
                }
                else
                {
                    a = a - b;
                }
            }

            return a;
        }

        private static bool AreEqual(int[] left, int[] right)
        {
            for (var index = 0; index < left.Length; index++)
            {
                if (left[index] != right[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
