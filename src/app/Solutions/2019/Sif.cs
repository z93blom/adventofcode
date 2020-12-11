using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode.Y2019
{
    /// <summary>
    /// An image saved as SIF (Space Image Format)
    /// </summary>
    public class Sif
    {
        public enum Color
        {
            Black = 0,
            White = 1,
            Transparent = 2,
        }

        private readonly Dictionary<Point, Color> _colors = new Dictionary<Point, Color>();

        private Sif Move(Point origin)
        {
            var bounds = Bounds;
            if (bounds.X == origin.X && bounds.Y == origin.Y)
            {
                return this;
            }

            var sif = new Sif();

            for (var yOffset = 0; yOffset < bounds.Width; yOffset++)
            {
                for (var xOffset = 0; xOffset <= bounds.Height; xOffset++)
                {
                    sif.SetColor(new Point(xOffset + origin.X, yOffset + origin.Y), this.GetColor(new Point(bounds.X + xOffset, bounds.Y + yOffset)));
                }
            }

            return sif;
        }

        public void Draw(TextWriter writer)
        {
            var bounds = Bounds;
            var minX = bounds.X;
            var maxX = bounds.X + bounds.Width - 1;
            var minY = bounds.Y;
            var maxY = bounds.Y + bounds.Width - 1;
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    writer.Write(GetColorAsChar(new Point(x, y)));
                }

                writer.WriteLine();
            }
        }

        private char GetColorAsChar(Point point)
        {
            var color = GetColor(point);
            switch (color)
            {
                case Color.Black:
                    return ' ';
                case Color.White:
                    return '█';
                case Color.Transparent:
                    return ' ';
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Color GetColor(Point point)
        {
            if (!_colors.ContainsKey(point))
            {
                return Color.Transparent;
            }

            return _colors[point];
        }

        public void SetColor(Point point, Color color)
        {
            _colors[point] = color;
        }

        public Rectangle Bounds
        {
            get
            {
                var minX = _colors.Keys.Min(p => p.X);
                var maxX = _colors.Keys.Max(p => p.X);
                var minY = _colors.Keys.Min(p => p.Y);
                var maxY = _colors.Keys.Max(p => p.Y);
                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
        }


        /// <summary>
        /// OCR:s the image, and returns the resulting string.
        /// </summary>
        /// <returns></returns>
        public string OCR()
        {
            const int characterWidth = 5;
            const int characterHeight = 6;
            var sif = this.Move(Point.Empty);

            var bounds = sif.Bounds;
            if (bounds.Height + 1 != characterHeight)
            {
                return string.Empty;
            }

            var numberOfCharacters = (bounds.Width + 1) / characterWidth;
            var characters = new char[numberOfCharacters];
            for (var characterIndex = 0; characterIndex < numberOfCharacters; characterIndex++)
            {
                var hash = 0L;
                var bit = 0;
                for (var y = 0; y < characterHeight; y++)
                {
                    for (var x = 0; x < characterWidth; x++)
                    {
                        var color = sif.GetColor(new Point(x + characterIndex * characterWidth, y));
                        hash += (color == Color.White ? 1 : 0) << bit++;
                    }
                }

                characters[characterIndex] = GetChar(hash);
            }

            return new string(characters);
        }

        private char GetChar(long hash)
        {
            switch (hash)
            {
                case 138553905: return 'Y';
                case 244620583: return 'B';
                case 311737641: return 'H';
                case 504398881: return 'L';
                case 504434959: return 'Z';
                default: Console.WriteLine($"Unknown character hash: {hash}."); return '?';
            }
        }
    }
}