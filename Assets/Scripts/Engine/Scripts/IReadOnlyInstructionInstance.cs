using System.Collections.Generic;

namespace Flowbit.Engine
{
    /// <summary>
    /// Read only values of a concrete use of an instruction definition inside a program.
    /// </summary>
    public interface IReadOnlyInstructionInstance
    {
        /// <summary>
        /// Returns the instruction definition used by this instance.
        /// </summary>
        IInstructionDefinition GetDefinition();

        /// <summary>
        /// Returns all the parameters of this instruction instance, and their values.
        /// </summary>
        IReadOnlyDictionary<string, object> GetParameterValues();
    }
}