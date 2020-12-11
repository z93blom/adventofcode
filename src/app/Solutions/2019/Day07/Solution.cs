using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2019.Day07 {

    class Solution : ISolver {

        public string GetName() => "Amplification Circuit";

        public IEnumerable<object> Solve(string input) 
        {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input) 
        {
            var ampSettings = Enumerable.Range(0, 5).Permutate();
            var maxOutput = long.MinValue;
            foreach(var ampSetting in ampSettings)
            {
                var ampInput = new long[5 + 1];
                for(var index = 0; index < 5; index++)
                {
                    var comp = new IntCode(input);
                    comp.ProvideInput(ampSetting[index]);
                    comp.ProvideInput(ampInput[index]);
                    comp.Run();
                    ampInput[index+1] = comp.ReadOutput();
                }

                maxOutput = Math.Max(maxOutput, ampInput[5]);
            }

            return maxOutput;
        }

        private class CompInputArray
        {
            private int[] _inputValues;
            private int _index;

            public CompInputArray(int[] inputValues)
            {
                _inputValues = inputValues;
                _index = 0;
            }

            public int Next()
            {
                return _inputValues[_index++];
            }
        }

        object PartTwo(string input) {
            var ampSettings = Enumerable.Range(5, 5).Permutate();
            var maxOutput = long.MinValue;
            foreach(var ampSetting in ampSettings)
            {
                // Need to run all 5 computers, entering the output from each one
                // into the input of the next, until they stop.
                var computers = new IntCode[5];
                for(var index = 0; index < 5; index++)
                {
                    computers[index] = new IntCode(input);
                    computers[index].ProvideInput(ampSetting[index]);
                }

                computers[0].ProvideInput(0);

                // Run each computer until it is either finished or it is waiting for input
                int computerRunning = -1;
                ProgramState state = ProgramState.Running;
                do
                {
                    computerRunning = ++computerRunning % 5;
                    var currentComputer = computers[computerRunning];

                    // Provide the input to the computer.
                    var previousComputer = computers[(computerRunning + 4) % 5];
                    if (previousComputer.HasOutput)
                    {
                        currentComputer.ProvideInput(previousComputer.ReadOutput());
                    }
                    do
                    {
                        state = currentComputer.ExecuteInstruction();
                    }
                    while(state != ProgramState.WaitingForInput && state != ProgramState.Finished);
                }
                while(computers[4].State != ProgramState.Finished);

                maxOutput = Math.Max(maxOutput, computers[4].AllOutputs.Last());
            }

            return maxOutput;

        }
    }
}