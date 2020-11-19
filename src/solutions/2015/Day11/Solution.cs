using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2015.Day11 {

    class Solution : ISolver {

        public string GetName() => "Corporate Policy";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        string PartOne(string input)
        {
            return GetNextValidPassword(input);
        }

        string PartTwo(string input)
        {
            var current = PartOne(input);
            return GetNextValidPassword(current);
        }

        private string GetNextValidPassword(string current)
        {
            var currentAsArray = ToArray(current);
            var proposed = GetNextPwd(currentAsArray);
            while (!FullFillsReq1(proposed) ||
                   !FullFillsReq2(proposed) ||
                   !FullFillsReq3(proposed))
            {
                proposed = GetNextPwd(proposed);
            }

            return ToString(proposed);
        }

        public byte[] ToArray(string password)
        {
            var result = new byte[password.Length];
            for (var index = 0; index < password.Length; index++)
            {
                result[index] = (byte)(password[index] - 'a');
            }

            return result;
        }

        public string ToString(byte[] password)
        {
            var bytes = password.Select(b => (byte)(b + 'a')).ToArray();
            var text = Encoding.ASCII.GetString(bytes);
            return text;
        }

        public byte[] GetNextPwd(byte[] current)
        {
            var newPassword = new byte[current.Length];
            // Find the last max value.
            var position = current.Length - 1;
            var add = 1;
            while (position >= 0)
            {
                newPassword[position] = (byte)((current[position] + add) % 26);
                if (newPassword[position] != 0)
                    add = 0;
                position--;
            }

            return newPassword;
        }

        public bool FullFillsReq1(byte[] password)
        {
            for (var index = 0; index < password.Length - 3; index++)
            {
                if (password[index + 1] == password[index] + 1 &&
                    password[index + 2] == password[index] + 2)
                {
                    return true;
                }
            }

            return false;
        }

        public bool FullFillsReq2(byte[] password)
        {
            for (var index = 0; index < password.Length; index++)
            {
                if (password[index] == 'i' - 'a' ||
                    password[index] == 'l' - 'a' ||
                    password[index] == 'o' - 'a')
                {
                    return false;
                }
            }

            return true;
        }

        public bool FullFillsReq3(byte[] password)
        {
            var pairsFound = 0;
            var index = 0;
            while (index < password.Length - 1)
            {
                if (password[index] == password[index + 1])
                {
                    pairsFound++;
                    index += 2;
                }
                else
                {
                    index += 1;
                }
            }

            return pairsFound > 1;
        }
    }
}