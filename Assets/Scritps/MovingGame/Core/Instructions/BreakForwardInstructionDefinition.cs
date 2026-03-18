using Flowbit.Engine.Definitions;
using Flowbit.Engine.Instructions;
using Flowbit.Engine;

namespace Flowbit.MovingGame.Core.Instructions
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
        protected override void ExecuteTyped(IMovingGame game, InstructionInstance instance)
        {
            game.BreakForward();
        }
    }
}