using Flowbit.Engine.Instructions;
using Flowbit.Engine;
using Flowbit.GameBase.Definitions;

namespace Flowbit.MovingGame.Core.Instructions
{
    /// <summary>
    /// Represents a primitive instruction that breaks the breakable obstacle in front of the agent.
    /// </summary>
    public sealed class BreakForwardInstructionDefinition : GameInstructionDefinitionBase<IMovingGame, InstructionType>
    {
        /// <summary>
        /// Returns the instruction ID.
        /// </summary>
        public override InstructionType GetInstructionId()
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
        protected override void ExecuteTyped(IMovingGame game, InstructionInstance<InstructionType> instance)
        {
            game.BreakForward();
        }
    }
}