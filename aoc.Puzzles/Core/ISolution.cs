using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace aoc.Puzzles.Core
{
    public interface ISolution
    {
        event EventHandler<SolutionProgressEventArgs> ProgressUpdated;

        CancellationToken CancellationToken { get; set; }

        Task<object> PartOne(string input, ChannelWriter<string> info);

        Task<object> PartTwo(string input, ChannelWriter<string> info);
    }
}
