using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using System.CodeDom.Compiler;

namespace AdventOfCode.Y2020.Day22 {

    class Solution : ISolver {

        public string GetName() => "Crab Combat";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var players = input.Split("\n\n");
            var one = new Queue<int>(players[0].Lines().Skip(1).Select(int.Parse));
            var two = new Queue<int>(players[1].Lines().Skip(1).Select(int.Parse));

            while(one.Count > 0 && two.Count > 0)
            {
                var p1 = one.Dequeue();
                var p2 = two.Dequeue();
                if (p1 > p2)
                {
                    one.Enqueue(p1);
                    one.Enqueue(p2);
                }
                else
                {
                    two.Enqueue(p2);
                    two.Enqueue(p1);
                }
            }

            var winningDeck = one.Count == 0 ? two : one;

            var reverse = winningDeck.Reverse().ToArray();
            var retVal = 0L;
            for(int i  = 0; i < reverse.Length;)
                retVal += reverse[i]*(++i);

            return retVal;
        }
        
        object PartTwo(string input)
        {
            var players = input.Split("\n\n");
            var one = new Queue<int>(players[0].Lines().Skip(1).Select(int.Parse));
            var two = new Queue<int>(players[1].Lines().Skip(1).Select(int.Parse));

            var logger = new IndentedTextWriter(Console.Out, "  ");
            var winner = RecursiveCombat(/*logger,*/ one, two);

            var winningDeck = winner == Player.One ? one : two;

            var reverse = winningDeck.Reverse().ToArray();
            var retVal = 0L;
            for (int i = 0; i < reverse.Length;)
                retVal += reverse[i] * (++i);

            return retVal;
        }

        private enum Player{
            One,
            Two
        }

        private static int GameNumber = 0;
        private static Player RecursiveCombat(/*IndentedTextWriter writer,*/ Queue<int> one, Queue<int> two)
        {   
            var previous = new HashSet<string>();
            var thisGame = ++GameNumber;
            //writer.WriteLine($"=== Game {thisGame} ===");
            //writer.Indent++;
            var round = 0L;
            while (one.Count > 0 && two.Count > 0)
            {
                if (++round % 100 == 0)
                {
                    //writer.WriteLine($"{thisGame}.{round}. One: {one.Count}, Two: {two.Count}");
                }

                var unique = ToUnique(one, two);
                if (previous.Contains(unique))
                {
                    //writer.WriteLine($"Player one won by default.");
                    return Player.One;
                }

                previous.Add(unique);

                var p1 = one.Dequeue();
                var p2 = two.Dequeue();

                Player winner;
                if(p1 <= one.Count && p2 <= two.Count)
                {
                    // Need to determine the winner by recursion
                    var newOne = new Queue<int>(one.Take(p1));
                    var newTwo = new Queue<int>(two.Take(p2));
                    //StackDepth++;
                    //writer.WriteLine($"Determining winner of round {round} by recursion. Current depth: {StackDepth}");
                    winner = RecursiveCombat(/*writer,*/ newOne, newTwo);
                    //StackDepth--;
                }
                else
                {
                    winner = p1 > p2 ? Player.One : Player.Two;
                }

                if (winner == Player.One)
                {
                    one.Enqueue(p1);
                    one.Enqueue(p2);
                }
                else
                {
                    two.Enqueue(p2);
                    two.Enqueue(p1);
                }
            }

            //writer.Indent--;
            var gameWinner = one.Count > 0 ? Player.One : Player.Two;
            //writer.WriteLine($"The winner of game {thisGame} is {gameWinner}.");
            
            return gameWinner;
        }

        private static string ToUnique(Queue<int> one, Queue<int> two)
        {
            return "One" + string.Join(",", one) + "Two" + string.Join(",", two);
        }
    }
}