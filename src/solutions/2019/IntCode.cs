using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Y2019.Day05;

namespace AdventOfCode.Y2019
{
    class IntCode
    {
        private readonly Func<IntCode, int> _input;
        private readonly Action<int> _output;
        private readonly List<int> _memory;
        private int InstructionPointer { get; set; }

        public IntCode(string program, Func<IntCode, int> input, Action<int> output)
        {
            _input = input;
            _output = output;
            _memory = program.Split(',').Select(int.Parse).ToList();
            InstructionPointer = 0;
        }

        public void Run()
        {
            while (ExecuteInstruction() != ProgramState.Finished)
            {
                // This space intentionally left empty.
            }
        }

        private ProgramState ExecuteInstruction()
        {
            var opCode = GetOpCode();
            switch (opCode)
            {
                case 1:
                {
                    // adds 
                    var left = GetValue(0);
                    var right = GetValue(1);
                    var val = left + right;
                    var position = GetPosition(2);
                    this[position] = val;
                    InstructionPointer += 4;
                    return ProgramState.Running;
                }

                case 2:
                {
                    // multiplies 
                    var left = GetValue(0);
                    var right = GetValue(1);
                    var val = left * right;
                    var position = GetPosition(2);
                    this[position] = val;
                    InstructionPointer += 4;
                    return ProgramState.Running;
                }

                case 3:
                {
                    // input 
                    var val = _input(this);
                    var position = GetPosition(0);
                    this[position] = val;
                    InstructionPointer += 2;
                    return ProgramState.Running;
                }

                case 4:
                {
                    // outputs 
                    var value = GetValue(0);
                    _output(value);
                    InstructionPointer += 2;
                    return ProgramState.Running;
                }

                case 5:
                {
                    // jump-if-true
                    var value = GetValue(0);
                    if (value != 0)
                    {
                        var position = GetValue(1);
                        InstructionPointer = position;
                    }
                    else
                    {
                        InstructionPointer += 3;
                    }
                    return ProgramState.Running;
                }

                case 6:
                {
                    // jump-if-false
                    var value = GetValue(0);
                    if (value == 0)
                    {
                        var position = GetValue(1);
                        InstructionPointer = position;
                    }
                    else
                    {
                        InstructionPointer += 3;
                    }
                    return ProgramState.Running;
                }

                case 7:
                {
                    //  less than
                    var left = GetValue(0);
                    var right = GetValue(1);
                    var value = left < right ? 1 : 0;
                    var position = GetPosition(2);
                    this[position] = value;
                    InstructionPointer += 4;
                    return ProgramState.Running;
                }

                case 8:
                {
                    //  equals
                    var left = GetValue(0);
                    var right = GetValue(1);
                    var value = left == right ? 1 : 0;
                    var position = GetPosition(2);
                    this[position] = value;
                    InstructionPointer += 4;
                    return ProgramState.Running;
                }

                case 99:
                {
                    return ProgramState.Finished;
                }

                default:
                    throw new Exception($"Unknown opCode ('{opCode}')");
            }
        }

        private int GetOpCode()
        {
            return Current % 100;
        }

        private int this[int index]
        {
            get => _memory[index];
            set => _memory[index] = value;
        }

        private int Current => this[this.InstructionPointer];

        private int GetValue(int parameterIndex)
        {
            var parameterMode = GetParameterMode(Current, parameterIndex);
            int value;
            switch (parameterMode)
            {
                case ParameterMode.Position:
                    var position = this[InstructionPointer + parameterIndex + 1];
                    value = this[position];
                    break;
                case ParameterMode.Immediate:
                    value = this[InstructionPointer + parameterIndex + 1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        private int GetPosition(int parameterIndex)
        {
            var parameterMode = GetParameterMode(Current, parameterIndex);
            if (parameterMode == ParameterMode.Position)
            {
                var position = this[InstructionPointer + parameterIndex + 1];
                return position;
            }

            throw new Exception("Invalid parameter mode for getting the position.");
        }

        private ParameterMode GetParameterMode(int instruction, int parameterIndex)
        {
            var modes = instruction / 100;
            if (modes == 0)
            {
                return ParameterMode.Position;
            }
            var text = modes.ToString();
            if (text.Length <= parameterIndex)
            {
                return ParameterMode.Position;
            }

            var c = text[text.Length - 1 - parameterIndex];
            if (c == '0') return ParameterMode.Position;
            if (c == '1') return ParameterMode.Immediate;
            throw new Exception($"Unknown parameter mode ('{c}')");
        }
    }
}