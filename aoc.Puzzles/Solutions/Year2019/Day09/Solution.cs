using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using aoc.Puzzles.Core;
#pragma warning disable 1998

namespace aoc.Puzzles.Solutions.Year2019.Day09 
{
    [Puzzle("Sensor Boost", 2019, 9)]
    class Solution : SolutionBase
    {
        public override async Task<object> PartOne(string input, ChannelWriter<string> output)
        {
            var computer = new IntCode(input);
            computer.ProvideInput(1);
            computer.Run();

            await output.WriteAsync($"VM Outputs: {string.Join(", ", computer.AllOutputs)}", CancellationToken);
            return computer.ReadOutput().ToString();
        }

        public override async Task<object> PartTwo(string input, ChannelWriter<string> info)
        {
            throw new NotImplementedException();
        }
    }
}