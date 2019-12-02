using QuickGraph;

namespace AdventOfCode
{
    public struct Edge<TVertex> : IReversibleEdge<TVertex, Edge<TVertex>>
    {
        public Edge(TVertex source, TVertex target)
        {
            Source = source;
            Target = target;
        }

        public TVertex Source { get; }
        public TVertex Target { get; }

        public Edge<TVertex> Reverse => new Edge<TVertex>(this.Target, this.Source);
    }
}