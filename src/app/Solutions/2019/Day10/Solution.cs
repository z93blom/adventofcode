using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day10 {

    class Solution : ISolver {

        public string GetName() => "Monitoring Station";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var asteroids = GetAsteroids(input);
            CalculateSeenAsteroids(asteroids);

            var orderedBySeen = asteroids.OrderByDescending(kvp => kvp.Value.Count).ToArray();
            var maxSeen = orderedBySeen[0].Value.Count;
            return maxSeen;
        }

        private static void CalculateSeenAsteroids(Dictionary<Point, List<Point>> asteroids)
        {
            foreach (var point in asteroids.Keys)
            {
                var seen = asteroids[point];
                var blockedAngles = new HashSet<double>();
                var otherAsteroids = asteroids.Keys.OrderBy(p => SquaredDist(point, p)).Skip(1).ToArray();
                foreach (var p1 in otherAsteroids)
                {
                    var angle = GetAngleToPositiveX(point, p1);
                    if (blockedAngles.Contains(angle))
                    {
                        continue;
                    }

                    seen.Add(p1);
                    blockedAngles.Add(angle);
                }
            }
        }

        private static double GetAngleToPositiveX(Point @from, Point to)
        {
            var deltaX = to.X - @from.X;
            var deltaY = to.Y - @from.Y;
            var angle = Math.Atan2(deltaY, deltaX);
            return angle;
        }

        static int SquaredDist(Point p1, Point p2)
        {
            var deltaX = p1.X - p2.X;
            var deltaY = p1.Y - p2.Y;
            var xSquared = deltaX * deltaX;
            var ySquared = deltaY * deltaY;
            return xSquared + ySquared;
        }

        private static Dictionary<Point, List<Point>> GetAsteroids(string input)
        {
            var asteroids = new Dictionary<Point, List<Point>>();
            var y = 0;
            foreach (var line in input.Lines())
            {
                var x = 0;
                foreach (var c in line)
                {
                    if (c == '#')
                    {
                        asteroids.Add(new Point(x, y), new List<Point>());
                    }

                    x++;
                }

                y++;
            }

            return asteroids;
        }

        object PartTwo(string input) {
            var asteroids = GetAsteroids(input);
            CalculateSeenAsteroids(asteroids);
            var orderedBySeen = asteroids.OrderByDescending(kvp => kvp.Value.Count).ToArray();
            var asteroidWithLaser = orderedBySeen[0];

            var point = asteroidWithLaser.Key;
            var others = asteroids.Keys.Select(p =>
                {
                    var angle = -GetAngleToPositiveX(point, p);
                    var angleInDegrees = (180 / Math.PI) * angle;
                    return new
                    {
                        Location = p, 
                        Angle =  angle,
                        AngleInDeg = angleInDegrees,
                        ShiftedAngleInDegrees = angleInDegrees > 90 ? angleInDegrees - 360 : angleInDegrees,
                        SquaredDist = SquaredDist(point, p)

                    };
                })
                .Where(pa => pa.SquaredDist > 0)
                .OrderByDescending(pa => pa.ShiftedAngleInDegrees)
                .ToArray();
            var seenAndAngle = others
                .GroupBy(pa => pa.ShiftedAngleInDegrees)
                .OrderByDescending(g => g.Key)
                .Select(g => g.OrderBy(a => a.SquaredDist).ToList())
                .ToArray();

            var destroyed = new List<Point>();
            var currentIndex = 0;
            while (destroyed.Count < asteroids.Count - 1)
            {
                while (seenAndAngle[currentIndex].Count < 1)
                {
                    currentIndex = (currentIndex + 1) % seenAndAngle.Length;
                }

                var foo = seenAndAngle[currentIndex][0];
                destroyed.Add(foo.Location);
                seenAndAngle[currentIndex].RemoveAt(0);
                currentIndex = (currentIndex + 1) % seenAndAngle.Length;
            }

            return destroyed[Math.Min(destroyed.Count - 1, 199)].X * 100 + destroyed[Math.Min(destroyed.Count - 1, 199)].Y;
        }
    }
}