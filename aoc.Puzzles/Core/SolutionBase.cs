using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using aoc.Puzzles.Solutions;

namespace aoc.Puzzles.Core
{
    public abstract class SolutionBase : ISolution, IProgressPublisher
    {
        public event EventHandler<SolutionProgressEventArgs> ProgressUpdated;

        public int MillisecondsBetweenProgressUpdates { get; set; } = 200;

        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public abstract Task<object> PartOne(string input, ChannelWriter<string> output);

        public abstract Task<object> PartTwo(string input, ChannelWriter<string> info);

        /// <summary>
        /// Breaks the input into lines and removes empty lines at the end.
        /// </summary>
        public static List<string> GetLines(string input)
        {
            return input.Lines().ToList();
        }

        bool IProgressPublisher.IsUpdateProgressNeeded() => IsUpdateProgressNeeded();

        Task IProgressPublisher.UpdateProgressAsync(double current, double total) => UpdateProgressAsync(current, total);

        protected virtual SolutionProgress Progress { get; set; } = new SolutionProgress();

        /// <summary>
        /// Returns true if <see cref="UpdateProgressAsync"/> should be called to update the UI of the solution runner. This happens every couple of milliseconds.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsUpdateProgressNeeded() => Environment.TickCount >= myUpdateTick;

        /// <summary>
        /// Updates the UI of the solution runner with the current progress, and schedules the next update a couple of milliseconds in the future.
        /// </summary>
        protected Task UpdateProgressAsync(double current, double total)
        {
            Progress.Percentage = current / Math.Max(total, double.Epsilon) * 100;
            return UpdateProgressAsync();
        }

        /// <summary>
        /// Updates the UI of the solution runner with the current progress, and schedules the next update a couple of milliseconds in the future.
        /// </summary>
        protected Task UpdateProgressAsync()
        {
            myUpdateTick = Environment.TickCount + MillisecondsBetweenProgressUpdates;
            ProgressUpdated?.Invoke(this, new SolutionProgressEventArgs(Progress));
            return Task.Delay(1, CancellationToken.None);
        }

        /// <summary>
        /// A scheduled tick from <see cref="Environment.TickCount"/>, when a progress update should happen.
        /// </summary>
        private int myUpdateTick = Environment.TickCount;
    }
}
