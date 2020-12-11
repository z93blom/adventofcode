using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day08 {

    class Solution : ISolver {

        public string GetName() => "Matchsticks";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        int PartOne(string input) {
            var result = input.Lines()
                 .Select(l => new { Text = l, Escaped = Escape(l) }).ToArray()
                 .Select(t => t.Text.Length - t.Escaped.Length)
                 .Sum();
            return result;
        }

        private int EncodedLength(string l)
        {
            var length = 2; // The starting and ending double quotes.
            foreach(var c in l)
            {
                switch(c)
                {
                    case '"' :
                    case '\\' :
                        length += 2;
                        break;
                    default :
                        length += 1;
                        break;
                }
            }

            return length;
        }

        private string Escape(string l)
        {
            var sb = new StringBuilder();
            var index = 0;
            while(index < l.Length)
            {
                switch(l[index])
                {
                    case '\\' :
                        index++;
                        switch(l[index])
                        {
                            case '\\' :
                                sb.Append('\\');
                                index++;
                                break;
                            case '"' :
                                sb.Append('"');
                                index++;
                                break;
                            case 'x' :
                                // Read the two next characters.
                                index++;
                                var asciiCode = Convert.ToByte(l.Substring(index, 2), 16);
                                index+= 2;
                                sb.Append(Encoding.ASCII.GetString(new[] { asciiCode }));
                                break;
                            default:
                                throw new Exception("Unknown escape sequence");
                        }
                        break;
                    case '"':
                        index++;
                        break;
                    default:
                        sb.Append(l[index++]);
                        break;
                }
            }

            var escaped = sb.ToString();
            return escaped;
        }

        object PartTwo(string input) {
             var result = input.Lines()
                 .Select(l => EncodedLength(l) - l.Length)
                 .Sum();
            return result;
       }
    }
}