using System;
using System.Collections.Generic;
using Flowbit.Engine.Instructions;
using Flowbit.Engine;
using Flowbit.GameBase.Definitions;

namespace Flowbit.MovingGame.Core.Instructions
{
    /// <summary>
    /// Represents a primitive instruction that moves the agent forward.
    /// </summary>
    public sealed class MoveForwardInstructionDefinition : GameInstructionDefinitionBase<IMovingGame, InstructionType>
    {
        private readonly InstructionParameterDefinition[] parameterDefinitions_;

        /// <summary>
        /// Creates a new move-forward instruction definition.
        /// </summary>
        public MoveForwardInstructionDefinition()
        {
            parameterDefinitions_ = new[]
            {
            new InstructionParameterDefinition("steps", typeof(int), 1)
        };
        }

        /// <summary>
        /// Returns the instruction ID.
        /// </summary>
        public override InstructionType GetInstructionId()
        {
            return InstructionType.MoveForward;
        }

        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        public override string GetDisplayName()
        {
            return "Move Forward";
        }

        /// <summary>
        /// Returns the parameter definitions for this instruction.
        /// </summary>
        public override IReadOnlyList<InstructionParameterDefinition> GetParameterDefinitions()
        {
            return parameterDefinitions_;
        }

        /// <summary>
        /// Executes this instruction instance on the given game.
        /// </summary>
        protected override void ExecuteTyped(IMovingGame game, InstructionInstance<InstructionType> instance)
        {
            int steps = (int)instance.GetParameterValue("steps");
            if (steps <= 0)
            {
                throw new InvalidOperationException("Steps must be greater than zero.");
            }

            game.MoveForward(steps);
        }
    }
}