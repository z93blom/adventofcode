using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day08 {

    class Solution : ISolver {

        public string GetName() => "Space Image Format";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var layers = new List<string>();
            for (var index = 0; index < input.Length; index += 25*6)
            {
                layers.Add(input.Substring(index, 25 * 6));
            }

            var layer = layers.OrderBy(l => l.Count(c => c == '0'))
                .First();

            var ones = layer.Count(c => c == '1');
            var twos = layer.Count(c => c == '2');

            return ones * twos;
        }

        object PartTwo(string input) {
            var layersString = new List<string>();
            for (var index = 0; index < input.Length; index += 25 * 6)
            {
                layersString.Add(input.Substring(index, 25 * 6));
            }

            var finalImage = new StringBuilder();
            for(var index = 0; index < 25*6; index++)
            {
                var layersForThisPixel = layersString.Select(s => s[index]).ToArray();
                var color = layersForThisPixel.First(c => c != '2');
                finalImage.Append(color);
            }

            var text = finalImage.ToString();

            var sif = new Sif();
            for (var l = 0; l < 6; l++)
            {
                var line = text.Substring(25 * l, 25);
                for (var ci = 0; ci < 25; ci++)
                {
                    sif.SetColor(new Point(ci, l), line[ci] == '0' ? Sif.Color.Black : Sif.Color.White);
                }

            }

            sif.Draw(Console.Out);
            return sif.OCR();

            //for (var l = 0; l < 6; l++)
            //{
            //    var line = text.Substring(25 * l, 25);
            //    var t = line.Replace('0', ' ');
            //    t = t.Replace('1', '█');
            //    Console.WriteLine(t);
            //}
            //
            //return "ZYBLH";
        }
    }
}