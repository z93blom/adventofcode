using aoc.ConsoleApp.Model;

namespace aoc.ConsoleApp.Generators 
{
    public static class SolutionTemplateGenerator {
        public static string Generate(Problem problem) 
        {
            return $@"using System;
                 |using System.Collections.Generic;
                 |using System.IO;
                 |using System.Linq;
                 |using System.Text.RegularExpressions;
                 |using System.Text;
                 |using System.Threading;
                 |using System.Threading.Channels;
                 |using System.Threading.Tasks;
                 |using aoc.Puzzles.Core;
                 |#pragma warning disable 1998 // Async method lacks await operators.
                 |
                 |using aoc.Puzzles.Core;
                 |
                 |namespace aoc.Puzzles.Solutions.Year{problem.Year:0000}.Day{problem.Day:00} 
                 |{{
                 |    [Puzzle(""{problem.Title}"", {problem.Year}, {problem.Day})]
                 |    class Solution : SolutionBase
                 |    {{
                 |        public override async Task<string> PartOne(string input)
                 |        {{
                 |            throw new NotImplementedException();
                 |        }}
                 |
                 |        public override async Task<string> PartTwo(string input)
                 |        {{
                 |            throw new NotImplementedException();
                 |        }}
                 |    }}
                 |}}".StripMargin();
        }
    }
}