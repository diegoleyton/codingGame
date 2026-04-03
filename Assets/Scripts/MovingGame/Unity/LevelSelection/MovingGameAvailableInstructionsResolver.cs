using System;
using System.Collections.Generic;

using Flowbit.EngineController;
using Flowbit.GameBase.Definitions;
using Flowbit.MovingGame.Core.Levels;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Resolves the instructions available for a moving game level.
    /// </summary>
    public sealed class MovingGameAvailableInstructionsResolver
        : AvailableInstructionsResolverBase<InstructionType>
    {
        private readonly IReadOnlyList<MovingGameLevelData> levels_;

        public MovingGameAvailableInstructionsResolver(IReadOnlyList<MovingGameLevelData> levels)
        {
            levels_ = levels ?? throw new ArgumentNullException(nameof(levels));
        }

        /// <inheritdoc />
        public override IReadOnlyList<InstructionType> Resolve(int levelId)
        {
            MovingGameLevelData levelData = FindLevel(levelId);

            var result = new List<InstructionType>
            {
                InstructionType.MoveForward,
                InstructionType.RotateLeft,
                InstructionType.RotateRight
            };

            if (levelData.breakableBlockedPositions != null &&
                levelData.breakableBlockedPositions.Count > 0)
            {
                result.Add(InstructionType.BreakForward);
            }

            return result;
        }

        private MovingGameLevelData FindLevel(int levelId)
        {
            for (int i = 0; i < levels_.Count; i++)
            {
                if (levels_[i].id == levelId)
                {
                    return levels_[i];
                }
            }

            throw new InvalidOperationException($"Level with id '{levelId}' was not found.");
        }
    }
}