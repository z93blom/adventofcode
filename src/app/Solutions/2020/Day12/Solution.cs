using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day12 {

    class Solution : ISolver {

        public string GetName() => "Rain Risk";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var instructions = input.Lines()
                .Match(@"(\w)(\d+)")
                .Select(m => new {Direction = m.Groups[1].Value, Distance = int.Parse(m.Groups[2].Value) })
                .ToArray();

            var facing = 0;
            var x = 0;
            var y = 0;
            foreach(var i in instructions)
            {
                var d = i.Direction;
                if (d == "F")
                {
                    d = facing switch{ 0 => "E", 1 => "N", 2 => "W", 3 => "S", _ => throw new Exception($"Unexpected facing: {facing}") };
                }

                switch(d)
                {
                    case "N" :
                        y += i.Distance;
                        break;
                    case "S" :
                        y -= i.Distance;
                        break;
                    case "E" :
                        x += i.Distance;
                        break;
                    case "W" :
                        x -= i.Distance;
                        break;
                    case "L" :
                        facing += (i.Distance / 90);
                        facing = facing % 4;
                        break;
                    case "R" :
                        facing = facing - (i.Distance / 90) + 4;
                        facing = facing % 4;
                        break;
                    default :
                        throw new Exception($"Unexpected direction {d}");
                }


            }
            
            return Math.Abs(x) + Math.Abs(y);
        }

        object PartTwo(string input) {
            var instructions = input.Lines()
                .Match(@"(\w)(\d+)")
                .Select(m => new {Direction = m.Groups[1].Value, Distance = int.Parse(m.Groups[2].Value) })
                .ToArray();

            var x = 0;
            var y = 0;
            var wx = 10;
            var wy = 1;
            foreach(var i in instructions)
            {
                switch(i.Direction)
                {
                    case "N" :
                        wy += i.Distance;
                        break;
                    case "S" :
                        wy -= i.Distance;
                        break;
                    case "E" :
                        wx += i.Distance;
                        break;
                    case "W" :
                        wx -= i.Distance;
                        break;
                    case "L" :
                    case "R" :
                        (wx, wy) = RotateAroundOrigo(i.Direction == "R", i.Distance, wx, wy);
                        break;
                    case "F":
                        x += wx * i.Distance;
                        y += wy * i.Distance;
                        break;
                    default :
                        throw new Exception($"Unexpected direction {i.Direction}");
                }
            }
            
            return Math.Abs(x) + Math.Abs(y);        
        }

        private (int x, int y) RotateAroundOrigo(bool clockWise, int angle, int wx, int wy)
        {
            angle = clockWise ? -angle : angle;
            var s = Math.Sin((Math.PI / 180) * angle);
            var c = Math.Cos((Math.PI / 180) * angle);
            var x = (int)Math.Round(wx * c - wy * s);
            var y = (int)Math.Round(wx * s + wy * c);

            return (x, y);
        }
    }
}