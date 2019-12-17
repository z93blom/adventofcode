using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace aoc.Puzzles.Core
{
    public class SolutionHandler : ISolutionHandler
    {
        private IReadOnlyDictionary<Moment, SolutionMetadata> Solutions { get; }

        public SolutionHandler()
        {
            Solutions = GatherPuzzleSolutions();
        }

        public SolutionMetadata GetMetadata(Moment moment)
        {
            if (Solutions.ContainsKey(moment))
            {
                return Solutions[moment];
            }

            return null;
        }

        public IEnumerable<Moment> Moments => Solutions.Keys;

        public async Task Solve(Moment moment, Fluent.IO.Path solutionDirectory, TextWriter output, CancellationToken cancellationToken)
        {
            if (!Solutions.ContainsKey(moment))
            {
                output.WriteLine($"{moment} does not have a solution associated with it.");
                return;
            }

            var metadata = GetMetadata(moment);
            var instance = metadata.CreateInstance();
            instance.CancellationToken = cancellationToken;

            var input = solutionDirectory
                .Combine("Input")
                .Files($"Day{moment.Day:00}.txt", false)
                .First()
                .Read(Encoding.UTF8)
                .Trim();

            await output.WriteLineAsync($"{moment}: {metadata.Title}");
            await SolvePart(1, input, output, instance.PartOne, cancellationToken);
            await SolvePart(2, input, output, instance.PartTwo, cancellationToken);
        }

        private async Task SolvePart(int partNumber, string input,TextWriter output, Func<string, ChannelWriter<string>, Task<object>> action, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<string>();
            output.WriteLine($"  Part {partNumber}:");
            var consumer = new Consumer(channel.Reader, $"    Info:", output);
            var consumerTask = consumer.ConsumeData();
            var producerTask = action(input, channel.Writer);
            await producerTask.ContinueWith(solverTask =>
            {
                channel.Writer.Complete();

                if (solverTask.IsFaulted)
                {
                    if (solverTask.Exception is AggregateException ex && ex.InnerExceptions[0] is NotImplementedException)
                    {
                        output.WriteLine("    Not implemented.");
                    }
                    else
                    {
                        output.WriteLine(solverTask.Exception);
                    }

                    return;
                }

                var result = solverTask.Result.ToString();
                output.Write($"    Solution: ");
                if (result.Contains(Environment.NewLine))
                {
                    result = Environment.NewLine + result;
                }

                output.WriteLine(result);
            }, cancellationToken);

            await consumerTask;
        }

        private static Dictionary<Moment, SolutionMetadata> GatherPuzzleSolutions()
        {
            var solutions = new Dictionary<Moment, SolutionMetadata>();
            var solutionInterface = typeof(ISolution);
            var solutionTypes = solutionInterface.Assembly.GetTypes()
                .Where(x => solutionInterface.IsAssignableFrom(x) && !x.IsAbstract)
                .ToList();

            foreach (var solutionType in solutionTypes)
            {
                var puzzleAttribute = solutionType.GetCustomAttributes(typeof(PuzzleAttribute), false).OfType<PuzzleAttribute>().First();
                var year = puzzleAttribute.Year;
                var day = puzzleAttribute.Day;
                var title = puzzleAttribute.Title;
                solutions[new Moment(year, day)] = new SolutionMetadata(solutionType, new Moment(year, day), title);
            }

            return solutions;
        }

        private class Consumer
        {
            private readonly ChannelReader<string> _reader;
            private readonly string _prefix;
            private readonly TextWriter _output;

            public Consumer(ChannelReader<string> reader, string prefix, TextWriter output)
            {
                _reader = reader;
                _prefix = prefix;
                _output = output;
            }

            public async Task ConsumeData()
            {
                var first = true;
                while (await _reader.WaitToReadAsync())
                {
                    if (_reader.TryRead(out var item))
                    {
                        if (first && !string.IsNullOrWhiteSpace(_prefix))
                        {
                            _output.WriteLine(_prefix);
                        }

                        first = false;
                        _output.WriteLine($"      {item}");
                    }
                }
            }
        }


    }
}
