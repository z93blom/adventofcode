using System;

namespace aoc.Puzzles.Core
{
    public struct Moment : IComparable<Moment>
    {
        public readonly int Year;
        public readonly int Day;

        public Moment(int year, int day)
        {
            Year = year;
            Day = day;
        }

        public override string ToString()
        {
            return $"{Year:0000}/{Day:00}";
        }

        public int CompareTo(Moment other)
        {
            var yearComparison = Year.CompareTo(other.Year);
            if (yearComparison != 0) return yearComparison;
            return Day.CompareTo(other.Day);
        }
    }
}