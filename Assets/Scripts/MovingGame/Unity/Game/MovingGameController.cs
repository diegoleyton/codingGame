using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Flowbit.EngineController;
using Flowbit.MovingGame.Core;
using Flowbit.MovingGame.Core.Instructions;
using Flowbit.MovingGame.Core.Levels;
using Flowbit.Utilities.Core.Events;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Services;
using Flowbit.Engine.Instructions;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Coordinates the moving game runtime, scene view, and execution controls.
    /// </summary>
    public sealed class MovingGameController : GeneralGameController<IMovingGame>
    {
        [Header("Level Data")]
        [SerializeField] private MovingGameLevelsLibrary levelsLibrary_;

        [Header("View References")]
        [SerializeField] private MovingGameView movingGameView_;
        [SerializeField] private GridRenderer gridRenderer_;

        private IAvailableInstructionsResolver<InstructionType> availableInstructionsResolver_;

        private IInstructionFactory<IMovingGame, InstructionType> instructionFactory_ = new MovingGameInstructionFactory();

        private Core.MovingGame game_;
        private MovingGameLevelData currentLevelData_;
        private bool completedEventSent_;
        private bool failedEventSent_;

        /// <summary>
        /// Resolver used by the base controller to determine which instructions
        /// are available for the currently loaded level.
        /// </summary>
        protected override IAvailableInstructionsResolver<InstructionType> AvailableInstructionsResolver =>
            availableInstructionsResolver_;

        protected override IInstructionFactory<IMovingGame, InstructionType> InstructionFactory => instructionFactory_;

        /// <summary>
        /// Loads the given level into the controller.
        /// </summary>
        public void LoadLevel(MovingGameLevelData levelData)
        {
            if (levelData == null)
            {
                throw new ArgumentNullException(nameof(levelData));
            }

            EnsureAvailableInstructionsResolverInitialized();

            currentLevelData_ = levelData;
            completedEventSent_ = false;
            failedEventSent_ = false;

            ResolveAvailableInstructions(levelData.id);

            game_ = CreateGameFromLevelData(levelData);
            LoadGame(game_);
        }

        /// <summary>
        /// Creates the game instance.
        /// </summary>
        protected override IMovingGame CreateGame()
        {
            if (currentLevelData_ == null)
            {
                throw new InvalidOperationException("No level data has been loaded.");
            }

            return CreateGameFromLevelData(currentLevelData_);
        }

        /// <summary>
        /// Initializes the grid and moving game view.
        /// </summary>
        protected override void InitializeView()
        {
            RenderGrid();

            if (movingGameView_ != null && game_ != null)
            {
                movingGameView_.Initialize(gridRenderer_, game_);
            }
        }

        /// <summary>
        /// Refreshes the view immediately.
        /// </summary>
        protected override void RefreshViewImmediate()
        {
            if (movingGameView_ == null || game_ == null)
            {
                return;
            }

            RenderGrid();
            movingGameView_.RefreshImmediate(game_);
        }

        /// <summary>
        /// Refreshes the view with animation.
        /// </summary>
        protected override IEnumerator RefreshViewAnimated()
        {
            if (movingGameView_ == null || game_ == null)
            {
                yield break;
            }

            StepAfterProcess stepAfterProcess = game_.GetStepAfterProcess();

            yield return movingGameView_.ExecuteStepAfterProcess(stepAfterProcess, game_);

            game_.FinalizeStepAfterProcess();

            RenderGrid();
            movingGameView_.RefreshImmediate(game_);
        }

        /// <summary>
        /// Applies the last executed step immediately without animation.
        /// </summary>
        protected override void ApplyExecutedStepImmediate()
        {
            if (game_ == null)
            {
                return;
            }

            if (game_.HasStepAfterProcess())
            {
                game_.FinalizeStepAfterProcess();
            }

            RenderGrid();

            if (movingGameView_ != null)
            {
                movingGameView_.RefreshImmediate(game_);
            }
        }

        /// <summary>
        /// Updates the result UI and emits level state events when needed.
        /// </summary>
        protected override void RefreshResultView()
        {
            base.RefreshResultView();

            if (game_ == null || currentLevelData_ == null)
            {
                return;
            }

            if (game_.HasWon())
            {
                if (!completedEventSent_)
                {
                    completedEventSent_ = true;
                    failedEventSent_ = false;

                    eventDispatcher_?.Send(
                        this,
                        new LevelCompletedEvent(currentLevelData_.id));
                }

                return;
            }

            if (game_.HasFailed())
            {
                if (!failedEventSent_)
                {
                    failedEventSent_ = true;
                    completedEventSent_ = false;

                    eventDispatcher_?.Send(
                        this,
                        new LevelFailedEvent(currentLevelData_.id));
                }

                return;
            }

            completedEventSent_ = false;
            failedEventSent_ = false;
        }

        private void EnsureAvailableInstructionsResolverInitialized()
        {
            if (availableInstructionsResolver_ != null)
            {
                return;
            }

            if (levelsLibrary_ == null)
            {
                throw new InvalidOperationException(
                    "Levels library is not assigned on MovingGameController.");
            }

            int levelCount = levelsLibrary_.GetLevelCount();
            List<MovingGameLevelData> levels = new List<MovingGameLevelData>(levelCount);

            for (int i = 0; i < levelCount; i++)
            {
                levels.Add(levelsLibrary_.GetLevelAt(i));
            }

            availableInstructionsResolver_ = new MovingGameAvailableInstructionsResolver(levels);
        }

        private Core.MovingGame CreateGameFromLevelData(MovingGameLevelData levelData)
        {
            List<GridPosition> foodPositions = ConvertPositions(levelData.foodPositions);
            List<GridPosition> blockedPositions = ConvertPositions(levelData.blockedPositions);
            List<GridPosition> breakableBlockedPositions =
                ConvertPositions(levelData.breakableBlockedPositions);

            List<ToggleBlockedObstacleState> toggleBlockedObstacles =
                ConvertToggleBlockedTiles(levelData.toggleBlockedTiles);

            List<ToggleSwitchTileState> toggleSwitchTiles =
                ConvertToggleSwitchTiles(levelData.toggleSwitchTiles);

            return new Core.MovingGame(
                width: levelData.width,
                height: levelData.height,
                startCharacterPosition: ToGridPosition(levelData.startPosition),
                startCharacterDirection: ParseDirection(levelData.startDirection),
                foodPositions: foodPositions,
                blockedPositions: blockedPositions,
                breakableBlockedPositions: breakableBlockedPositions,
                toggleBlockedObstacles: toggleBlockedObstacles,
                toggleSwitchTiles: toggleSwitchTiles);
        }

        private void RenderGrid()
        {
            if (gridRenderer_ == null || game_ == null)
            {
                return;
            }

            gridRenderer_.RenderGrid(
                game_.GetWidth(),
                game_.GetHeight(),
                game_.GetBlockedPositions(),
                game_.GetBreakableBlockedPositions(),
                game_.GetVisitedPositions(),
                game_.GetToggleBlockedObstacles(),
                game_.GetToggleSwitchTiles());
        }

        private static List<ToggleBlockedObstacleState> ConvertToggleBlockedTiles(
    List<ToggleBlockedTileData> tiles)
        {
            List<ToggleBlockedObstacleState> result = new List<ToggleBlockedObstacleState>();

            if (tiles == null)
            {
                return result;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                ToggleBlockedTileData tile = tiles[i];
                if (tile == null)
                {
                    continue;
                }

                result.Add(new ToggleBlockedObstacleState(
                    new GridPosition(tile.x, tile.y),
                    tile.groupId,
                    tile.isOn));
            }

            return result;
        }

        private static List<ToggleSwitchTileState> ConvertToggleSwitchTiles(
            List<ToggleSwitchTileData> tiles)
        {
            List<ToggleSwitchTileState> result = new List<ToggleSwitchTileState>();

            if (tiles == null)
            {
                return result;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                ToggleSwitchTileData tile = tiles[i];
                if (tile == null)
                {
                    continue;
                }

                result.Add(new ToggleSwitchTileState(
                    new GridPosition(tile.x, tile.y),
                    tile.groupId));
            }

            return result;
        }

        private static List<GridPosition> ConvertPositions(List<PositionData> positions)
        {
            List<GridPosition> result = new List<GridPosition>();

            if (positions == null)
            {
                return result;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                result.Add(ToGridPosition(positions[i]));
            }

            return result;
        }

        private static GridPosition ToGridPosition(PositionData position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            return new GridPosition(position.x, position.y);
        }

        private static Direction ParseDirection(string direction)
        {
            if (string.IsNullOrWhiteSpace(direction))
            {
                return Direction.Up;
            }

            return direction.Trim().ToLowerInvariant() switch
            {
                "up" => Direction.Up,
                "right" => Direction.Right,
                "down" => Direction.Down,
                "left" => Direction.Left,
                _ => Direction.Up
            };
        }
    }
}