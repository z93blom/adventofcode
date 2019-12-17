using System;
using System.Threading.Channels;
using aoc.Puzzles.Core;

namespace aoc.Puzzles.Test
{
    public abstract class TestBase<TSolution> where TSolution : ISolution
    {
        private readonly Channel<string> _channel;
        protected ChannelWriter<string> Writer => _channel.Writer;
        protected TSolution Solution { get; }

        protected TestBase()
        {
            Solution = Activator.CreateInstance<TSolution>();
            _channel = Channel.CreateUnbounded<string>();
        }


    }
}
