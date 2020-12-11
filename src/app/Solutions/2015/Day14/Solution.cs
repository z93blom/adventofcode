using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day14 {

    class Solution : ISolver {

        public string GetName() => "Reindeer Olympics";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        [DebuggerDisplay("{" + nameof(Name) + "}")]
        public class Reindeer
        {
            public string Name { get; }
            public int FlyingSpeed { get; }
            public int FlyingTime { get; }
            public int RestingTime { get; }

            public double AverageSpeed =>
                this.FlyingTime * this.FlyingSpeed / (double) (this.FlyingTime + this.RestingTime);

            public Reindeer(string name, int flyingSpeed, int flyingTime, int restingTime)
            {
                Name = name;
                FlyingSpeed = flyingSpeed;
                FlyingTime = flyingTime;
                RestingTime = restingTime;
            }

            public int GetDistanceFor(int second)
            {
                var cyclePart = second % (this.FlyingTime + this.RestingTime);
                if (cyclePart < this.FlyingTime)
                    return this.FlyingSpeed;
                return 0;
            }
        }

        object PartOne(string input)
        {
            var reindeers = GetReindeers(input);

            // Simulate how far each and every reindeer has reached at 2503 seconds.
            var result = new Dictionary<string, int>();
            foreach (var reindeer in reindeers)
            {
                var distance = 0;
                var remainingTime = 2503;
                while (remainingTime > 0)
                {
                    var travelingTime = Math.Min(reindeer.FlyingTime, remainingTime);
                    distance += travelingTime * reindeer.FlyingSpeed;
                    remainingTime -= travelingTime;
                    remainingTime -= reindeer.RestingTime;
                }

                result.Add(reindeer.Name, distance);
            }

            var maxDistance = result.Max(kvp => kvp.Value);
            return maxDistance;
        }

        private static ImmutableArray<Reindeer> GetReindeers(string input)
        {
            var reindeers = input
                .Lines()
                .Match(@"(\w+) .* (\d+) .* (\d+) .* (\d+).*") //km/s for (\d+) seconds .+ (\d+) seconds\.")
                .Groups()
                .Select(g => new Reindeer(g.Elements[0], int.Parse(g.Elements[1]), int.Parse(g.Elements[2]),
                    int.Parse(g.Elements[3])))
                .ToImmutableArray();
            return reindeers;
        }

        object PartTwo(string input)
        {
            var reindeers = GetReindeers(input);

            var currentDistance = new Dictionary<Reindeer, int>().Initialize(reindeers, 0);
            var points = new Dictionary<Reindeer, int>().Initialize(reindeers, 0);
            for (var second = 0; second < 2503; second++)
            {
                foreach (var reindeer in reindeers)
                {
                    currentDistance[reindeer] += reindeer.GetDistanceFor(second);
                }

                var orderedDistances = currentDistance.OrderByDescending(kvp => kvp.Value).ToImmutableArray();
                foreach (var leader in orderedDistances.TakeWhile(kvp => kvp.Value == orderedDistances[0].Value))
                {
                    points[leader.Key] += 1;
                }
            }

            var maxPoints = points.Max(kvp => kvp.Value);
            return maxPoints;
        }
    }
}