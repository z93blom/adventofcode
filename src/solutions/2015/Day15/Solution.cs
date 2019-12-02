using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCode.Y2015.Day15 {

    class Solution : ISolver {

        public string GetName() => "Science for Hungry People";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            return Solve(input, null);
        }

        private object Solve(string input, int? calories)
        {
            var ingredients = GetIngredients(input);

            var maxValue = 0L;
            foreach (var amounts in CreateIngredientAmounts(100, ingredients.Length))
            {
                var properties = new int[ingredients[0].Length];

                for (var ingredientIndex = 0; ingredientIndex < ingredients.Length; ingredientIndex++)
                {
                    for (var propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
                    {
                        properties[propertyIndex] += ingredients[ingredientIndex][propertyIndex] * amounts[ingredientIndex];
                    }
                }

                if (!calories.HasValue || calories.Value == properties[properties.Length - 1])
                {
                    var value = properties.Take(properties.Length - 1).Aggregate(1L, (accumulated, v) => accumulated * Math.Max(0, v));
                    maxValue = Math.Max(value, maxValue);
                }
            }

            return maxValue;
        }

        private ImmutableArray<ImmutableArray<int>> GetIngredients(string input)
        {
            var ingredients = input
                .Lines()
                .Match(
                    @"\w+: capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)")
                .Groups()
                .Select(g => g.Elements.Select(int.Parse).ToImmutableArray())
                .ToImmutableArray();

            return ingredients;
        }

        object PartTwo(string input) {
            return Solve(input, 500);
        }

        IEnumerable<int[]> CreateIngredientAmounts(int totalAmount, int numberOfIngredients)
        {
            if (numberOfIngredients == 1)
            {
                yield return new[] {totalAmount};
            }
            else
            {
                for (var amount = 0; amount <= totalAmount; ++amount)
                {
                    foreach (var otherIngredients in CreateIngredientAmounts(totalAmount - amount, numberOfIngredients - 1))
                    {
                        yield return otherIngredients.Append(amount).ToArray();
                    }
                }
            }
        }
    }
}