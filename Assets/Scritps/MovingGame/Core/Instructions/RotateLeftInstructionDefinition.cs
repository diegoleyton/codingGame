using Flowbit.Engine.Instructions;
using Flowbit.Engine;
using Flowbit.Engine.Definitions;

namespace Flowbit.MovingGame.Core.Instructions
{
    /// <summary>
    /// Represents a primitive instruction that rotates the agent to the left.
    /// </summary>
    public sealed class RotateLeftInstructionDefinition : GameInstructionDefinitionBase<IMovingGame>
    {
        /// <summary>
        /// Returns the instruction type
        /// </summary>
        public override InstructionType GetInstructionType()
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
        protected override void ExecuteTyped(IMovingGame game, InstructionInstance instance)
        {
            game.RotateLeft();
        }
    }
}