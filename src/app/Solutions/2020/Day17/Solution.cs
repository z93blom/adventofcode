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
                return $"{Location}: {Current}->{Next}";
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
                    if (cubes.ContainsKey(p) && cubes[p].Current == State.Active)
                        yield return cubes[p];
                }
            }

            public void ApplyRules()
            {
                var points = PointsToEvaluate().ToArray();
                foreach (var p in points)
                {
                    var c = this[p];
                    if (c.Current == State.Active)
                    {
                        // If a cube is *active* and *exactly `2` or `3`* of its
                        // neighbors are also active, the cube remains *active*.
                        // Otherwise, the cube becomes *inactive*.
                        var activeNeighbors = GetAdjacentActiveCubes(p).Count();
                        if (activeNeighbors == 2 || activeNeighbors == 3)
                        {
                            c.Next = State.Active;
                        }
                    }
                    else
                    {
                        // If a cube is *inactive* but *exactly `3`* of its
                        // neighbors are active, the cube becomes *active*.
                        // Otherwise, the cube remains *inactive*.
                        if (GetAdjacentActiveCubes(p).Count() == 3)
                            this[p].Next = State.Active;
                    }
                }
            }

            public IEnumerable<Point> PointsToEvaluate()
            {
                var xMin = cubes.Keys.Select(p => p.X).Min();
                var xMax = cubes.Keys.Select(p => p.X).Max();
                var yMin = cubes.Keys.Select(p => p.Y).Min();
                var yMax = cubes.Keys.Select(p => p.Y).Max();
                var zMin = cubes.Keys.Select(p => p.Z).Min();
                var zMax = cubes.Keys.Select(p => p.Z).Max();
                for(var z = zMin - 1; z <= zMax + 1; z++)
                    for(var y = yMin - 1; y <= yMax + 1; y++)
                        for(var x = xMin - 1; x <= xMax + 1; x++)
                            yield return new Point(x, y, z);
            }

            public void NextCycle()
            {
                foreach(var c in cubes.Values.ToArray())
                {
                    // Only keep the active cubes for the next cycle.
                    if (c.Next != State.Active)
                        cubes.Remove(c.Location);
                    c.Current = c.Next;
                    c.Next = State.Inactive;
                }

                Cycle++;
            }

            public int Count => this.cubes.Count;

            public int ActiveCount => this.cubes.Count(kvp => kvp.Value.Current == State.Active);


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
                    sb.AppendLine($"z={z}");
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

            //Console.Write(g.ToString());

            while(g.Cycle < 6)
            {
                g.ApplyRules();
                g.NextCycle();
                Console.WriteLine($"... Cycle {g.Cycle} finished.");
                //Console.Write(g.ToString());
            }

            return g.ActiveCount;
        }

        
        public struct Point4
        {
            public int X {get;}
            public int Y {get;}
            public int Z {get;}
            public int W {get;}

            public Point4(int x, int y, int z, int w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }

            public IEnumerable<Point4> GetAdjacent()
            {
                for(int w = W - 1; w <= W + 1; w++)
                    for(int z = Z - 1; z <= Z + 1; z++)
                        for(int y = Y - 1; y <= Y + 1; y++)
                            for(int x = X - 1; x <= X + 1; x++)
                                if (x != X || y != Y || z != Z || w != W)
                                    yield return new Point4(x, y, z, w);
            }

            public override string ToString()
            {
                return $"({X}, {Y}, {Z}, {W}";
            }
        }

        public class Cube4
        {
            public Point4 Location {get; init;}

            public State Current {get; set;}
            public State Next {get;set;}

            public static bool IsActive(Cube4 cube) => cube.Current == State.Active;

            public override string ToString()
            {
                return $"{Location}: {Current}->{Next}";
            }
        }

        public class Game4
        {
            private Dictionary<Point4, Cube4> cubes = new Dictionary<Point4, Cube4>();
            public int Cycle {get; private set;} = 0;

            public Cube4 this[int x, int y, int z, int w]
            {
                get { return this[new Point4(x, y, z, w)]; }
            }

            public Cube4 this[Point4 p]
            {
                get
                {
                    if (!this.cubes.ContainsKey(p))
                    {
                        Cube4 cube = new Cube4 { Location = p, Current = State.Inactive, Next = State.Inactive};
                        cubes[p] = cube;
                    }

                    return cubes[p];
                }
            }

            public IEnumerable<Cube4> GetAdjacentActiveCubes(Point4 point)
            {
                foreach (var p in point.GetAdjacent())
                {
                    if (cubes.ContainsKey(p) && cubes[p].Current == State.Active)
                        yield return cubes[p];
                }
            }

            public void ApplyRules()
            {
                var points = PointsToEvaluate2().Distinct().ToArray();
                foreach (var p in points)
                {
                    Evaluate(p);
                }
            }

            public IEnumerable<Point4> PointsToEvaluate2()
            {
                foreach(var p in this.cubes.Keys)
                {
                    yield return p;
                    foreach(var a in p.GetAdjacent())
                    {
                        yield return a;
                    }
                }
            }

            private void Evaluate(Point4 p)
            {
                var c = this[p];
                var activeNeighbors = GetAdjacentActiveCubes(p).Count();
                if (c.Current == State.Active)
                {
                    // If a cube is *active* and *exactly `2` or `3`* of its
                    // neighbors are also active, the cube remains *active*.
                    // Otherwise, the cube becomes *inactive*.
                    if (activeNeighbors == 2 || activeNeighbors == 3)
                    {
                        c.Next = State.Active;
                    }
                }
                else
                {
                    // If a cube is *inactive* but *exactly `3`* of its
                    // neighbors are active, the cube becomes *active*.
                    // Otherwise, the cube remains *inactive*.
                    if (activeNeighbors == 3)
                        c.Next = State.Active;
                }
            }

            public IEnumerable<Point4> PointsToEvaluate()
            {
                var xMin = cubes.Keys.Select(p => p.X).Min();
                var xMax = cubes.Keys.Select(p => p.X).Max();
                var yMin = cubes.Keys.Select(p => p.Y).Min();
                var yMax = cubes.Keys.Select(p => p.Y).Max();
                var zMin = cubes.Keys.Select(p => p.Z).Min();
                var zMax = cubes.Keys.Select(p => p.Z).Max();
                var wMin = cubes.Keys.Select(p => p.W).Min();
                var wMax = cubes.Keys.Select(p => p.W).Max();
                for(var w = wMin - 1; w <= wMax + 1; w++)
                    for(var z = zMin - 1; z <= zMax + 1; z++)
                        for(var y = yMin - 1; y <= yMax + 1; y++)
                            for(var x = xMin - 1; x <= xMax + 1; x++)
                                yield return new Point4(x, y, z, w);
            }

            public void NextCycle()
            {
                foreach(var c in cubes.Values.ToArray())
                {
                    // Only keep the active cubes for the next cycle.
                    if (c.Next != State.Active)
                        cubes.Remove(c.Location);
                    c.Current = c.Next;
                    c.Next = State.Inactive;
                }

                Cycle++;
            }

            public int Count => this.cubes.Count;

            public int ActiveCount => this.cubes.Count(kvp => kvp.Value.Current == State.Active);

            public int MinX => cubes.Keys.Select(p => p.X).Min();
            public int MaxX => cubes.Keys.Select(p => p.X).Max();
            public int MinY => cubes.Keys.Select(p => p.Y).Min();
            public int MaxY => cubes.Keys.Select(p => p.Y).Max();
            public int MinZ => cubes.Keys.Select(p => p.Z).Min();
            public int MaxZ => cubes.Keys.Select(p => p.Z).Max();
            public int MinW => cubes.Keys.Select(p => p.W).Min();
            public int MaxW => cubes.Keys.Select(p => p.W).Max();


            public IEnumerable<Cube4> Cubes => this.cubes.Values;

            public override string ToString()
            {
                var sb = new StringBuilder();
                var xMin = MinX;
                var xMax = MaxX;
                var yMin = MinY;
                var yMax = MaxY;

                sb.AppendLine("============================");
                sb.AppendLine($"Cycle: {Cycle}");
                sb.AppendLine($"X: {xMin}-{xMax}, Y: {yMin}-{yMax}");
                sb.AppendLine();
                foreach(var w in this.cubes.Keys.Select(p => p.W).Distinct().OrderBy(v => v))
                {
                    foreach(var z in this.cubes.Keys.Select(p => p.Z).Distinct().OrderBy(v => v))
                    {
                        sb.AppendLine($"z={z}, w={w}");
                        for(var y = yMin; y <= yMax; y++)
                        {
                            for(var x = xMin; x <= xMax; x++)
                            {
                                var key = new Point4(x, y, z, w);
                                sb.Append(this.cubes.ContainsKey(key) ? '#' : '.');
                            }
                            sb.AppendLine();
                        }

                        sb.AppendLine();
                    }
                }
                sb.AppendLine("============================");

                return sb.ToString();
            }
        }        

        object PartTwo(string input) {
            var initialState = input.Lines()
                .Select(l => l.Select(c => c switch {'.' => State.Inactive, '#' => State.Active, _ => throw new Exception($"Unknown state '{c}'")}).ToArray())
                .ToArray();
            var g = new Game4();
            for(var y = 0; y < initialState.Length; y++)
            {
                var yVec = initialState[y];
                for(var x = 0; x < yVec.Length; x++)
                {
                    var s = yVec[x];
                    if (s == State.Active)
                        g[x, y, 0, 0].Current = s;
                }
            }

            Console.Write(g.ToString());

            Console.WriteLine($"... Cycle {g.Cycle} finished. There are {g.ActiveCount} cubes.");
            while(g.Cycle < 6)
            {
                g.ApplyRules();
                g.NextCycle();
                Console.Write(g.ToString());
                Console.WriteLine($"... Cycle {g.Cycle} finished. There are {g.ActiveCount} cubes.");
            }

            return g.ActiveCount;
        }
    }
}
