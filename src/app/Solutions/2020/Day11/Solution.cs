using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using System.Collections;
using System.IO;

namespace AdventOfCode.Y2020.Day11 {

    class Solution : ISolver {

        public string GetName() => "Seating System";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        private enum Seating
        {
            Floor,
            Empty,
            Occupied,
        }

        object PartOne(string input) {
            var grid = Grid<Seating>.FromString(input, ToSeating);

            var iteration = 0;
            // grid.WriteTo(Console.Out, ToChar);
            while(true)
            {
                var changes = GetChanges(grid).ToArray();
                if (changes.Length == 0)
                    break;
                
                foreach(var change in changes)
                {
                    grid[change.X, change.Y] = change.Value;
                }

                iteration++;
                if (iteration % 100 == 0)
                {
                    // Console.Out.WriteLine("");
                    // Console.Out.WriteLine($"After {iteration}:");
                    // grid.WriteTo(Console.Out, ToChar);
                }
            }

            return grid.Count(gp => gp.Value == Seating.Occupied);
        }

        private char ToChar(Seating seating)
        {
            return seating switch 
            {
                 Seating.Floor => '.',
                 Seating.Empty => 'L',
                 Seating.Occupied => '#',
                 _ => throw new Exception("Unexpected input"),
            };          
        }
        private Seating ToSeating(char c)
        {
            return c switch 
            {
                 '.' => Seating.Floor,
                 'L' => Seating.Empty,
                 '#' => Seating.Occupied,
                 _ => throw new Exception("Unexpected input"),
            };          
        }



        object PartTwo(string input) {
            var grid = Grid<Seating>.FromString(input, ToSeating);

            var iteration = 0;
            while(true)
            {
                var changes = GetChangesPart2(grid).ToArray();
                if (changes.Length == 0)
                    break;
                
                foreach(var change in changes)
                {
                    grid[change.X, change.Y] = change.Value;
                }

                iteration++;
                if (iteration % 100 == 0)
                {
                    // Console.Out.WriteLine("");
                    // Console.Out.WriteLine($"After {iteration}:");
                    // grid.WriteTo(Console.Out, ToChar);
                }
            }

            return grid.Count(gp => gp.Value == Seating.Occupied);

        }

        private IEnumerable<GridPoint<Seating>> GetChanges(Grid<Seating> grid)
        {
            foreach(var gp in grid)
            {
                var adjacent = gp.GetAdjacent(grid).ToArray();

                // If a seat is *empty* (`L`) and there are *no* occupied seats adjacent to it, the seat becomes *occupied*.
                if(gp.Value == Seating.Empty)
                {
                    if (!adjacent.Any(p => p.Value == Seating.Occupied))
                    {
                        yield return new GridPoint<Seating>(gp.X, gp.Y, Seating.Occupied);
                    }
                }

                // If a seat is *occupied* (`#`) and *four or more* seats adjacent to it are also occupied, the seat becomes *empty*.
                if(gp.Value == Seating.Occupied)
                {
                    if (adjacent.Count(p => p.Value == Seating.Occupied) >= 4)
                    {
                        yield return new GridPoint<Seating>(gp.X, gp.Y, Seating.Empty);
                    }
                }
            }
        }

        private IEnumerable<GridPoint<Seating>> GetChangesPart2(Grid<Seating> grid)
        {
            foreach(var gp in grid)
            {
                var adjacent = gp.GetAdjacentSeats(grid, s => s == Seating.Floor).ToArray();

                // If a seat is *empty* (`L`) and there are *no* occupied seats adjacent to it, the seat becomes *occupied*.
                if(gp.Value == Seating.Empty)
                {
                    if (!adjacent.Any(p => p.Value == Seating.Occupied))
                    {
                        yield return new GridPoint<Seating>(gp.X, gp.Y, Seating.Occupied);
                    }
                }

                // If a seat is *occupied* (`#`) and it now takes *five or more* visible occupied seats for an occupied seat to become empty
                if(gp.Value == Seating.Occupied)
                {
                    if (adjacent.Count(p => p.Value == Seating.Occupied) >= 5)
                    {
                        yield return new GridPoint<Seating>(gp.X, gp.Y, Seating.Empty);
                    }
                }
            }
        }        

