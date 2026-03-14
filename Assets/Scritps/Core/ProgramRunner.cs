using System;
using System.Collections.Generic;

namespace CodingGame.Core
{
    /// <summary>
    /// Executes a program step by step for a given game.
    /// </summary>
    public sealed class ProgramRunner
    {
        private readonly Stack<ExecutionFrame> frames_;
        private ProgramDefinition program_;
        private bool isInitialized_;

        /// <summary>
        /// Creates a new program runner.
        /// </summary>
        public ProgramRunner()
        {
            frames_ = new Stack<ExecutionFrame>();
            program_ = null;
            isInitialized_ = false;
        }

        /// <summary>
        /// Loads a program into the runner and resets execution state.
        /// </summary>
        public void LoadProgram(ProgramDefinition program)
        {
            program_ = program ?? throw new ArgumentNullException(nameof(program));
            ResetExecution();
        }

        /// <summary>
        /// Resets execution state without modifying the loaded program.
        /// </summary>
        public void ResetExecution()
        {
            frames_.Clear();
            isInitialized_ = false;
        }

        /// <summary>
        /// Returns whether a program is currently loaded.
        /// </summary>
        public bool HasProgramLoaded()
        {
            return program_ != null;
        }

        /// <summary>
        /// Returns whether execution has finished for the currently loaded program.
        /// </summary>
        public bool IsFinished()
        {
            if (program_ == null)
            {
                return true;
            }

            if (!isInitialized_)
            {
                return program_.GetInstructionCount() == 0;
            }

            return frames_.Count == 0;
        }

        /// <summary>
        /// Executes the next primitive step of the loaded program on the given game.
        /// </summary>
        public StepResult ExecuteNextStep(IGame game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (program_ == null)
            {
                throw new InvalidOperationException("No program loaded.");
            }

            if (game.HasWon() || game.HasFailed())
            {
                return new StepResult(StepExecutionStatus.BlockedByGameState, null);
            }

            EnsureInitialized();

            while (frames_.Count > 0)
            {
                ExecutionFrame topFrame = frames_.Peek();

                if (topFrame.IsComplete())
                {
                    frames_.Pop();
                    continue;
                }

                InstructionInstance instruction = topFrame.GetCurrentInstruction();
                topFrame.Advance();

                if (instruction.GetDefinition().IsPrimitive())
                {
                    instruction.GetDefinition().Execute(game, instruction);
                    return new StepResult(StepExecutionStatus.ExecutedPrimitive, instruction);
                }

                IReadOnlyList<InstructionInstance> expanded = instruction.GetDefinition().Expand(instruction);
                frames_.Push(new ExecutionFrame(expanded));
            }

            return new StepResult(StepExecutionStatus.CompletedProgram, null);
        }

        private void EnsureInitialized()
        {
            if (isInitialized_)
            {
                return;
            }

            frames_.Push(new ExecutionFrame(program_.GetInstructions()));
            isInitialized_ = true;
        }
    }
}