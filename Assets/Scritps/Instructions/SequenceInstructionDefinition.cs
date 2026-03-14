using System.Collections.Generic;
using CodingGame.Core;

namespace CodingGame.Instructions
{
    /// <summary>
    /// Represents a composite instruction that expands into its child instructions in order.
    /// </summary>
    public sealed class SequenceInstructionDefinition : InstructionDefinitionBase
    {
        /// <summary>
        /// Returns a unique identifier for this instruction definition.
        /// </summary>
        public override string GetId()
        {
            return "sequence";
        }

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
        public override IReadOnlyList<InstructionInstance> Expand(InstructionInstance instance)
        {
            return instance.GetChildren();
        }
    }
}