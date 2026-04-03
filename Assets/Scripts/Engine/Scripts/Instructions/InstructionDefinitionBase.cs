using System;
using System.Collections.Generic;

namespace Flowbit.Engine.Instructions
{
    /// <summary>
    /// Provides a base implementation for instruction definitions.
    /// </summary>
    public abstract class InstructionDefinitionBase<TInstruction> : IInstructionDefinition<TInstruction>
    {
        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        public abstract string GetDisplayName();

        /// <summary>
        /// Returns the instruction id
        /// </summary>
        public abstract TInstruction GetInstructionId();

        /// <summary>
        /// Returns whether this instruction executes directly without expanding child instructions.
        /// </summary>
        public virtual bool IsPrimitive()
        {
            return true;
        }

        /// <summary>
        /// Returns whether this instruction supports child instructions.
        /// </summary>
        public virtual bool SupportsChildren()
        {
            return false;
        }

        /// <summary>
        /// Returns the parameter definitions for this instruction.
        /// </summary>
        public virtual IReadOnlyList<InstructionParameterDefinition> GetParameterDefinitions()
        {
            return Array.Empty<InstructionParameterDefinition>();
        }

        /// <summary>
        /// Executes this instruction instance on the given game.
        /// </summary>
        public virtual void Execute(IGame game, InstructionInstance<TInstruction> instance)
        {
            throw new InvalidOperationException($"{GetDisplayName()} cannot execute directly.");
        }

        /// <summary>
        /// Expands this instruction instance into child instruction instances.
        /// </summary>
        public virtual IReadOnlyList<InstructionInstance<TInstruction>> Expand(InstructionInstance<TInstruction> instance)
        {
            throw new InvalidOperationException($"{GetDisplayName()} cannot expand.");
        }
    }
}