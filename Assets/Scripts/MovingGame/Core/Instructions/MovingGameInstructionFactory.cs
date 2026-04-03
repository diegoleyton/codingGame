using System;
using System.Collections.Generic;

using Flowbit.GameBase.Definitions;
using Flowbit.Engine.Instructions;

namespace Flowbit.MovingGame.Core.Instructions
{
    /// <summary>
    /// Creates instruction definition objects based on the instruction type.
    /// </summary>
    public sealed class MovingGameInstructionFactory : IInstructionFactory<IMovingGame, InstructionType>
    {
        private Dictionary<InstructionType, Func<GameInstructionDefinitionBase<IMovingGame, InstructionType>>> instructionMap_ = new()
        {
            {InstructionType.MoveForward, () => new MoveForwardInstructionDefinition()},
            {InstructionType.RotateLeft, () => new RotateLeftInstructionDefinition()},
            {InstructionType.RotateRight, () => new RotateRightInstructionDefinition()},
            {InstructionType.BreakForward, () => new BreakForwardInstructionDefinition()}
        };

        /// <summary>
        /// Creates a instruction definition based on the instructionType type.
        /// </summary>
        /// <param name="instructionType"></param>
        /// <returns></returns>
        public GameInstructionDefinitionBase<IMovingGame, InstructionType> CreateInstruction(InstructionType instructionType)
        {
            if (!instructionMap_.TryGetValue(instructionType, out var factory))
            {
                throw new NotSupportedException(
                    $"Unsupported instruction type '{instructionType}'.");
            }
            return instructionMap_[instructionType]();
        }
    }
}