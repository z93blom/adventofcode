using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day21 {

    class Solution : ISolver {

        public string GetName() => "RPG Simulator 20XX";

        private Mob Fight(Mob first, Mob second)
        {
            var attacker = first;
            var defender = second;

            while (true)
            {
                attacker.Attack(defender);
                if (defender.IsDead)
                {
                    return attacker;
                }

                var temp = defender;
                defender = attacker;
                attacker = temp;
            }
        }

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        private IEnumerable<EquipmentSet> GetAllSets()
        {
            var weapons = new[]
            {
                //new Equipment(0, 0, 0, "No weapon"),
                new Equipment(8, 4, 0, "Dagger"),
                new Equipment(10, 5, 0, "Shortsword"),
                new Equipment(25, 6, 0, "Warhammer"),
                new Equipment(40, 7, 0, "Longsword"),
                new Equipment(74, 8, 0, "Greataxe"),
            };

            var armors = new[]
            {
                new Equipment(0, 0, 0, "No armor"),
                new Equipment(13, 0, 1, "Leather"),
                new Equipment(31, 0, 2, "Chainmail"),
                new Equipment(53, 0, 3, "Splintmail"),
                new Equipment(75, 0, 4, "Bandedmail"),
                new Equipment(102, 0, 5, "Platemail"),
            };

            var rings = new[]
            {
                new Equipment(0, 0, 0, "No ring 1"),
                new Equipment(0, 0, 0, "No ring 2"),
                new Equipment(25, 1, 0, "Damage +1"),
                new Equipment(50, 2, 0, "Damage +2"),
                new Equipment(100, 3, 0, "Damage +3"),
                new Equipment(20, 0, 1, "Defense +1"),
                new Equipment(40, 0, 2, "Defense +2"),
                new Equipment(80, 0, 3, "Defense +3"),
            };

            foreach (var ringPairs in rings.Permutations(2).Select(ie => ie.ToArray()))
            {
                foreach (var weapon in weapons)
                {
                    foreach (var armor in armors)
                    {
                        //sets.Add(new EquipmentSet(weapon, armor, ringPairs[0], ringPairs[1]));
                        yield return new EquipmentSet(weapon, armor, ringPairs[0], ringPairs[1]);
                    }
                }
            }
        }

        object PartOne(string input) {
            var inputValues = input.Lines()
                .Select(s => s.Split(':'))
                .ToDictionary(a => a[0].Trim(), a => int.Parse(a[1]));

            var equipmentSets = GetAllSets().OrderBy(s => s.Cost).ToArray();
            foreach (var set in equipmentSets)
            {
                var boss = new Mob {HitPoints = inputValues["Hit Points"], Damage = inputValues["Damage"], Armor = inputValues["Armor"]};
                var player = new Mob {HitPoints = 100, Damage = 0, Armor = 0};
                set.Equip(player);
                var winner = Fight(player, boss);
                if (winner == player)
                {
                    return set.Cost;
                }
            }

            return int.MaxValue;
        }

        class EquipmentSet
        {
            public EquipmentSet(Equipment weapon, Equipment armor, Equipment leftRing, Equipment rightRing)
            {
                Weapon = weapon;
                Armor = armor;
                LeftRing = leftRing;
                RightRing = rightRing;
            }

            public int Cost => this.Weapon.Cost + this.Armor.Cost + this.LeftRing.Cost + this.RightRing.Cost;
            public Equipment Armor { get; }
            public Equipment Weapon { get; }
            public Equipment LeftRing { get; }
            public Equipment RightRing { get; }

            public void Equip(Mob mob)
            {
                Armor.Equip(mob);
                Weapon.Equip(mob);
                LeftRing.Equip(mob);
                RightRing.Equip(mob);
            }

            public override string ToString()
            {
                return $"{Cost} => Weapon: {Weapon}, Armor: {Armor}, Left: {LeftRing}, Right: {RightRing}";
            }
        }

        class Equipment
        {
            public Equipment(int cost, int damage, int armor, string name)
            {
                Cost = cost;
                Damage = damage;
                Armor = armor;
                Name = name;
            }

            public int Cost { get; set; }
            public int Damage { get; set; }
            public int Armor { get; set; }
            public string Name { get; }

            public void Equip(Mob mob)
            {
                mob.Armor += this.Armor;
                mob.Damage += this.Damage;
            }

            public override string ToString()
            {
                return this.Name;
            }
        }

        object PartTwo(string input) {
            var inputValues = input.Lines()
                .Select(s => s.Split(':'))
                .ToDictionary(a => a[0].Trim(), a => int.Parse(a[1]));

            var equipmentSets = GetAllSets().OrderByDescending(s => s.Cost).ToArray();
            foreach (var set in equipmentSets)
            {
                var boss = new Mob {HitPoints = inputValues["Hit Points"], Damage = inputValues["Damage"], Armor = inputValues["Armor"]};
                var player = new Mob {HitPoints = 100, Damage = 0, Armor = 0};
                set.Equip(player);
                var winner = Fight(player, boss);
                if (winner == boss)
                {
                    return set.Cost;
                }
            }

            return int.MaxValue;
        }
    }

    internal class Mob
    {
        public int Damage { get; set; }
        public int Armor { get; set; }
        public int HitPoints { get; set; }
        public int Mana { get; set; }
        public int ManaSpent { get; set; }

        public void Attack(Mob defender)
        {
            defender.HitPoints -= Math.Max(this.Damage - defender.Armor, 1);
        }

        public bool IsDead => this.HitPoints <= 0;
        public bool HasLost => this.HitPoints <= 0 || this.Mana < 0;


    }
}