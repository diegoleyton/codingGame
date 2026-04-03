using System.Collections.Generic;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Resolves the list of instructions available for a specific level.
    /// </summary>
    /// <typeparam name="TInstruction">
    /// The instruction identifier type used by the game.
    /// </typeparam>
    public interface IAvailableInstructionsResolver<TInstruction>
    {
        /// <summary>
        /// Returns the instructions available for the provided level id.
        /// </summary>
        IReadOnlyList<TInstruction> Resolve(int levelId);
    }
}