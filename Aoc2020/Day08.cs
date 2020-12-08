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
            var sampleProgram = LoadProgram("input/08-sample");
            var inputProgram = LoadProgram("input/08-input");

            console.Output.WriteLine(LoadMachine(sampleProgram).Run().Accumulator);
            console.Output.WriteLine(LoadMachine(inputProgram).Run().Accumulator);

            FindGoodProgram(sampleProgram);
            FindGoodProgram(inputProgram);

            void FindGoodProgram(List<Instruction> initialProgram) => CreateAlternatePrograms(initialProgram)
               .Select(LoadMachine)
               .Select(m => m.Run())
               .First(m => m.IsTerminated)
               .Then(m => console.Output.WriteLine(m.Accumulator));
        }

        static List<Instruction> LoadProgram(string filename) => File
           .ReadAllLines(filename)
           .Select(ParseInstruction)
           .ToList();

        static Machine LoadMachine(List<Instruction> program) => new Machine(0, program.ToList(), new List<int>(), 0);
        static Instruction ParseInstruction(string line) => InstructionParsingRegex.Deserialize<Instruction>(line);
        static readonly Regex InstructionParsingRegex = new(@"^(?<Op>[a-z]{3}) (?<Argument>[\+\-0-9]+)$", RegexOptions.Compiled);

        IEnumerable<List<Instruction>> CreateAlternatePrograms(List<Instruction> initialProgram)
        {
            return Enumerable.Range(0, initialProgram.Count)
               .Where(ShouldSwapInstruction)
               .Select(SwapInstructionAt)
               .ToList();

            bool ShouldSwapInstruction(int i) => initialProgram[i].Op == "nop" || initialProgram[i].Op == "jmp";

            List<Instruction> SwapInstructionAt(int i)
            {
                var newInstr = initialProgram[i] with {
                    Op = initialProgram[i].Op switch
                    {
                        "jmp" => "nop",
                        "nop" => "jmp",
                        _ => throw new InvalidOperationException("invalid instruction swap")
                    }
                };

                return Enumerable.Empty<Instruction>()
                   .Concat(initialProgram.Take(i))
                   .Concat(new[]{newInstr})
                   .Concat(initialProgram.Skip(i + 1))
                   .ToList();
            }
        }
    }

    public record Machine(int Accumulator, List<Instruction> Instructions, List<int> AlreadyExecuted, int CurrentInstructionPointer)
    {
        public Instruction CurrentInstruction => Instructions[CurrentInstructionPointer];
        public bool IsTerminated => CurrentInstructionPointer == Instructions.Count;

        public Machine Run()
        {
            var machine = this;
            while (!machine.AlreadyExecuted.Contains(machine.CurrentInstructionPointer) && !machine.IsTerminated)
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