        public struct GridPoint<T>
        {
            public int X { get; }
            public int Y { get; }
            public T Value { get; }

            public GridPoint(int x, int y, T value)
            {
                X = x;
                Y = y;
                Value = value;
            }

            public IEnumerable<GridPoint<T>> GetAdjacent(Grid<T> grid)
            {
                yield return new GridPoint<T>(X-1, Y-1, grid[X-1, Y-1]);
                yield return new GridPoint<T>(X  , Y-1, grid[X  , Y-1]);
                yield return new GridPoint<T>(X+1, Y-1, grid[X+1, Y-1]);
                yield return new GridPoint<T>(X-1, Y  , grid[X-1, Y  ]);
                yield return new GridPoint<T>(X+1, Y  , grid[X+1, Y  ]);
                yield return new GridPoint<T>(X-1, Y+1, grid[X-1, Y+1]);
                yield return new GridPoint<T>(X  , Y+1, grid[X  , Y+1]);
                yield return new GridPoint<T>(X+1, Y+1, grid[X+1, Y+1]);
            }

            public IEnumerable<GridPoint<T>> GetAdjacentSeats(Grid<T> grid, Func<T, bool> isFloor)
            {
                yield return GetAdjacentSeat(grid, -1, -1, isFloor);
                yield return GetAdjacentSeat(grid,  0, -1, isFloor);
                yield return GetAdjacentSeat(grid, +1, -1, isFloor);
                yield return GetAdjacentSeat(grid, -1,  0, isFloor);
                yield return GetAdjacentSeat(grid, +1,  0, isFloor);
                yield return GetAdjacentSeat(grid, -1, +1, isFloor);
                yield return GetAdjacentSeat(grid,  0, +1, isFloor);
                yield return GetAdjacentSeat(grid, +1, +1, isFloor);
            }   

            private GridPoint<T> GetAdjacentSeat(Grid<T> grid, int deltaX, int deltaY, Func<T, bool> isFloor)
            {
                var x = X + deltaX;
                var y = Y + deltaY;
                while(true)
                {
                    var value = grid[x, y];
                    if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height || !isFloor(value))
                    {
                        return new GridPoint<T>(x, y, value);;
                    }

                    x += deltaX;
                    y += deltaY;
                }
            }         

        }

        public class Grid<T> : IEnumerable<GridPoint<T>>
        {
            private T[,] _grid;

            private Grid(T[,] grid)
            {
                _grid = grid;
            }

            public Grid<T> Clone()
            {
                _grid = new T[Width, Height];
                foreach(var gp in this)
                {
                    _grid[gp.X, gp.Y] = gp.Value;
                }

                return new Grid<T>(_grid);
            }

            public static Grid<T> FromString(string s, Func<char, T> mapper)
            {
                var lines = s.Lines().ToArray();
                var grid = new T[lines[0].Length, lines.Length];
                int y = 0;
                foreach(var line in lines)
                {
                    var x = 0;
                    foreach(var c in line)
                    {
                        grid[x, y] = mapper(c);
                        x++;
                    }

                    y++;
                }

                return new Grid<T>(grid);
            }

            public IEnumerator<GridPoint<T>> GetEnumerator()
            {
                foreach(var x in Enumerable.Range(0, Width))
                    foreach(var y in Enumerable.Range(0, Height))
                        yield return new GridPoint<T>(x, y, _grid[x, y]);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public T this[int x, int y]
            {
                get 
                {
                    // If "out of bounds, just return the default value for T
                    if (x < 0 || x >= Width || y < 0 || y >= Height)
                    {
                        return default(T);
                    }

                    return _grid[x,y]; 
                }
                set { _grid[x,y] = value; }
            }
            
            public int Width => _grid.GetLength(0);

            public int Height => _grid.GetLength(1);


            public void WriteTo(TextWriter w, Func<T, char> mapper)
            {
                for(var y = 0; y < this.Height; y++)
                {
                    for(var x = 0; x < this.Width; x++)
                    {
                        w.Write(mapper(this[x,y]));
                    }

                    w.WriteLine();
                }
            }
        }
    }
}