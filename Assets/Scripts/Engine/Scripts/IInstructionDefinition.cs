using System.Collections.Generic;

namespace Flowbit.Engine
{
    /// <summary>
    /// Represents the definition of an instruction that can be instantiated and executed.
    /// </summary>
    public interface IInstructionDefinition<TInstruction>
    {
        /// <summary>
        /// Returns the display name of this instruction definition.
        /// </summary>
        string GetDisplayName();

        /// <summary>
        /// Returns whether this instruction executes directly without expanding child instructions.
        /// </summary>
        bool IsPrimitive();

        /// <summary>
        /// Returns whether this instruction supports child instructions.
        /// </summary>
        bool SupportsChildren();

        /// <summary>
        /// Returns the instruction ID
        /// </summary>
        /// <returns></returns>
        TInstruction GetInstructionId();

        /// <summary>
        /// Returns the parameter definitions for this instruction.
        /// </summary>
        IReadOnlyList<InstructionParameterDefinition> GetParameterDefinitions();

        /// <summary>
        /// Executes this instruction instance on the given game.
        /// </summary>
        void Execute(IGame game, InstructionInstance<TInstruction> instance);

        /// <summary>
        /// Expands this instruction instance into child instruction instances.
        /// </summary>
        IReadOnlyList<InstructionInstance<TInstruction>> Expand(InstructionInstance<TInstruction> instance);
    }
}