using AdventOfCode.Model;

namespace AdventOfCode.Generator
{

    public class SolutionTemplateGenerator {
        public string Generate(Problem problem) {
            return $@"using System;
                 |using System.Collections.Generic;
                 |using System.Collections.Immutable;
                 |using System.Linq;
                 |using System.Text.RegularExpressions;
                 |using System.Text;
                 |using AdventOfCode.Utilities;
                 |
                 |namespace AdventOfCode.Y{problem.Year}.Day{problem.Day.ToString("00")} {{
                 |
                 |    class Solution : ISolver {{
                 |
                 |        public string GetName() => ""{problem.Title}"";
                 |
                 |        public IEnumerable<object> Solve(string input) {{
                 |            yield return PartOne(input);
                 |            yield return PartTwo(input);
                 |        }}
                 |
                 |        object PartOne(string input) {{
                 |            return 0;
                 |        }}
                 |
                 |        object PartTwo(string input) {{
                 |            return 0;
                 |        }}
                 |    }}
                 |}}".StripMargin();
        }
    }
}