using System;
using System.Collections.Generic;

namespace Flowbit.Engine
{
    /// <summary>
    /// Represents a program made of instruction instances.
    /// </summary>
    public sealed class ProgramDefinition<TInstruction>
    {
        private readonly List<InstructionInstance<TInstruction>> instructions_;

        /// <summary>
        /// Creates an empty program definition.
        /// </summary>
        public ProgramDefinition()
        {
            instructions_ = new List<InstructionInstance<TInstruction>>();
        }

        /// <summary>
        /// Returns the number of root instructions in the program.
        /// </summary>
        public int GetInstructionCount()
        {
            return instructions_.Count;
        }

        /// <summary>
        /// Returns the root instruction at the given index.
        /// </summary>
        public InstructionInstance<TInstruction> GetInstructionAt(int index)
        {
            return instructions_[index];
        }

        /// <summary>
        /// Returns the root instructions as a read-only collection.
        /// </summary>
        public IReadOnlyList<InstructionInstance<TInstruction>> GetInstructions()
        {
            return instructions_;
        }

        /// <summary>
        /// Creates and return the root instructions as a read-only values.
        /// </summary>
        public IReadOnlyList<IReadOnlyInstructionInstance<TInstruction>> GenerateReadOnlyInstructions()
        {
            List<IReadOnlyInstructionInstance<TInstruction>> readOnlyInstructions = new List<IReadOnlyInstructionInstance<TInstruction>>();
            foreach (var instructionInstance in instructions_)
            {
                readOnlyInstructions.Add(instructionInstance);
            }

            return readOnlyInstructions;
        }

        /// <summary>
        /// Adds a root instruction to the end of the program.
        /// </summary>
        public void AddInstruction(InstructionInstance<TInstruction> instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException(nameof(instruction));
            }

            instructions_.Add(instruction);
        }

        /// <summary>
        /// Inserts a root instruction at the given index.
        /// </summary>
        public void InsertInstruction(int index, InstructionInstance<TInstruction> instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException(nameof(instruction));
            }

            instructions_.Insert(index, instruction);
        }

        /// <summary>
        /// Removes the root instruction at the given index.
        /// </summary>
        public void RemoveInstructionAt(int index)
        {
            instructions_.RemoveAt(index);
        }

        /// <summary>
        /// Removes the last root instruction if one exists.
        /// </summary>
        public void RemoveLastInstruction()
        {
            if (instructions_.Count == 0)
            {
                return;
            }

            instructions_.RemoveAt(instructions_.Count - 1);
        }

        /// <summary>
        /// Removes all root instructions from the program.
        /// </summary>
        public void Clear()
        {
            instructions_.Clear();
        }
    }
}