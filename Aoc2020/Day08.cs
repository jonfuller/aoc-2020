using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Aoc2020
{
    [Command(name: "day-08")]
    public class Day08 : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var sampleMachine = LoadMachine("input/08-sample");
            var inputMachine = LoadMachine("input/08-input");

            console.Output.WriteLine(sampleMachine.RunUntilInfLoop().Accumulator);
            console.Output.WriteLine(inputMachine.RunUntilInfLoop().Accumulator);
        }

        static Machine LoadMachine(string filename) => File.ReadAllLines(filename)
           .Select(ParseInstruction)
           .Then(program => new Machine(0, program.ToList(), new List<int>(), 0));

        static Instruction ParseInstruction(string line) => InstructionParsingRegex.Deserialize<Instruction>(line);
        static readonly Regex InstructionParsingRegex = new(@"^(?<Op>[a-z]{3}) (?<Argument>[\+\-0-9]+)$", RegexOptions.Compiled);
    }

    public record Machine(int Accumulator, List<Instruction> Instructions, List<int> AlreadyExecuted, int CurrentInstructionPointer)
    {
        public Instruction CurrentInstruction => Instructions[CurrentInstructionPointer];

        public Machine RunUntilInfLoop()
        {
            var machine = this;
            while (!machine.AlreadyExecuted.Contains(machine.CurrentInstructionPointer))
            {
                machine = machine.ExecuteCurrentInstruction();
            }
            return machine;
        }

        public Machine ExecuteCurrentInstruction() => CurrentInstruction.Op switch
        {
            "nop" => this with
            {
                AlreadyExecuted = AlreadyExecuted.Concat(new[] {CurrentInstructionPointer}).ToList(),
                CurrentInstructionPointer = CurrentInstructionPointer + 1
            },
            "acc" => this with
            {
                AlreadyExecuted = AlreadyExecuted.Concat(new[] { CurrentInstructionPointer }).ToList(),
                CurrentInstructionPointer = CurrentInstructionPointer + 1,
                Accumulator = Accumulator + CurrentInstruction.Argument
            },
            "jmp" => this with
            {
                AlreadyExecuted = AlreadyExecuted.Concat(new[] { CurrentInstructionPointer }).ToList(),
                CurrentInstructionPointer = CurrentInstructionPointer + CurrentInstruction.Argument
            },
            _ => throw new InvalidProgramException($"This operation is not supported {CurrentInstruction.Op}")
        };
    }

    public record Instruction
    {
        public string Op { get; init; }
        public int Argument { get; init; }
    }
}