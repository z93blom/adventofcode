using System.Threading.Tasks;

namespace aoc.Puzzles.Core
{
    public interface IProgressPublisher
    {
        bool IsUpdateProgressNeeded();
        Task UpdateProgressAsync(double current, double total);
    }
}
