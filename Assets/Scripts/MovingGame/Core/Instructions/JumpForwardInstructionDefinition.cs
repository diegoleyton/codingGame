using Flowbit.Engine.Instructions;
using Flowbit.Engine;
using Flowbit.GameBase.Definitions;

namespace Flowbit.MovingGame.Core.Instructions
{
    /// <summary>
    /// Represents a primitive instruction that jumps the agent two cells forward.
    /// </summary>
    public sealed class JumpForwardInstructionDefinition : GameInstructionDefinitionBase<IMovingGame, InstructionType>
    {
        /// <summary>
        /// Returns the instruction ID.
        /// </summary>
        public override InstructionType GetInstructionId()
        {
            return InstructionType.JumpForward;
        }

        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        public override string GetDisplayName()
        {
            return "Jump Forward";
        }

        /// <summary>
        /// Executes this instruction instance on the strongly typed game.
        /// </summary>
        protected override void ExecuteTyped(IMovingGame game, InstructionInstance<InstructionType> instance)
        {
            game.JumpForward();
        }
    }
}
