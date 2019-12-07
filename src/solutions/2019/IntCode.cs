using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Y2019.Day05;

namespace AdventOfCode.Y2019
{
    class IntCode
    {
        private List<int> _input = new List<int>();
        private int _inputIndex = 0;

        private List<int> _output = new List<int>();

        private int _outputIndex = 0;

        private readonly List<int> _memory;
        private ProgramState _state;

        private int InstructionPointer { get; set; }

        public IntCode(string program)
        {
            _memory = program.Split(',').Select(int.Parse).ToList();
            _state = ProgramState.NotStarted;
            InstructionPointer = 0;
        }

        public void ProvideInput(int value)
        {
            _input.Add(value);
        }

        public bool HasOutput => _output.Count > _outputIndex;

        public int ReadOutput()
        {
            return _output[_outputIndex++];
        }

        public ProgramState State => _state;

        public List<int> AllOutputs => _output;

        public void Run()
        {
            while (ExecuteInstruction() != ProgramState.Finished)
            {
                // This space intentionally left empty.
            }
        }

        public ProgramState ExecuteInstruction()
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
                    _state = ProgramState.Running;
                    return _state;
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
                    _state = ProgramState.Running;
                    return _state;
                }

                case 3:
                {
                    // input 
                    if (_input.Count <= _inputIndex)
                    {
                    _state = ProgramState.WaitingForInput;
                    return _state;
                    }

                    var val = _input[_inputIndex++];
                    var position = GetPosition(0);
                    this[position] = val;
                    InstructionPointer += 2;
                    _state = ProgramState.Running;
                    return _state;
                }

                case 4:
                {
                    // outputs 
                    var value = GetValue(0);
                    _output.Add(value);
                    InstructionPointer += 2;
                    _state = ProgramState.Running;
                    return _state;
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

                    _state = ProgramState.Running;
                    return _state;
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

                    _state = ProgramState.Running;
                    return _state;
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
                    _state = ProgramState.Running;
                    return _state;
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
                    _state = ProgramState.Running;
                    return _state;
                }

                case 99:
                {
                    _state = ProgramState.Finished;
                    return _state;
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