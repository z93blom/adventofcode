using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day17 {

    class Solution : ISolver {

        public string GetName() => "Conway Cubes";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        public enum State
        {
            Inactive,
            Active,
        }

        public struct Point
        {
            public int X {get;}
            public int Y {get;}
            public int Z {get;}

            public Point(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public IEnumerable<Point> GetAdjacent()
            {
                for(int z = Z - 1; z <= Z + 1; z++)
                    for(int y = Y - 1; y <= Y + 1; y++)
                        for(int x = X - 1; x <= X + 1; x++)
                            if (x != X || y != Y || z != Z)
                                yield return new Point(x, y, z);
            }

            public override string ToString()
            {
                return $"({X}, {Y}, {Z}";
            }
        }

        public class Cube
        {
            public Point Location {get; init;}

            public State Current {get; set;}
            public State Next {get;set;}

            public static bool IsActive(Cube cube) => cube.Current == State.Active;

            public override string ToString()
            {
                return $"{Location}: {Current}";
            }
        }

        public class Game
        {
            private Dictionary<Point, Cube> cubes = new Dictionary<Point, Cube>();
            public int Cycle {get; private set;} = 0;

            public Cube this[int x, int y, int z]
            {
                get { return this[new Point(x, y, z)]; }
            }

            public Cube this[Point p]
            {
                get
                {
                    if (!this.cubes.ContainsKey(p))
                    {
                        Cube cube = new Cube { Location = p, Current = State.Inactive, Next = State.Inactive};
                        cubes[p] = cube;
                    }

                    return cubes[p];
                }
            }

            public IEnumerable<Cube> GetAdjacentActiveCubes(Point point)
            {
                foreach (var p in point.GetAdjacent())
                {
                    if (cubes.ContainsKey(p))
                        yield return cubes[p];
                }
            }

            public void ApplyRules()
            {
                HashSet<Point> evaluated = new HashSet<Point>();
                foreach(var c in this.cubes.Values.ToArray())
                {
                    // If a cube is *active* and *exactly `2` or `3`* of its neighbors are also active, the cube remains *active*. Otherwise, the cube becomes *inactive*.
                    var activeNeighbors = GetAdjacentActiveCubes(c.Location).Count();
                    if (activeNeighbors == 2 || activeNeighbors == 3)
                    {
                        c.Next = State.Active;
                    }
                    else
                    {
                        c.Next = State.Inactive;
                    }

                    evaluated.Add(c.Location);

                    // If a cube is *inactive* but *exactly `3`* of its neighbors are active, the cube becomes *active*. Otherwise, the cube remains *inactive*.
                    foreach(var p in c.Location.GetAdjacent().Where(p => !evaluated.Contains(p) && !this.cubes.ContainsKey(p)))
                    {
                        evaluated.Add(p);
                        if (GetAdjacentActiveCubes(p).Count() == 3)
                            this[p].Next = State.Active;
                    }                    
                }
            }

            public void NextCycle()
            {
                foreach(var c in cubes.Values.ToArray())
                {
                    // Only keep the active cubes.
                    if (c.Next != State.Active)
                        cubes.Remove(c.Location);
                    c.Current = c.Next;
                    c.Next = State.Inactive;
                }

                Cycle++;
            }

            public int Count => this.cubes.Count;


            public IEnumerable<Cube> Cubes => this.cubes.Values;

            public override string ToString()
            {
                var sb = new StringBuilder();
                var xMin = cubes.Keys.Select(p => p.X).Min();
                var xMax = cubes.Keys.Select(p => p.X).Max();
                var yMin = cubes.Keys.Select(p => p.Y).Min();
                var yMax = cubes.Keys.Select(p => p.Y).Max();

                sb.AppendLine("============================");
                sb.AppendLine($"Cycle: {Cycle}");
                sb.AppendLine();
                foreach(var z in this.cubes.Keys.Select(p => p.Z).Distinct().OrderBy(v => v))
                {
                    sb.AppendLine($"Z = {z}");
                    for(var y = yMin; y <= yMax; y++)
                    {
                        for(var x = xMin; x <= xMax; x++)
                        {
                            Point key = new Point(x, y, z);
                            sb.Append(this.cubes.ContainsKey(key) ? '#' : '.');
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                }
                sb.AppendLine("============================");

                return sb.ToString();
            }
        }

        object PartOne(string input) {
            var initialState = input.Lines()
                .Select(l => l.Select(c => c switch {'.' => State.Inactive, '#' => State.Active, _ => throw new Exception($"Unknown state '{c}'")}).ToArray())
                .ToArray();
            var g = new Game();
            for(var y = 0; y < initialState.Length; y++)
            {
                var yVec = initialState[y];
                for(var x = 0; x < yVec.Length; x++)
                {
                    var s = yVec[x];
                    if (s == State.Active)
                        g[x, y, 0].Current = s;
                }
            }

            Console.Write(g.ToString());

            while(g.Cycle < 6)
            {
                g.ApplyRules();
                g.NextCycle();
                Console.Write(g.ToString());
            }

            return g.Count;
        }

        object PartTwo(string input) {
            return 0;
        }
    }
}
