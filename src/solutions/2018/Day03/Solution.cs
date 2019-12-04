using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2018.Day03 {

    class Solution : ISolver {

        public string GetName() => "No Matter How You Slice It";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var claims = input.Lines()
                .Matches(@"\d+")
                .Select(matches => matches.Select(m => int.Parse(m.Value)).ToArray())
                .Select(v => new {Id = v[0], Rect = new Rect(v[1], v[2], v[3], v[4])}).ToArray();

            var fabric = new int[1000, 1000];
            foreach (var claim in claims)
            {
                claim.Rect.Cover(fabric);
            }

            var doubleCoveredAreas = 0;
            for (var x = 0; x < 1000; x++)
            {
                for (var y = 0; y < 1000; y++)
                {
                    if (fabric[x, y] > 1)
                    {
                        doubleCoveredAreas++;
                    };
                }
            }

            return doubleCoveredAreas;
        }

        public struct Rect
        {
            public int Left { get; }
            public int Top { get; }
            public int Width { get; }
            public int Height { get; }

            public Rect(int left, int top, int width, int height)
            {
                Left = left;
                Top = top;
                Width = width;
                Height = height;
            }

            public void Cover(int[,] fabric)
            {
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        fabric[Left + x, Top + y]++;
                    }
                }
            }

            public IEnumerable<int> Cover2(int[,] fabric, int id)
            {
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (fabric[Left + x, Top + y] == 0)
                        {
                            fabric[Left + x, Top + y] = id;
                        }
                        else
                        {
                            yield return id;
                            yield return fabric[Left + x, Top + y];
                        }
                    }
                }
            }
        }

        object PartTwo(string input) {
            var claims = input.Lines()
                .Matches(@"\d+")
                .Select(matches => matches.Select(m => int.Parse(m.Value)).ToArray())
                .ToDictionary(v => v[0], v => new Rect(v[1], v[2], v[3], v[4]));

            var fabric = new int[1000, 1000];
            var nonCoveredClaims = new HashSet<int>(claims.Keys);
            foreach (var claim in claims)
            {
                foreach (var coveredClaim in claim.Value.Cover2(fabric, claim.Key))
                {
                    if (nonCoveredClaims.Contains(coveredClaim))
                    {
                        nonCoveredClaims.Remove(coveredClaim);
                    }
                }
            }

            if (nonCoveredClaims.Count != 1)
            {
                throw new Exception("Something wrong with the algorithm.");
            }

            return nonCoveredClaims.First();
        }
    }
}