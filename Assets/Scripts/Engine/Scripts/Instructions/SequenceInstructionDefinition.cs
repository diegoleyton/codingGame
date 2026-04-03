using System.Collections.Generic;
using Flowbit.Engine;

namespace Flowbit.Engine.Instructions
{
    /// <summary>
    /// Represents a composite instruction that expands into its child instructions in order.
    /// </summary>
    public abstract class SequenceInstructionDefinition<TInstruction> : InstructionDefinitionBase<TInstruction>
    {
        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        public override string GetDisplayName()
        {
            return "Sequence";
        }

        /// <summary>
        /// Returns whether this instruction executes directly without expanding child instructions.
        /// </summary>
        public override bool IsPrimitive()
        {
            return false;
        }

        /// <summary>
        /// Returns whether this instruction supports child instructions.
        /// </summary>
        public override bool SupportsChildren()
        {
            return true;
        }

        /// <summary>
        /// Expands this instruction instance into child instruction instances.
        /// </summary>
        public override IReadOnlyList<InstructionInstance<TInstruction>> Expand(InstructionInstance<TInstruction> instance)
        {
            return instance.GetChildren();
        }
    }
}