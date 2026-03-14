using System;
using CodingGame.Runtime.Core;

namespace CodingGame.Runtime.Instructions
{
    /// <summary>
    /// Provides a typed base class for instruction definitions that execute on a specific game type.
    /// </summary>
    public abstract class GameInstructionDefinitionBase<TGame>
        : InstructionDefinitionBase
        where TGame : IGame
    {
        /// <summary>
        /// Executes this instruction instance on the given game.
        /// </summary>
        public override void Execute(IGame game, InstructionInstance instance)
        {
            if (game is not TGame typedGame)
            {
                throw new ArgumentException(
                    $"Game must be of type {typeof(TGame).Name}.",
                    nameof(game));
            }

            ExecuteTyped(typedGame, instance);
        }

        /// <summary>
        /// Executes this instruction instance on the strongly typed game.
        /// </summary>
        protected abstract void ExecuteTyped(TGame game, InstructionInstance instance);
    }
}