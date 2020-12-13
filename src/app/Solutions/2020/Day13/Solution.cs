using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day13 {

    class Solution : ISolver {

        public string GetName() => "Shuttle Search";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var l = input.Lines().ToArray();
            var timestamp = int.Parse(l[0]);
            var availableBuses = l[1].Split(',')
                .Where(s => s != "x")
                .Select(int.Parse)
                .ToArray();
            var bestBus = availableBuses
                .Select(b => new { Bus = b, Wait = b - timestamp % b})
                .OrderBy(b => b.Wait)
                .ToArray()[0];
            var wait = bestBus.Bus * (bestBus.Bus - timestamp % bestBus.Bus);
            return wait;
        }

        private record Bus
        {
            public int BusNumber { get; }
            public int Index { get; }
            public Bus(int number, int index) { BusNumber = number; Index = index;}
            public int Remainder => Index % BusNumber;
        }

        object PartTwo(string input)
        {
            var l = input.Lines().ToArray();
            var buses = l[1].Split(',')
                .Select((b, i) => b != "x" ? new Bus(int.Parse(b), i) : null)
                .Where(b => b != null)
                .ToArray();

            var increment = 1L; // To start with, we increment by a single minute (which means we test every minute).
            var time = 0L;      // Assume that the 0-minute will work.
            // Console.WriteLine($"Looking for:\n  {string.Join("\n  ", buses.Select(b => b.ToString()))}");
            foreach(var bus in buses)
            {
                // Console.WriteLine($"Testing bus {bus}. Current time {time}, increment: {increment}.");
                var iteration = 0L;
                while (true)
                {
                    if ((time + bus.Remainder) % bus.BusNumber == 0)
                    {
                        // We've found a time when this bus matches the correct schedule.
                        // This will only occur at increments of BusNumber again.
                        // Console.WriteLine($"At {time} we've got the following result:");
                        // foreach(var b in buses.TakeWhile(bu => bu != bus))
                        // {
                        //     Console.WriteLine($"    {b.BusNumber}: Remainder: {b.Remainder}, offset: {time % b.BusNumber}");
                        // }
                        // Console.WriteLine($"    {bus.BusNumber}: Remainder: {bus.Remainder}, offset: {time % bus.BusNumber}");
                        increment *= bus.BusNumber; // Since all bus numbers are prime numbers, this works.
                        break;
                    }

                    iteration++;
                    // if (iteration % 1_000_000_000 == 0)
                    // {
                    //     Console.WriteLine($"    Testing. Iteration {iteration}. Current time: {time}.");
                    // }

                    // Test the next time that could possibly be the solution.
                    time += increment;
                }
            }

            return time;
        }
    }
}