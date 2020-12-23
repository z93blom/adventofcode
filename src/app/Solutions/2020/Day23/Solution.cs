using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using System.IO;
using System.Collections;

namespace AdventOfCode.Y2020.Day23 {

    class Solution : ISolver {

        public string GetName() => "Crab Cups";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {

            var list = new List<int>(input.Select(c => int.Parse(c.ToString())));
            var cups = new CircularLinkedList<int>(list);

            RunCups2(cups, 100, true);

            var one = cups[1];
            var current = one.Next;
            var sb = new StringBuilder();
            while(current != one)
            {
                sb.Append(current.Label);
                current = current.Next;
            }

            return sb.ToString();
        }

        private class Node<T> : IEnumerable<Node<T>>
        {
            public Node<T> Previous {get; set;}
            public Node<T> Next {get; set;}
            public T Label {get;}

            public Node(T value)
            {
                Label = value;
            }

            public override string ToString()
            {
                return $"{Label}";
            }

            public IEnumerator<Node<T>> GetEnumerator()
            {
                var current = this;
                do
                {
                    yield return current;
                    current = current.Next;
                }
                while(current != this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class CircularLinkedList<T> : IEnumerable<Node<T>>
        {
            private Dictionary<T, Node<T>> _nodeLookup = new Dictionary<T, Node<T>>();

            public Node<T> Head {get; private set;}
            public CircularLinkedList(IEnumerable<T> values)
            {
                Node<T> previous = null;
                foreach(var v in values)
                {
                    var node = new Node<T>(v);
                    _nodeLookup.Add(v, node);
                    if (Head == null)
                    {
                        Head = node;
                    }

                    if (previous != null)
                    {
                        previous.Next = node;
                    }

                    node.Previous = previous;
                    previous = node;
                }

                previous.Next = Head;
                Head.Previous = previous;
            }

            public Node<T> this[T index]
            {
                get { return _nodeLookup[index]; }
            }

            public void Remove(T value)
            {
                var node = this[value];
                if (Head == node)
                {
                    Head = node.Next;
                }

                node.Previous.Next = node.Next;
                node.Next.Previous = node.Previous;

                _nodeLookup.Remove(node.Label);
                node.Next = null;
                node.Previous = null;
            }

            public void InsertAfter(T label, IEnumerable<T> values)
            {
                var current = this[label];
                var next = current.Next;
                foreach(var v in values)
                {
                    var node = new Node<T>(v);
                    _nodeLookup.Add(v, node);
                    current.Next = node;
                    node.Previous = current;

                    current = node;
                }

                current.Next = next;
                next.Previous = current;
            }

            public IEnumerator<Node<T>> GetEnumerator()
            {
                if (Head == null)
                    yield break;
                
                foreach(var n in Head)
                    yield return n;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
               return this.GetEnumerator();
            }
        }

        private static void RunCups2(CircularLinkedList<int> list, int turns, bool debugPrint)
        {
            var lowestLabel = list.Head.Select(n => n.Label).Min();
            var highestLabel = list.Head.Select(n => n.Label).Max();
            var currentLabel = list.Head.Label;
            var currentTurn = 0;
            var removedLabels = new int[3];
            while (currentTurn < turns)
            {
                if (++currentTurn % 10_000 == 0)
                {
                    Console.Out.Write(".");
                }

                if (currentTurn % 100_000 == 0)
                {
                    Console.Out.WriteLine($" {currentTurn}");
                }
                
                if (debugPrint)
                    PrintInitialState(Console.Out, currentTurn, list, currentLabel);

                // The crab picks up the *three cups* that are immediately *clockwise* of the *current cup*.
                // They are removed from the circle; cup spacing is adjusted as necessary to maintain the circle.
                removedLabels = list[currentLabel].Next.Take(3).Select(n => n.Label).ToArray();
                foreach(var label in removedLabels)
                {
                    list.Remove(label);
                }

                // The crab selects a *destination cup*: the cup with a *label* equal to the *current cup's* label minus one.
                // If this would select one of the cups that was just picked up, the crab will keep subtracting one until it
                // finds a cup that wasn't just picked up. If at any point in this process the value goes below the lowest
                // value on any cup's label, it *wraps around* to the highest value on any cup's label instead.
                var destinationLabel = currentLabel - 1 < lowestLabel ? highestLabel : currentLabel - 1;
                while (removedLabels.Contains(destinationLabel))
                {
                    destinationLabel = destinationLabel - 1 < lowestLabel ? highestLabel : destinationLabel - 1;
                }

                if (debugPrint)
                    PrintState(Console.Out, removedLabels, destinationLabel);

                // The crab places the cups it just picked up so that they are *immediately clockwise* of the destination cup.
                // They keep the same order as when they were picked up.

                list.InsertAfter(destinationLabel, removedLabels);

                // The crab selects a new *current cup*: the cup which is immediately clockwise of the current cup.
                currentLabel = list[currentLabel].Next.Label;
            }
        }

        private static void RunCups(List<int> cups, int turns)
        {
            var lowestLabel = cups.Min();
            var highestLabel = cups.Max();
            var currentCup = cups[0];
            var currentTurn = 0;
            var temp = new int[3];
            while (currentTurn < turns)
            {
                if (++currentTurn % 10_000 == 0)
                {
                    Console.Out.Write(".");
                }

                if (currentTurn % 100_000 == 0)
                {
                    Console.Out.WriteLine($" {currentTurn}");
                }
                //PrintInitialState(Console.Out, currentTurn, cups, currentCup);

                // The crab picks up the *three cups* that are immediately *clockwise* of the *current cup*.
                // They are removed from the circle; cup spacing is adjusted as necessary to maintain the circle.
                var currentIndex = cups.IndexOf(currentCup);
                var pickup = (currentIndex + 1) % cups.Count;
                temp[0] = cups[(pickup + cups.Count) % cups.Count];
                temp[1] = cups[(pickup + cups.Count + 1) % cups.Count];
                temp[2] = cups[(pickup + cups.Count + 2) % cups.Count];
                cups.Remove(temp[0]);
                cups.Remove(temp[1]);
                cups.Remove(temp[2]);


                // The crab selects a *destination cup*: the cup with a *label* equal to the *current cup's* label minus one.
                // If this would select one of the cups that was just picked up, the crab will keep subtracting one until it
                // finds a cup that wasn't just picked up. If at any point in this process the value goes below the lowest
                // value on any cup's label, it *wraps around* to the highest value on any cup's label instead.
                var destinationCup = currentCup - 1 < lowestLabel ? highestLabel : currentCup - 1;
                while (temp.Contains(destinationCup))
                {
                    destinationCup = destinationCup - 1 < lowestLabel ? highestLabel : destinationCup - 1;
                }

                //PrintState(Console.Out, temp, destinationCup);                

                // The crab places the cups it just picked up so that they are *immediately clockwise* of the destination cup.
                // They keep the same order as when they were picked up.
                cups.InsertRange(cups.IndexOf(destinationCup) + 1, temp);

                // The crab selects a new *current cup*: the cup which is immediately clockwise of the current cup.
                currentCup = cups[(cups.IndexOf(currentCup) + 1) % cups.Count];
            }
        }

        private static void PrintInitialState(TextWriter writer, int turn, CircularLinkedList<int> list, int currentLabel)
        {
            writer.WriteLine($"-- move {turn} --");
            writer.Write("cups: ");
            foreach(var node in list)
            {
                if (currentLabel == node.Label)
                    writer.Write($"({node.Label}) ");
                else
                    writer.Write($"{node.Label} ");
            }
            writer.WriteLine();
        }


        private static void PrintState(TextWriter writer, int[] removedLabels, int destinationLabel)
        {
            writer.WriteLine($"pick up: {string.Join(" ", removedLabels)}");
            writer.WriteLine($"destination: {destinationLabel}");
            writer.WriteLine();
        }

        object PartTwo(string input) {
            var list = new List<int>(input.Select(c => int.Parse(c.ToString())));
            var max = list.Max();
            list.AddRange(Enumerable.Range(max + 1, 1_000_000 - list.Count));

            var cups = new CircularLinkedList<int>(list);

            RunCups2(cups, 10_000_000, false);

            var one = cups[1];
            var value = Math.BigMul(one.Next.Label, one.Next.Next.Label);

            return value;
        }
    }
}