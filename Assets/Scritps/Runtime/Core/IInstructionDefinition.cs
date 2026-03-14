using System.Collections.Generic;

namespace CodingGame.Runtime.Core
{
    /// <summary>
    /// Represents the definition of an instruction that can be instantiated and executed.
    /// </summary>
    public interface IInstructionDefinition
    {
        /// <summary>
        /// Returns a unique identifier for this instruction definition.
        /// </summary>
        string GetId();

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
        /// Returns the parameter definitions for this instruction.
        /// </summary>
        IReadOnlyList<InstructionParameterDefinition> GetParameterDefinitions();

        /// <summary>
        /// Executes this instruction instance on the given game.
        /// </summary>
        void Execute(IGame game, InstructionInstance instance);

        /// <summary>
        /// Expands this instruction instance into child instruction instances.
        /// </summary>
        IReadOnlyList<InstructionInstance> Expand(InstructionInstance instance);
    }
}