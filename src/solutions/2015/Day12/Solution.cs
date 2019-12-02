using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventOfCode.Y2015.Day12 {

    class Solution : ISolver {

        public string GetName() => "JSAbacusFramework.io";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var sum = input.ParseNumbers().Sum();
            return sum;
        }

        object PartTwo(string input)
        {
            var o = JToken.Parse(input);
            var sum = GetSum(o);
            return sum;
        }

        int GetSum(JToken o)
        {
            var sum = 0;
            switch (o.Type)
            {
                case JTokenType.Object:

                    var values = o.Values();
                    var isRed = false;
                    foreach (var value in values)
                    {
                        if (value.Type == JTokenType.String && value.Value<string>() == "red")
                        {
                            isRed = true;
                            break;
                        }
                    }

                    if (!isRed)
                    {
                        foreach (var child in o.Values())
                        {
                            sum += GetSum(child);
                        }
                    }

                    break;
                case JTokenType.Array:
                    foreach (var child in o.Children())
                    {
                        sum += GetSum(child);
                    }
                    break;

                case JTokenType.Integer:
                    sum += o.Value<int>();
                    break;
                case JTokenType.String:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return sum;
        }
    }
}