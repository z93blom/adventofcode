using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public static class StringExtensions
    {
        public static IEnumerable<int> ParseNumbers(this string t)
        {
            var position = 0;
            while (position < t.Length)
            {
                if (char.IsDigit(t[position]) || 
                    t[position] == '-' && char.IsDigit(t[position + 1]))
                {
                    var start = position;
                    position += 1;
                    while (char.IsDigit(t[position]))
                    {
                        position++;
                    }

                    yield return int.Parse(t.Substring(start, position - start));
                }
                else
                {
                    position++;
                }
            }
        }

        public static string Replace(this string text, int index, int length, string replacement)
        {
            var builder = new StringBuilder();
            builder.Append(text.Substring(0, index));
            builder.Append(replacement);
            builder.Append(text.Substring(index + length));
            return builder.ToString();
        }

        public static string ReplaceAll(this MatchCollection matches, string source, string replacement)
        {
            foreach (var match in matches.Cast<Match>())
            {
                source = match.Replace(source, replacement);
            }

            return source;
        }

        public static string Replace(this Match match, string source, string replacement)
        {
            return source.Substring(0, match.Index) + replacement + source.Substring(match.Index + match.Length);
        }
    }
}