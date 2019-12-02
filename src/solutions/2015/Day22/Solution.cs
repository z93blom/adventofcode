using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using AdventOfCode.Y2015.Day21;

namespace AdventOfCode.Y2015.Day22 {

    class Solution : ISolver {

        public string GetName() => "Wizard Simulator 20XX";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var inputValues = input.Lines()
                .Select(s => s.Split(':'))
                .ToDictionary(a => a[0].Trim(), a => int.Parse(a[1]));
            var boss = new Mob {HitPoints = inputValues["Hit Points"], Damage = inputValues["Damage"]};
            var player = new Mob(){HitPoints = 50, Mana = 500};

            var magicMissile = new FightAction(53, f =>
            {
                boss.HitPoints -= 4;
            });
            
            var drain = new FightAction(73, f =>
            {
                boss.HitPoints -= 2;
                player.HitPoints += 2;
            });
            
            var shield = new FightAction(113, f =>
            {
                player.Armor += 7;
                f.Effects.Add(new Effect(6, turns =>
                {
                    if (turns == 0) { player.Armor -= 7;}
                }));
            });
            var poison = new FightAction(173, f =>
            {
                f.Effects.Add(new Effect(6, turns => { boss.HitPoints -= 3; }));
            });

            var recharge = new FightAction(229, f =>
            {
                f.Effects.Add(new Effect(5, turns => { player.Mana += 101; }));
            });

            var hit = new FightAction(0, f => {
                f.First.Attack(f.Second);
            });

            var fight = new Fight(player, boss);
            fight.Run(f => {
                if(f.First == boss)
                {
                    return hit;
                }

                // How do we pick the best action?
                return magicMissile;
            });

            var playerWinLose = player.HasLost ? "Lost" : "Won";
            Console.WriteLine($"Player: {playerWinLose}");

            return player.ManaSpent;
        }

        object PartTwo(string input) {
            return 0;
        }

        public class Fight
        {
            public Mob First;
            public Mob Second;

            public readonly List<Effect> Effects = new List<Effect>();

            public Fight(Mob first, Mob second)
            {
                First = first;
                Second = second;
            }

            public Mob Run(Func<Fight, FightAction> actionFunc)
            {
                while(!First.HasLost && !Second.HasLost)
                {
                    ApplyEffects();
                    var action = actionFunc(this);
                    action.Apply(this);

                    Switch();
                }

                if (First.HasLost)
                {
                    return Second;
                }
                
                return First;
            }

            private void ApplyEffects()
            {
                foreach (var effect in Effects)
                {
                    effect.Apply();
                    effect.RemainingTurns--;
                }

                foreach (var effect in Effects.Where(e => e.RemainingTurns <= 0).ToArray())
                {
                    Effects.Remove(effect);
                }
            }

            private void Switch()
            {
                var temp = First;
                First = Second;
                Second = temp;
            }
        }

        public class Effect
        {
            private readonly Action<int> _action;

            public Effect(int turns, Action<int> action)
            {
                RemainingTurns = turns;
                _action = action;
            }

            public int RemainingTurns { get; set; }

            public void Apply()
            {
                _action(RemainingTurns--);
            }
        }

        public class FightAction
        {
            private readonly int _manaCost;
            private readonly Action<Fight> _action;

            public FightAction(int manaCost, Action<Fight> action)
            {
                _manaCost = manaCost;
                _action = action;
            }

            public void Apply(Fight fight)
            {
                fight.First.Mana -= _manaCost;
                fight.First.ManaSpent += _manaCost;
                _action(fight);
            }
        }
    }
}