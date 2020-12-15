using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day15 {

    class Solution : ISolver {

        public string GetName() => "Rambunctious Recitation";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        class Game{
            private Dictionary<int, int> _map = new Dictionary<int, int>();
            
            public int Turn { get; private set; } = 1;

            public Game()
            {
            }

            public int Initialize(IEnumerable<int> startingSequence)
            {
                int returnValue = 0;
                foreach(var n in startingSequence)
                {
                    returnValue = Play(n);
                }

                return returnValue;
            }

            public int Play(int number)
            {
                int returnValue = 0;
                if (_map.ContainsKey(number))
                {
                    returnValue = Turn - _map[number];
                }
                _map[number] = Turn;
                Turn++;

                return returnValue;
            }
        }

        object PartOne(string input) {
            var startingSequence = input.Split(',').Select(int.Parse).ToArray();
            var g = new Game();
            var nextNumber = g.Initialize(startingSequence);
            while(g.Turn < 2020)
            {
                nextNumber = g.Play(nextNumber);
            }

            return nextNumber;
        }

        object PartTwo(string input) {
            var startingSequence = input.Split(',').Select(int.Parse).ToArray();
            var g = new Game();
            var nextNumber = g.Initialize(startingSequence);
            while(g.Turn < 30_000_000)
            {
                nextNumber = g.Play(nextNumber);
            }

            return nextNumber;
        }
    }
}