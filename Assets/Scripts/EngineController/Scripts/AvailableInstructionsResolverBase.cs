using System.Collections.Generic;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Base class for resolving the instructions available for a specific level.
    /// </summary>
    /// <typeparam name="TInstruction">
    /// The instruction identifier type used by the game.
    /// </typeparam>
    public abstract class AvailableInstructionsResolverBase<TInstruction>
        : IAvailableInstructionsResolver<TInstruction>
    {
        /// <inheritdoc />
        public abstract IReadOnlyList<TInstruction> Resolve(int levelId);
    }
}