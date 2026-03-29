using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day21 : AbstractDaySolver<IReadOnlyList<Food>>
{
    protected override IReadOnlyList<Food> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var bracketIndex = line.IndexOf('(');
                var ingredients = line[..(bracketIndex - 1)].Split(' ').ToImmutableHashSet();
                var allergens = line[(bracketIndex + 10)..^1].Split([',', ' '], StringSplitOptions.RemoveEmptyEntries).ToImmutableHashSet();
                return new Food(ingredients, allergens);
            })
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<Food> foods)
    {
        var foodsByAllergen = new Dictionary<string, HashSet<Food>>();
        foreach (var food in foods)
        {
            foreach (var allergen in food.Allergens)
            {
                foodsByAllergen.TryAdd(allergen, []);
                foodsByAllergen[allergen].Add(food);
            }
        }

        var possibleIngredientsByAllergen = new Dictionary<string, HashSet<string>>();
        foreach ((string allergen, HashSet<Food> allergenFoods) in foodsByAllergen)
        {
            foreach (var food in allergenFoods)
            {
                possibleIngredientsByAllergen.TryAdd(allergen, food.Ingredients.ToHashSet());
                possibleIngredientsByAllergen[allergen].IntersectWith(food.Ingredients);
            }
        }

        var allergenContainingIngredients = new HashSet<string>();
        foreach ((string allergen, HashSet<string> ingredients) in possibleIngredientsByAllergen)
        {
            allergenContainingIngredients.UnionWith(ingredients);
        }

        var total = foods.Sum(food => food.Ingredients.Count(ingredient => !allergenContainingIngredients.Contains(ingredient)));

        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<Food> foods)
    {
        var possibleIngredientsByAllergen = new Dictionary<string, HashSet<string>>();
        var foodsByAllergen = new Dictionary<string, HashSet<Food>>();
        foreach (var food in foods)
        {
            foreach (var allergen in food.Allergens)
            {
                foodsByAllergen.TryAdd(allergen, []);
                foodsByAllergen[allergen].Add(food);
            }
        }

        foreach ((string allergen, HashSet<Food> f) in foodsByAllergen)
        {
            foreach (var food in f)
            {
                possibleIngredientsByAllergen.TryAdd(allergen, food.Ingredients.ToHashSet());
                possibleIngredientsByAllergen[allergen].IntersectWith(food.Ingredients);
            }
        }

        var dangerousIngredients = new List<(string allergen, string ingredient)>();
        while (possibleIngredientsByAllergen.Count > 0)
        {
            foreach ((string allergen, HashSet<string> ingredients) in possibleIngredientsByAllergen)
            {
                if (ingredients.Count == 1)
                {
                    var offendingIngredient = ingredients.First();
                    dangerousIngredients.Add((allergen, offendingIngredient));
                    foreach (var ingredientsToRemoveFrom in possibleIngredientsByAllergen.Values)
                    {
                        ingredientsToRemoveFrom.Remove(offendingIngredient);
                    }
                    possibleIngredientsByAllergen.Remove(allergen);
                    break;
                }
            }
        }

        dangerousIngredients.Sort();
        return string.Join(",", dangerousIngredients.Select(t => t.ingredient));
    }
}

internal sealed class Food(ImmutableHashSet<string> ingredients, ImmutableHashSet<string> allergens)
{
    public ImmutableHashSet<string> Ingredients { get; } = ingredients;
    public ImmutableHashSet<string> Allergens { get; } = allergens;
}