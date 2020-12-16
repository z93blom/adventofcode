using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day16 {

    class Solution : ISolver {

        public string GetName() => "Ticket Translation";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        public record Field
        {
            public string Name {get;}
            public int L1 {get;}
            public int U1 {get;}
            public int L2 {get;}
            public int U2 {get;}

            public Field(string name, int l1, int u1, int l2, int u2) 
            {
                Name = name;
                L1 = l1;
                U1 = u1;
                L2 = l2;
                U2 = u2;
            }

            public bool IsInRange(int v)
            {
                return (v >= L1 && v <= U1) || (v >= L2 && v <= U2);
            }
        }

        object PartOne(string input)
        {
            var parts = input.Split("\n\n").ToArray();
            var fields = parts[0].Lines()
                .Match(@"([\w\s]+)\: (\d+)-(\d+) or (\d+)-(\d+)")
                .Select(m => new Field(
                        m.Groups[1].Value,
                        int.Parse(m.Groups[2].Value),
                        int.Parse(m.Groups[3].Value),
                        int.Parse(m.Groups[4].Value),
                        int.Parse(m.Groups[5].Value)))
                .ToArray();
            

            var otherTickets = parts[2].Lines()
                .Skip(1)
                .Select(s => s.Split(',').Select(int.Parse).ToArray())
                .ToArray();
            
            var invalidSum = 0;

            foreach(var t in otherTickets)
            {
                foreach(var v in t)
                {
                    var valid = false;
                    foreach(var f in fields)
                    {
                        if (f.IsInRange(v))
                        {
                            valid = true;
                            break;
                        }
                    }

                    if (!valid)
                    {
                        invalidSum += v;
                    }
                }
            }
            
            return invalidSum;
        }

        object PartTwo(string input) {
            var parts = input.Split("\n\n").ToArray();
            var fields = parts[0].Lines()
                .Match(@"([\w\s]+)\: (\d+)-(\d+) or (\d+)-(\d+)")
                .Select(m => new Field(
                        m.Groups[1].Value,
                        int.Parse(m.Groups[2].Value),
                        int.Parse(m.Groups[3].Value),
                        int.Parse(m.Groups[4].Value),
                        int.Parse(m.Groups[5].Value)))
                .ToArray();
            
            var myTicket = parts[1].Lines()
                .Skip(1)
                .Select(s => s.Split(',').Select(int.Parse).ToArray())
                .First();

            var otherTickets = parts[2].Lines()
                .Skip(1)
                .Select(s => s.Split(',').Select(int.Parse).ToArray())
                .ToArray();
            
            var validTickets = new List<int[]>();

            foreach(var t in otherTickets)
            {
                var ticketIsValid = true;
                foreach(var v in t)
                {
                    var valid = false;
                    foreach(var f in fields)
                    {
                        if (f.IsInRange(v))
                        {
                            valid = true;
                            break;
                        }
                    }

                    if (!valid)
                    {
                        ticketIsValid = false;
                        break;
                    }
                }

                if (ticketIsValid)
                    validTickets.Add(t);
            }

            var possibleFieldMapper = new Dictionary<int, List<string>>();
            foreach(var field in fields)
            {
                var matchFound = false;
                for(var index = 0; index < fields.Length; index++)
                {
                    var values = validTickets.Select(t => t[index]).ToArray();
                    bool allMatches = values.All(v => field.IsInRange(v));
                    if (allMatches)
                    {
                        if (!possibleFieldMapper.ContainsKey(index))    
                            possibleFieldMapper[index] = new List<string>();
                        possibleFieldMapper[index].Add(field.Name);
                        matchFound = true;
                    }
                }

                if (!matchFound)
                {
                    //throw new Exception($"Unable to match field '{field.Name}' to any column.");
                }
            }

            var mapper = new Dictionary<int, string>();
            var changed = true;
            while(changed)
            {
                changed = false;
                var mappedValues = possibleFieldMapper.Where(kvp => kvp.Value.Count == 1).ToArray();
                foreach(var mappedValue in mappedValues)
                {
                    changed = true;
                    var fieldName = mappedValue.Value[0];
                    mapper[mappedValue.Key] = fieldName;

                    possibleFieldMapper.Remove(mappedValue.Key);

                    // Remove the value from all other occurrances
                    foreach(var p in possibleFieldMapper)
                    {
                        if (p.Value.Contains(fieldName))
                        {
                            p.Value.Remove(fieldName);
                        }
                    }

                }
            }

            var value = mapper.Where(kvp => kvp.Value.StartsWith("departure"))
                .Select(kvp => myTicket[kvp.Key])
                .Aggregate(1L, (a, v) => a*v);
            
            return value;
        }
    }
}