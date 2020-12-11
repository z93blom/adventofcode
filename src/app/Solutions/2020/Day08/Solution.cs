using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2020.Day08 {

    class Solution : ISolver {

        public string GetName() => "Handheld Halting";



        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        private enum OpType
        {
            nop,
            acc,
            jmp,
        }

        private class Op
        {
            public OpType OpType { get; set; }

            public int Value { get; init; }

            public static Op FromText(string t)
            {
                var m = Regex.Match(t, @"(\w{3}) ([+-]?\d+)");
                return new Op()
                {
                    OpType = (OpType)Enum.Parse(typeof(OpType), m.Groups[1].Value),
                    Value = int.Parse(m.Groups[2].Value),
                };
            }
        }



        object PartOne(string input) {
            var ops = input.Lines()
                .Select(Op.FromText)
                .ToArray();
            var acc = 0;
            var offset = 0;
            var visited = new HashSet<int>();
            while(!visited.Contains(offset) && offset < ops.Length)
            {
                visited.Add(offset);
                switch(ops[offset].OpType)
                {
                    case OpType.nop:
                        offset++;
                        break;
                    case OpType.acc:
                        acc+= ops[offset].Value;
                        offset++;
                        break;
                    case OpType.jmp:
                        offset += ops[offset].Value;
                        break;
                }
            }

            return acc;
        }
        

        object PartTwo(string input)
        {
            var ops = input.Lines()
                .Select(Op.FromText)
                .ToArray();
            int acc;
            var opsToTry = ops.Select((op, i) => new {Op = op, Index = i})
                .Where(a => a.Op.OpType != OpType.acc)
                .Select(a => a.Index)
                .Reverse()
                .ToArray();
            var i = 0;
            ops[opsToTry[i]].OpType = ops[opsToTry[i]].OpType == OpType.jmp ? OpType.nop : OpType.jmp;
            while(!TryRun(ops, out acc))
            {
                // Change it back
                ops[opsToTry[i]].OpType = ops[opsToTry[i]].OpType == OpType.jmp ? OpType.nop : OpType.jmp;
                i++;
                ops[opsToTry[i]].OpType = ops[opsToTry[i]].OpType == OpType.jmp ? OpType.nop : OpType.jmp;
            }

            return acc;
        }

        private static bool TryRun(Op[] ops, out int acc)
        {
            acc = 0;
            var offset = 0;
            var visited = new HashSet<int>();
            while (!visited.Contains(offset) && offset < ops.Length)
            {
                visited.Add(offset);
                switch (ops[offset].OpType)
                {
                    case OpType.nop:
                        offset++;
                        break;
                    case OpType.acc:
                        acc += ops[offset].Value;
                        offset++;
                        break;
                    case OpType.jmp:
                        offset += ops[offset].Value;
                        break;
                }
            }

            return offset >= ops.Length;
        }
    }
}