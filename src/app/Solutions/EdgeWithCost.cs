namespace AdventOfCode
{
    public struct EdgeWithCost<TVertex> : IReversibleEdge<TVertex, EdgeWithCost<TVertex>>
    {
        public EdgeWithCost(TVertex source, TVertex target, int cost)
        {
            Source = source;
            Target = target;
            Cost = cost;
        }

        public TVertex Source { get; }
        public TVertex Target { get; }

        public int Cost { get; }

        public EdgeWithCost<TVertex> Reverse => new EdgeWithCost<TVertex>(this.Target, this.Source, this.Cost);
    }
}