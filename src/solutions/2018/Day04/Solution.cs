using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2018.Day04 {

    class Solution : ISolver {

        public string GetName() => "Repose Record";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var guardSleeping = GetSleepingEvents(input);

            var guardAndEvent = guardSleeping.OrderByDescending(kvp => kvp.Value.Total).First();
            return guardAndEvent.Key * guardAndEvent.Value.Highest;
        }

        private static Dictionary<int, SleepingEvents> GetSleepingEvents(string input)
        {
            var events = input.Lines()
                .Match(@"\[(\d{4}-\d{2}-\d{2}\s+\d{2}\:\d{2})\]\s+(.+)")
                .Select(m => new
                {
                    TimeStamp =
                        DateTime.ParseExact(m.Groups[1].Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Text = m.Groups[2].Value
                })
                .OrderBy(v => v.TimeStamp)
                .ToArray();

            var guard = -1;
            var isAwake = true;
            var lastEvent = 0;
            var guardSleeping = new Dictionary<int, SleepingEvents>();
            foreach (var e in events)
            {
                if (e.Text.Matches(@"Guard \#(\d+) begins shift", out var groups))
                {
                    // Handle the end of the last guard.
                    if (!isAwake)
                    {
                        guardSleeping[guard].Sleep(lastEvent, 60);
                    }

                    guard = int.Parse(groups[1].Value);
                    if (!guardSleeping.ContainsKey(guard))
                    {
                        guardSleeping.Add(guard, new SleepingEvents());
                    }

                    isAwake = true;
                    lastEvent = 0;
                }
                else if (e.Text.Matches(@"falls asleep", out _))
                {
                    isAwake = false;
                    lastEvent = e.TimeStamp.Minute;
                }
                else if (e.Text.Matches(@"wakes up", out _))
                {
                    // The guard was asleep between lastEvent and e.TimeStamp.Minute.
                    guardSleeping[guard].Sleep(lastEvent, e.TimeStamp.Minute);

                    isAwake = true;
                    lastEvent = e.TimeStamp.Minute;
                }
                else
                {
                    throw new NotImplementedException($"Unknown event: {e.Text}");
                }
            }

            if (!isAwake)
            {
                guardSleeping[guard].Sleep(lastEvent, 60);
            }

            return guardSleeping;
        }

        private class SleepingEvents
        {
            private readonly int[] Slept = new int[60];

            public int Total => Slept.Sum(i => i);

            public void Sleep(int startingMinute, int endingMinute)
            {
                for (var i = startingMinute; i < endingMinute; i++)
                {
                    Slept[i]++;
                }
            }

            public int Highest
            {
                get
                {
                    var max = Slept.Max(i => i);
                    for (var i = 0; i < 60; i++)
                    {
                        if (Slept[i] == max)
                        {
                            return i;
                        }
                    }

                    return -1;
                }
            }
        }

        object PartTwo(string input) {
            var guardSleeping = GetSleepingEvents(input);

            var guardAndEvent = guardSleeping.OrderByDescending(kvp => kvp.Value.Highest).First();
            return guardAndEvent.Key * guardAndEvent.Value.Highest;
        }
    }
}