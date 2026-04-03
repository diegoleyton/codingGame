using Flowbit.Engine.Instructions;
using Flowbit.Engine;
using Flowbit.GameBase.Definitions;

namespace Flowbit.MovingGame.Core.Instructions
{
    /// <summary>
    /// Represents a primitive instruction that rotates the agent to the left.
    /// </summary>
    public sealed class RotateLeftInstructionDefinition : GameInstructionDefinitionBase<IMovingGame, InstructionType>
    {
        /// <summary>
        /// Returns the instruction ID.
        /// </summary>
        public override InstructionType GetInstructionId()
        {
            return InstructionType.RotateLeft;
        }

        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        public override string GetDisplayName()
        {
            return "Rotate Left";
        }

        /// <summary>
        /// Executes this instruction instance on the given game.
        /// </summary>
        protected override void ExecuteTyped(IMovingGame game, InstructionInstance<InstructionType> instance)
        {
            game.RotateLeft();
        }
    }
}