using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace aoc.Puzzles.Core
{
    public interface ISolutionHandler
    {
        SolutionMetadata GetMetadata(Moment moment);
        IEnumerable<Moment> Moments { get; }
        Task Solve(Moment moment, Fluent.IO.Path solutionDirectory, TextWriter output, CancellationToken cancellationToken);
    }
}
