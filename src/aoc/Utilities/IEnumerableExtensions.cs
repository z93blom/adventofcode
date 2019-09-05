using System;
using System.Collections.Generic;

namespace AdventOfCode.Utilities
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> When<TSource, TResult>(this IEnumerable<TSource> e, params (Func<TSource, bool> match, Func<TSource, TResult> transformer)[] converters)
        {
            foreach(var v in e)
            {
                foreach(var converter in converters)
                {
                    if (converter.match(v))
                    {
                        yield return converter.transformer(v);
                        break;
                    }
                }
            }
        }

        public static IEnumerable<TResult> When<TSource, TIntermediate, TResult>(this IEnumerable<TSource> e, params (Func<TSource, (bool isMatch, TIntermediate intermediate)> match, Func<TIntermediate, TResult> transformer)[] converters)
        {
            foreach(var v in e)
            {
                foreach(var converter in converters)
                {
                    (var isMatch, var intermediate) = converter.match(v);
                    if (isMatch)
                    {
                        yield return converter.transformer(intermediate);
                        break;
                    }
                }
            }
        }
    }
}