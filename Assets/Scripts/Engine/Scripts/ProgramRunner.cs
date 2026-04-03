using System;
using System.Collections.Generic;

namespace Flowbit.Engine
{
    /// <summary>
    /// Executes a program step by step for a given game.
    /// </summary>
    public sealed class ProgramRunner<TInstruction>
    {
        private readonly Stack<ExecutionFrame<TInstruction>> frames_;
        private ProgramDefinition<TInstruction> program_;
        private bool isInitialized_;
        private ExecutionFrame<TInstruction> rootExecutionFrame_;

        public ProgramRunner()
        {
            frames_ = new Stack<ExecutionFrame<TInstruction>>();
            program_ = null;
            isInitialized_ = false;
            rootExecutionFrame_ = null;
        }

        public void LoadProgram(ProgramDefinition<TInstruction> program)
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

        public bool IsStopped()
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

        public StepResult<TInstruction> ExecuteNextStep(IGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            if (program_ == null)
                throw new InvalidOperationException("No program loaded.");

            if (game.HasWon() || game.HasFailed())
                return new StepResult<TInstruction>(StepExecutionStatus.BlockedByGameState, null);

            EnsureInitialized();

            while (frames_.Count > 0)
            {
                ExecutionFrame<TInstruction> topFrame = frames_.Peek();

                if (topFrame.IsComplete())
                {
                    frames_.Pop();
                    continue;
                }

                InstructionInstance<TInstruction> instruction = topFrame.GetCurrentInstruction();
                topFrame.Advance();

                if (instruction.GetDefinition().IsPrimitive())
                {
                    instruction.GetDefinition().Execute(game, instruction);
                    return new StepResult<TInstruction>(StepExecutionStatus.ExecutedPrimitive, instruction);
                }

                IReadOnlyList<InstructionInstance<TInstruction>> expanded =
                    instruction.GetDefinition().Expand(instruction);

                frames_.Push(new ExecutionFrame<TInstruction>(expanded));
            }

            return new StepResult<TInstruction>(StepExecutionStatus.CompletedProgram, null);
        }

        public int GetCurrentInstructionIndex()
        {
            if (program_ == null)
                return -1;

            EnsureInitialized();

            if (rootExecutionFrame_ == null)
                return -1;

            if (rootExecutionFrame_.IsComplete())
            {
                return rootExecutionFrame_.GetCurrentInstructionIndex() - 1;
            }

            return rootExecutionFrame_.GetCurrentInstructionIndex();
        }

        private void EnsureInitialized()
        {
            if (isInitialized_)
                return;

            rootExecutionFrame_ = new ExecutionFrame<TInstruction>(program_.GetInstructions());
            frames_.Push(rootExecutionFrame_);
            isInitialized_ = true;
        }
    }
}