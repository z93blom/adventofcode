using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using QuickGraph;

namespace AdventOfCode.Y2019.Day14 {

    class Solution : ISolver {

        public string GetName() => "Space Stoichiometry";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var data = input.Lines()
                .Match(@"^(.*) => (\d+) (.*)$");

            var reactions = new Dictionary<string, Reaction>();
            foreach (var m in data)
            {
                var count = m.Groups.Count;
                var result = new ChemAmount(m.Groups[count - 1].Value, int.Parse(m.Groups[count - 2].Value));
                var chemAmounts = Regex.Matches(m.Groups[1].Value, @"(\d+) (\w+)")
                    .Cast<Match>()
                    .Select(m2 => m2.Groups.Cast<Group>().Skip(1).ToArray())
                    .Select(g2 => new ChemAmount(g2[1].Value, int.Parse(g2[0].Value)));

                var reaction = new Reaction(chemAmounts, result);
                reactions.Add(reaction.Result.Chemical, reaction);
            }

            // Iterate all the reactions needed to go from FUEL to only ORE.
            var chemsNeeded = new Dictionary<string, int>();
            chemsNeeded.Add("FUEL", 1);
            var oreNeeded = 0;
            var extraChemicalsWeHave = new Dictionary<string, int>();
            while(chemsNeeded.Count > 0)
            {
                // Take any chemical except ore to process.
                var chemical = chemsNeeded.Keys.First();
                var amountNeeded = chemsNeeded[chemical];

                // Look up what it take to process it.
                var reaction = reactions[chemical];

                // How many times do we have to run the reaction to get the needed amount?
                var timesToRunProcess = (int)Math.Ceiling((double)amountNeeded / reaction.Result.Amount);

                // Add all the inputs to the list of chemicals needed.
                foreach(var ca in reaction.Inputs)
                {
                    var inputsNeeded = ca.Amount * timesToRunProcess;

                    // Do we have any leftovers from previous reactions?
                    var leftOvers = extraChemicalsWeHave.ContainsKey(ca.Chemical) ? extraChemicalsWeHave[ca.Chemical] : 0;
                    if (leftOvers >= inputsNeeded)
                    {
                        // There is more then enough of the leftovers so we do not have to process any more.
                        extraChemicalsWeHave[ca.Chemical] = leftOvers - inputsNeeded;
                        continue;
                    }

                    // There isn't enough of the leftovers. Let's take what we can, and process the rest.
                    inputsNeeded -= leftOvers;
                    extraChemicalsWeHave[ca.Chemical] = 0;

                    if (ca.Chemical == "ORE")
                    {
                        oreNeeded += inputsNeeded;
                    }
                    else if (!chemsNeeded.ContainsKey(ca.Chemical))
                    {
                        chemsNeeded.Add(ca.Chemical, inputsNeeded);
                    }
                    else
                    {
                        chemsNeeded[ca.Chemical] = chemsNeeded[ca.Chemical] + inputsNeeded;
                    }
                }

                // All the chemicals that are needed to process this chemical have been added.
                // Remove the need to process this chemical.
                chemsNeeded.Remove(reaction.Result.Chemical);

                // Did we produce any extra chemicals?
                var extrasProduced = timesToRunProcess * reaction.Result.Amount - amountNeeded;
                extraChemicalsWeHave[reaction.Result.Chemical] = extraChemicalsWeHave.ContainsKey(reaction.Result.Chemical) ? extrasProduced + extraChemicalsWeHave[reaction.Result.Chemical] : extrasProduced;
            }

            // Did we have any leftovers that we did not need to process?
            // TODO! (Not needed for part 1 - or at least I got the right answer anyways.)

            return oreNeeded;
        }


        private struct Reaction
        {
            public ChemAmount Result;

            public List<ChemAmount> Inputs;

            public Reaction(IEnumerable<ChemAmount> inputs, ChemAmount result)
            {
                this.Inputs = inputs.ToList();
                this.Result = result;
            }

            public override string ToString()
            {
                return $"{string.Join(", ", Inputs)} => {Result}";
            }
        }

        private struct ChemAmount
        {
            public string Chemical { get;}
            public int Amount { get;}

            public ChemAmount(string chemical, int amount)
            {
                Chemical = chemical;
                Amount = amount;
            }

            public override string ToString()
            {
                return $"{Amount} {Chemical}";
            }
        }

        object PartTwo(string input) {
            return 0;
        }
    }
}