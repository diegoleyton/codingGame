using System.Collections;
using UnityEngine;
using CodingGame.Runtime.Games.Moving;

namespace CodingGame.Presentation.Games.Moving
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
        [SerializeField] private Vector2Int foodPosition_ = new Vector2Int(3, 1);

        private MovingGame game_;

        public void AddMoveForwardInstruction()
        {
            AddInstructionToCurrentProgram(new MoveForwardInstructionDefinition());
        }

        public void AddRotateLeftInstruction()
        {
            AddInstructionToCurrentProgram(new RotateLeftInstructionDefinition());
        }

        public void AddRotateRightInstruction()
        {
            AddInstructionToCurrentProgram(new RotateRightInstructionDefinition());
        }

        protected override IMovingGame CreateGame()
        {
            game_ = new MovingGame(
                width: gridWidth_,
                height: gridHeight_,
                startCharacterPosition: new GridPosition(startPosition_.x, startPosition_.y),
                startCharacterDirection: startDirection_,
                foodPosition: new GridPosition(foodPosition_.x, foodPosition_.y));

            return game_;
        }

        protected override void InitializeView()
        {
            if (gridRenderer_ != null)
            {
                gridRenderer_.RenderGrid(gridWidth_, gridHeight_);
            }

            if (movingGameView_ != null && game_ != null)
            {
                movingGameView_.Initialize(gridRenderer_, game_);
            }
        }

        protected override void RefreshViewImmediate()
        {
            if (movingGameView_ == null || game_ == null)
            {
                return;
            }

            movingGameView_.RefreshImmediate(game_);
        }

        protected override IEnumerator RefreshViewAnimated()
        {
            if (movingGameView_ == null || game_ == null)
            {
                yield break;
            }

            yield return movingGameView_.RefreshAnimated(game_);
        }
    }
}