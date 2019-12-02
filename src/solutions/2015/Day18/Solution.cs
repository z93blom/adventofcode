using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day18 {

    class Solution : ISolver {

        public string GetName() => "Like a GIF For Your Yard";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var initialState = input
                .Lines()
                .Select(l => l.Select(c => c == '.' ? 0 : 1).ToArray())
                .ToArray();
            var state = initialState;
            for (var i = 0; i < 100; i++)
            {
                state = Run(state);
            }

            var lightsOn = state.SelectMany(r => r).Count(l => l == 1);
            return lightsOn;
        }

        
        object PartTwo(string input) {
            var initialState = input
                .Lines()
                .Select(l => l.Select(c => c == '.' ? 0 : 1).ToArray())
                .ToArray();
            var state = initialState;
            TurnOnCorners(state);
            for (var i = 0; i < 100; i++)
            {
                state = TurnOnCorners(Run(state));
            }

            var lightsOn = state.SelectMany(r => r).Count(l => l == 1);
            return lightsOn;
        }

        private int[][] TurnOnCorners(int[][] state)
        {
            state[0][0] = 1;
            state[0][state[0].Length - 1] = 1;
            state[state.Length - 1][0] = 1;
            state[state.Length - 1][state[0].Length - 1] = 1;
            return state;
        }

        private int[][] Run(int[][] state)
        {
            int NeighborsOn(int row, int col)
            {
                var lowRow = Math.Max(0, row - 1);
                var highRow = Math.Min(state.Length, row + 2);
                var lowCol = Math.Max(0, col - 1);
                var highCol = Math.Min(state[0].Length, col + 2);

                var count = 0;
                for (var r = lowRow; r < highRow; r++)
                {
                    for (var c = lowCol; c < highCol; c++)
                    {
                        count += state[r][c];
                    }
                }

                // Remove the state of the current (we are only counting the neighbors)
                return count - state[row][col];
            }

            int CalcValue(int rowIndex, int colIndex)
            {
                var neighborsOn = NeighborsOn(rowIndex, colIndex);
                var newState = state[rowIndex][colIndex] == 1 && (neighborsOn == 2 || neighborsOn == 3) ||
                               state[rowIndex][colIndex] == 0 && neighborsOn == 3;

                return newState ? 1 : 0;
            }

            var returnValue =
                state.Select((row, rowIndex) => row.Select((col, colIndex) => CalcValue(rowIndex, colIndex)).ToArray()).ToArray();
            return returnValue;
        }

    }
}