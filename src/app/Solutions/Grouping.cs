using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AdventOfCode
{
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            Elements = elements.ToImmutableArray();
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return ((IEnumerable<TElement>)Elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TKey Key { get; }
        public ImmutableArray<TElement> Elements { get; }

        public TElement this[int i] => this.Elements[i];

        public override string ToString()
        {
            return $"Key: {Key}. Count {Elements.Length}";
        }
    }
}