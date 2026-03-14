using System;
using System.Collections.Generic;
using CodingGame.Core;

namespace CodingGame.Instructions
{
    /// <summary>
    /// Represents a composite instruction that repeats its child instructions a fixed number of times.
    /// </summary>
    public sealed class RepeatInstructionDefinition : InstructionDefinitionBase
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
        /// Returns a unique identifier for this instruction definition.
        /// </summary>
        public override string GetId()
        {
            return "repeat";
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
        public override IReadOnlyList<InstructionInstance> Expand(InstructionInstance instance)
        {
            int count = (int)instance.GetParameterValue("count");

            if (count <= 0)
            {
                throw new InvalidOperationException("Repeat count must be greater than zero.");
            }

            IReadOnlyList<InstructionInstance> children = instance.GetChildren();
            if (children.Count == 0)
            {
                throw new InvalidOperationException("Repeat must contain at least one child instruction.");
            }

            List<InstructionInstance> expanded = new List<InstructionInstance>(count * children.Count);

            for (int i = 0; i < count; i++)
            {
                foreach (InstructionInstance child in children)
                {
                    expanded.Add(child);
                }
            }

            return expanded;
        }
    }
}