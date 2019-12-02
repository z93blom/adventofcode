using System.Collections.Generic;

namespace AdventOfCode
{
    public static class GraphExtensions
    {
        public static IEnumerable<TEdge> WithReverseEdge<TVertex, TEdge>(this IEnumerable<TEdge> edges)
            where TEdge : IReversibleEdge<TVertex, TEdge>
        {
            foreach (var edge in edges)
            {
                yield return edge;
                yield return edge.Reverse;
            }
        }
    }
}