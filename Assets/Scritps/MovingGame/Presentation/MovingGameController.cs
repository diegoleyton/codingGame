using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flowbit.MovingGame.Core;
using Flowbit.MovingGame.Core.Instructions;
using Flowbit.EngineController;

namespace Flowbit.MovingGame.Presentation
{
    /// <summary>
    /// Coordinates the moving game runtime, scene view, and execution controls.
    /// </summary>
    public sealed class MovingGameController : GameControllerBase<IMovingGame>
    {
        [Header("View References")]
        [SerializeField] private MovingGameView movingGameView_;
        [SerializeField] private GridRenderer gridRenderer_;

        [Header("Game Setup")]
        [SerializeField] private int gridWidth_ = 5;
        [SerializeField] private int gridHeight_ = 5;
        [SerializeField] private Vector2Int startPosition_ = new Vector2Int(0, 0);
        [SerializeField] private Direction startDirection_ = Direction.Right;
        [SerializeField] private Vector2Int[] foodPositions_;

        [Header("Obstacles")]
        [SerializeField] private Vector2Int[] blockedPositions_;
        [SerializeField] private Vector2Int[] breakableBlockedPositions_;

        private Core.MovingGame game_;

        /// <summary>
        /// Adds a move-forward instruction to the current program.
        /// </summary>
        public void AddMoveForwardInstruction()
        {
            AddInstructionToCurrentProgram(new MoveForwardInstructionDefinition());
        }

        /// <summary>
        /// Adds a rotate-left instruction to the current program.
        /// </summary>
        public void AddRotateLeftInstruction()
        {
            AddInstructionToCurrentProgram(new RotateLeftInstructionDefinition());
        }

        /// <summary>
        /// Adds a rotate-right instruction to the current program.
        /// </summary>
        public void AddRotateRightInstruction()
        {
            AddInstructionToCurrentProgram(new RotateRightInstructionDefinition());
        }

        /// <summary>
        /// Adds a break-forward instruction to the current program.
        /// </summary>
        public void AddBreakForwardInstruction()
        {
            AddInstructionToCurrentProgram(new BreakForwardInstructionDefinition());
        }

        /// <summary>
        /// Creates the moving game instance.
        /// </summary>
        protected override IMovingGame CreateGame()
        {
            List<GridPosition> foodPositions = new List<GridPosition>();
            List<GridPosition> blockedPositions = new List<GridPosition>();
            List<GridPosition> breakableBlockedPositions = new List<GridPosition>();

            if (foodPositions_ != null)
            {
                for (int i = 0; i < foodPositions_.Length; i++)
                {
                    foodPositions.Add(
                        new GridPosition(foodPositions_[i].x, foodPositions_[i].y));
                }
            }

            if (blockedPositions_ != null)
            {
                for (int i = 0; i < blockedPositions_.Length; i++)
                {
                    blockedPositions.Add(
                        new GridPosition(blockedPositions_[i].x, blockedPositions_[i].y));
                }
            }

            if (breakableBlockedPositions_ != null)
            {
                for (int i = 0; i < breakableBlockedPositions_.Length; i++)
                {
                    breakableBlockedPositions.Add(
                        new GridPosition(
                            breakableBlockedPositions_[i].x,
                            breakableBlockedPositions_[i].y));
                }
            }

            game_ = new Core.MovingGame(
                width: gridWidth_,
                height: gridHeight_,
                startCharacterPosition: new GridPosition(startPosition_.x, startPosition_.y),
                startCharacterDirection: startDirection_,
                foodPositions: foodPositions,
                blockedPositions: blockedPositions,
                breakableBlockedPositions: breakableBlockedPositions);

            return game_;
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

            RenderGrid();
            yield return movingGameView_.RefreshAnimated(game_);
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
                game_.GetBreakableBlockedPositions());
        }
    }
}