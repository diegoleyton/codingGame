using System;
using System.Collections.Generic;
using Flowbit.Engine;

namespace Flowbit.Engine.Instructions
{
    /// <summary>
    /// Represents a composite instruction that repeats its child instructions a fixed number of times.
    /// </summary>
    public abstract class RepeatInstructionDefinition<TInstruction> : InstructionDefinitionBase<TInstruction>
    {
        private readonly InstructionParameterDefinition[] parameterDefinitions_;

        /// <summary>
        /// Creates a new repeat instruction definition.
        /// </summary>
        public RepeatInstructionDefinition()
        {
            parameterDefinitions_ = new[]
            {
            new InstructionParameterDefinition("count", typeof(int), 2)
        };
        }

        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        public override string GetDisplayName()
        {
            return "Repeat";
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
        /// Returns the parameter definitions for this instruction.
        /// </summary>
        public override IReadOnlyList<InstructionParameterDefinition> GetParameterDefinitions()
        {
            return parameterDefinitions_;
        }

        /// <summary>
        /// Expands this instruction instance into repeated child instruction instances.
        /// </summary>
        public override IReadOnlyList<InstructionInstance<TInstruction>> Expand(InstructionInstance<TInstruction> instance)
        {
            int count = (int)instance.GetParameterValue("count");

            if (count <= 0)
            {
                throw new InvalidOperationException("Repeat count must be greater than zero.");
            }

            IReadOnlyList<InstructionInstance<TInstruction>> children = instance.GetChildren();
            if (children.Count == 0)
            {
                throw new InvalidOperationException("Repeat must contain at least one child instruction.");
            }

            List<InstructionInstance<TInstruction>> expanded = new List<InstructionInstance<TInstruction>>(count * children.Count);

            for (int i = 0; i < count; i++)
            {
                foreach (InstructionInstance<TInstruction> child in children)
                {
                    expanded.Add(child);
                }
            }

            return expanded;
        }
    }
}