using CodingGame.Runtime.Definitions;
using CodingGame.Runtime.Instructions;

namespace CodingGame.Runtime.Games.Moving
{
    /// <summary>
    /// Represents a primitive instruction that breaks the breakable obstacle in front of the agent.
    /// </summary>
    public sealed class BreakForwardInstructionDefinition : GameInstructionDefinitionBase<IMovingGame>
    {
        /// <summary>
        /// Returns the instruction type.
        /// </summary>
        public override InstructionType GetInstructionType()
        {
            return InstructionType.BreakForward;
        }

        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        public override string GetDisplayName()
        {
            return "Break Forward";
        }

        /// <summary>
        /// Executes this instruction instance on the strongly typed game.
        /// </summary>
        protected override void ExecuteTyped(IMovingGame game, Core.InstructionInstance instance)
        {
            game.BreakForward();
        }
    }
}