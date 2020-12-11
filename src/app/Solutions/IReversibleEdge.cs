using QuikGraph;

namespace AdventOfCode
{
    public interface IReversibleEdge<TVertex, out TEdge> : IEdge<TVertex> where TEdge : IReversibleEdge<TVertex, TEdge>
    {
        TEdge Reverse { get; }
    }
}