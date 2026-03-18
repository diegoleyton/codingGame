using System;
using System.Collections.Generic;

namespace Flowbit.Engine
{
    /// <summary>
    /// Executes a program step by step for a given game.
    /// </summary>
    public sealed class ProgramRunner
    {
        private readonly Stack<ExecutionFrame> frames_;
        private ProgramDefinition program_;
        private bool isInitialized_;
        private ExecutionFrame rootExecutionFrame_;

        public ProgramRunner()
        {
            frames_ = new Stack<ExecutionFrame>();
            program_ = null;
            isInitialized_ = false;
            rootExecutionFrame_ = null;
        }

        public void LoadProgram(ProgramDefinition program)
        {
            program_ = program ?? throw new ArgumentNullException(nameof(program));
            ResetExecution();
        }

        public void ResetExecution()
        {
            frames_.Clear();
            isInitialized_ = false;
            rootExecutionFrame_ = null;
        }

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

        public StepResult ExecuteNextStep(IGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            if (program_ == null)
                throw new InvalidOperationException("No program loaded.");

            if (game.HasWon() || game.HasFailed())
                return new StepResult(StepExecutionStatus.BlockedByGameState, null);

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

                IReadOnlyList<InstructionInstance> expanded =
                    instruction.GetDefinition().Expand(instruction);

                frames_.Push(new ExecutionFrame(expanded));
            }

            return new StepResult(StepExecutionStatus.CompletedProgram, null);
        }

        public int GetCurrentInstructionIndex()
        {
            if (program_ == null)
                return -1;

            EnsureInitialized();

            if (rootExecutionFrame_ == null || rootExecutionFrame_.IsComplete())
                return -1;

            return rootExecutionFrame_.GetCurrentInstructionIndex();
        }

        private void EnsureInitialized()
        {
            if (isInitialized_)
                return;

            rootExecutionFrame_ = new ExecutionFrame(program_.GetInstructions());
            frames_.Push(rootExecutionFrame_);
            isInitialized_ = true;
        }
    }
}