using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day04 {

    class Solution : ISolver {

        public string GetName() => "Passport Processing";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) {
            var requiredFields = new [] {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
            var fields = new Dictionary<string, string>();
            var validPassports = 0;
            foreach(var row in input.Split('\n'))
            {
                if (row.Length == 0)
                {
                    // Record is complete. Does it include all required fields?
                    validPassports += requiredFields.All(s => fields.ContainsKey(s)) ? 1 : 0;
                    fields.Clear();
                }
                else
                {
                    foreach(var field in row.Split(' '))
                    {
                        var kvp = field.Split(':');
                        fields.Add(kvp[0], kvp[1]);
                    }
                }
            }

            if (fields.Count > 0)
            {
                validPassports += requiredFields.All(s => fields.ContainsKey(s)) ? 1 : 0;
            }

            return validPassports;
        }

        object PartTwo(string input) {


            bool Year(string s, int low, int high)
            {
                bool v = s.Length == 4 &&
                        int.TryParse(s, out var i) &&
                        i >= low &&
                        i <= high;
                return v;
            }

            bool Hgt(string s)
            {
                var regex = new Regex(@"(\d+)(cm|in)");
                if (!regex.IsMatch(s))
                {
                    return false;
                }
                
                var m = regex.Match(s);
                var l = int.Parse(m.Groups[1].Value);
                if (m.Groups[2].Value == "cm")
                    return l >= 150 && l <= 193;
                return l >= 59 && l <= 76;
            };

            bool Hcl(string s)
            {
                var regex = new Regex(@"\#[0-9a-f]{6}");
                bool v = regex.IsMatch(s);
                return v;
            };

            bool Ecl(string s)
            {
                var regex = new Regex(@"amb|blu|brn|gry|grn|hzl|oth");
                bool v = regex.IsMatch(s);
                return v;
            };

            bool Pid(string s)
            {
                var regex = new Regex(@"[0-9]{9}");
                bool v = regex.IsMatch(s);
                return v;
            };

            var validation = new Dictionary<string, Func<string, bool>>()
            {
                {"byr", s => Year(s, 1920, 2002)},
                {"iyr", s => Year(s, 2010, 2020)},
                {"eyr", s => Year(s, 2020, 2030)},
                {"hgt", Hgt},
                {"hcl", Hcl},
                {"ecl", Ecl},
                {"pid", Pid},
                //{"cid", s => true},
            };
            var fields = new Dictionary<string, string>();
            var validPassports = 0;
            foreach(var row in input.Split('\n'))
            {
                if (row.Length == 0)
                {
                    var valid = validation.All(kvp => fields.ContainsKey(kvp.Key) && kvp.Value(fields[kvp.Key]));
                    //valid = fields.All(kvp => validation[kvp.Key](kvp.Value));
                    if (valid)
                        validPassports++;
                    fields.Clear();
                }
                else
                {
                    foreach(var field in row.Split(' '))
                    {
                        var kvp = field.Split(':');
                        fields.Add(kvp[0], kvp[1]);
                    }
                }
            }

            if (fields.Count > 0)
            {
                validPassports += fields.All(kvp => validation.ContainsKey(kvp.Key) && validation[kvp.Key](kvp.Value)) ? 1 : 0;
            }
            
            return validPassports;
        }
    }
}