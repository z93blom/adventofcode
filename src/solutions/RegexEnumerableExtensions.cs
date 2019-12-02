using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public static class RegexEnumerableExtensions
    {
        public static IEnumerable<string> Groups(this Match match)
        {
            return match.Groups.Cast<Group>().Skip(1).Select(g => g.Value);
        }

        public static IEnumerable<Grouping<string, string>> Groups(this IEnumerable<Match> matches)
        {
            return matches.Select(match => new Grouping<string, string>(match.Value, match.Groups()));
        }

        public static IEnumerable<Match> Match(this IEnumerable<string> text, string pattern)
        {
            var regex = new Regex(pattern);

            foreach (var t in text)
            {
                yield return regex.Match(t);
            }
        }

        public static IEnumerable<IEnumerable<Match>> Matches(this IEnumerable<string> text, string pattern)
        {
            var regex = new Regex(pattern);

            foreach (var t in text)
            {
                yield return regex.Matches(t).Cast<Match>();
            }
        }
    }
}