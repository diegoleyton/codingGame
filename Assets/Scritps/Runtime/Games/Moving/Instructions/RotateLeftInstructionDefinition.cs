using CodingGame.Runtime.Instructions;
using CodingGame.Runtime.Core;
using CodingGame.Runtime.Definitions;

namespace CodingGame.Runtime.Games.Moving
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