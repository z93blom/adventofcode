using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc.Puzzles.Core
{
    public static class StringExtensions
    {
        private static readonly Regex _intRegex = new Regex(@"([+-]?\d+)", RegexOptions.Compiled);

        public static IEnumerable<string> Lines(this string s, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return s.Split(new[] {'\r', '\n'}, options);
        }

        public static IEnumerable<int> Integers(this string s)
        {
            var matches = _intRegex.Matches(s);
            return matches.Cast<Match>().SelectMany(m => m.Captures.Cast<Capture>().Select(v => int.Parse(v.Value)));
        }
    }
}