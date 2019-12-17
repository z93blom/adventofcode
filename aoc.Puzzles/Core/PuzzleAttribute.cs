using System;

namespace aoc.Puzzles.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PuzzleAttribute : Attribute
    {
        public int Year { get; }

        public int Day { get; }

        public string Title { get; }

        public PuzzleAttribute(string title, int year, int day)
        {
            Title = title;
            Year = year;
            Day = day;
        }
    }
}
