using System;

namespace aoc.Puzzles.Core
{
    public sealed class SolutionMetadata
    {
        public int Year => Moment.Year;

        public int Day => Moment.Day;

        public Type Type { get; }

        public string Title { get; }

        public Moment Moment { get; }

        public SolutionMetadata(Type type, Moment moment, string name)
        {
            Type = type;
            Moment = moment;
            Title = name;
        }

        public ISolution CreateInstance()
        {
            return (ISolution)Activator.CreateInstance(Type);
        }
    }
}
