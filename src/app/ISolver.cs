using System.Collections.Generic;

namespace AdventOfCode
{
    public interface ISolver {
        string GetName();
        IEnumerable<object> Solve(string input);
    }
}