using Flowbit.Engine.Instructions;
using Flowbit.Engine;
using Flowbit.GameBase.Definitions;

namespace Flowbit.MovingGame.Core.Instructions
{
    /// <summary>
    /// Represents a primitive instruction that rotates the agent to the left.
    /// </summary>
    public sealed class RotateLeftInstructionDefinition : GameInstructionDefinitionBase<IMovingGame>
    {
        /// <summary>
        /// Returns the instruction ID.
        /// </summary>
        public override int GetInstructionId()
        {
            return (int)InstructionType.RotateLeft;
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