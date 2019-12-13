using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Puzzles.Solutions.Year2019
{
    class IntCode
    {
        private readonly List<long> _input = new List<long>();
        private int _inputIndex = 0;

        private readonly List<long> _output = new List<long>();

        private int _outputIndex = 0;

        private readonly Dictionary<long, long> _memory;
        private ProgramState _state;
        private long _relativeBaseOffset = 0;

        private long InstructionPointer { get; set; }

        public IntCode(string program)
        {
            var address = 0L;
            _memory = program.Split(',').Select(long.Parse).ToDictionary(v => address++, v => v);
            _state = ProgramState.NotStarted;
            InstructionPointer = 0;
        }

        public void ProvideInput(long value)
        {
            _input.Add(value);
        }

        public bool HasOutput => _output.Count > _outputIndex;

        public long ReadOutput()
        {
            return _output[_outputIndex++];
        }

        public ProgramState State => _state;

        public List<long> AllOutputs => _output;

        public void Run()
        {
            while (ExecuteInstruction() != ProgramState.Finished)
            {
                // This space intentionally left empty.
            }
        }

        public void RunToNextInputOrFinished()
        {
            ProgramState state;
            do
            {
                state = ExecuteInstruction();
            } while (state != ProgramState.Finished && state != ProgramState.WaitingForInput);
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

                case 9:
                {
                    var val = GetValue(0);
                    _relativeBaseOffset += val;
                    InstructionPointer += 2;
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

        private long GetOpCode()
        {
            return Current % 100;
        }

        public long this[long index]
        {
            get => _memory.ContainsKey(index) ? _memory[index] : 0;
            set => _memory[index] = value;
        }

        private long Current => this[this.InstructionPointer];

        private long GetValue(int instructionPointerOffset)
        {
            var parameterMode = GetParameterMode(Current, instructionPointerOffset);
            long value;
            switch (parameterMode)
            {
                case ParameterMode.Position:
                    var position = this[InstructionPointer + instructionPointerOffset + 1];
                    value = this[position];
                    break;
                case ParameterMode.Immediate:
                    value = this[InstructionPointer + instructionPointerOffset + 1];
                    break;
                case ParameterMode.Relative:
                    value = this[_relativeBaseOffset + this[InstructionPointer + instructionPointerOffset + 1]];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        private long GetPosition(int parameterIndex)
        {
            var parameterMode = GetParameterMode(Current, parameterIndex);
            if (parameterMode == ParameterMode.Position)
            {
                var position = this[InstructionPointer + parameterIndex + 1];
                return position;
            }
            else if (parameterMode == ParameterMode.Relative)
            {
                var position = _relativeBaseOffset + this[InstructionPointer + parameterIndex + 1];
                return position;
            }

            throw new Exception("Invalid parameter mode for getting the position.");
        }

        private ParameterMode GetParameterMode(long instruction, int parameterIndex)
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
            if (c == '2') return ParameterMode.Relative;
            throw new Exception($"Unknown parameter mode ('{c}')");
        }
    }
}