using System;
using System.Collections.Generic;

namespace CodingGame.Runtime.Core
{
    /// <summary>
    /// Represents the execution state of a list of instruction instances.
    /// </summary>
    internal sealed class ExecutionFrame
    {
        private readonly IReadOnlyList<InstructionInstance> instructions_;
        private int currentIndex_;

        /// <summary>
        /// Creates a frame for the given instruction list.
        /// </summary>
        public ExecutionFrame(IReadOnlyList<InstructionInstance> instructions)
        {
            instructions_ = instructions ?? throw new ArgumentNullException(nameof(instructions));
            currentIndex_ = 0;
        }

        /// <summary>
        /// Returns whether this frame has finished execution.
        /// </summary>
        public bool IsComplete()
        {
            return currentIndex_ >= instructions_.Count;
        }

        /// <summary>
        /// Returns the current instruction in this frame.
        /// </summary>
        public InstructionInstance GetCurrentInstruction()
        {
            if (IsComplete())
            {
                throw new InvalidOperationException("Frame is complete.");
            }

            return instructions_[currentIndex_];
        }

        /// <summary>
        /// Advances this frame to the next instruction.
        /// </summary>
        public void Advance()
        {
            currentIndex_++;
        }

        /// <summary>
        /// Returns the current instruction index.
        /// </summary>
        public int GetCurrentInstructionIndex()
        {
            return currentIndex_;
        }
    }
}