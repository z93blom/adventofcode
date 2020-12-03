using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day03 {

    class Solution : ISolver {

        public string GetName() => "Toboggan Trajectory";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var board = input.Lines().ToArray();
            var xStep = 3;
            var yStep = 1;
            return CountHits(board, xStep, yStep);
        }

        private static int CountHits(string[] board, int xStep, int yStep)
        {
            var x = 0;
            var y = 0;
            var hits = 0;
            while (y < board.Length)
            {
                if (board[y][x] == '#')
                    hits++;
                x = (x + xStep) % board[0].Length;
                y = y + yStep;
            }

            return hits;
        }

        object PartTwo(string input) {
            var board = input.Lines().ToArray();
            var hitList = new List<long>();
            hitList.Add(CountHits(board, 1, 1));
            hitList.Add(CountHits(board, 3, 1));
            hitList.Add(CountHits(board, 5, 1));
            hitList.Add(CountHits(board, 7, 1));
            hitList.Add(CountHits(board, 1, 2));

            return hitList.Aggregate((long)1, (p, v) => p*v);
        }
    }
}