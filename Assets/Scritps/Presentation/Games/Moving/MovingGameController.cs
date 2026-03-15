using System.Collections;
using UnityEngine;
using CodingGame.Runtime.Core;
using CodingGame.Runtime.Games.Moving;

namespace CodingGame.Presentation.Games.Moving
{
    /// <summary>
    /// Coordinates the moving game runtime, scene view, and execution controls.
    /// </summary>
    public sealed class MovingGameController : MonoBehaviour
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

        [Header("Execution")]
        [SerializeField] private float stepDelaySeconds_ = 0.5f;

        private Runtime.Games.Moving.MovingGame game_;
        private ProgramRunner runner_;
        private ProgramDefinition program_;
        private Coroutine runCoroutine_;

        /// <summary>
        /// Initializes the game, demo program, and view.
        /// </summary>
        private void Start()
        {
            CreateGame();
            CreateProgram();
            InitializeView();
            RefreshViewImmediate();
        }

        /// <summary>
        /// Executes one program step.
        /// </summary>
        public void Step()
        {
            if (!CanExecuteStep())
            {
                return;
            }

            runner_.ExecuteNextStep(game_);
            RefreshViewAnimated();
        }

        /// <summary>
        /// Runs the loaded program automatically.
        /// </summary>
        public void Run()
        {
            if (runCoroutine_ != null)
            {
                return;
            }

            runCoroutine_ = StartCoroutine(RunRoutine());
        }

        /// <summary>
        /// Stops automatic execution if it is running.
        /// </summary>
        public void Stop()
        {
            StopRunning();
        }

        /// <summary>
        /// Resets the game and execution state.
        /// </summary>
        public void ResetGame()
        {
            StopRunning();

            if (game_ == null || runner_ == null)
            {
                return;
            }

            game_.ResetGame();
            runner_.ResetExecution();
            RefreshViewImmediate();
        }

        /// <summary>
        /// Rebuilds the demo program and resets the game.
        /// </summary>
        public void RebuildProgram()
        {
            StopRunning();

            CreateProgram();

            if (game_ != null)
            {
                game_.ResetGame();
            }

            RefreshViewImmediate();
        }

        /// <summary>
        /// Returns the current moving game instance.
        /// </summary>
        public Runtime.Games.Moving.MovingGame GetGame()
        {
            return game_;
        }

        /// <summary>
        /// Returns the current program runner instance.
        /// </summary>
        public ProgramRunner GetRunner()
        {
            return runner_;
        }

        private void CreateGame()
        {
            game_ = new Runtime.Games.Moving.MovingGame(
                width: gridWidth_,
                height: gridHeight_,
                startCharacterPosition: new GridPosition(startPosition_.x, startPosition_.y),
                startCharacterDirection: startDirection_,
                foodPosition: new GridPosition(foodPosition_.x, foodPosition_.y));

            runner_ = new ProgramRunner();
        }

        private void CreateProgram()
        {
            program_ = new ProgramDefinition();

            InstructionInstance move1 =
                new InstructionInstance(new MoveForwardInstructionDefinition());

            InstructionInstance move2 =
                new InstructionInstance(new MoveForwardInstructionDefinition());

            InstructionInstance turn =
                new InstructionInstance(new RotateLeftInstructionDefinition());

            InstructionInstance move3 =
                new InstructionInstance(new MoveForwardInstructionDefinition());

            program_.AddInstruction(move1);
            program_.AddInstruction(move2);
            program_.AddInstruction(turn);
            program_.AddInstruction(move3);

            runner_.LoadProgram(program_);
        }

        private void InitializeView()
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

        private bool CanExecuteStep()
        {
            if (game_ == null || runner_ == null)
            {
                return false;
            }

            if (game_.HasWon() || game_.HasFailed())
            {
                return false;
            }

            if (runner_.IsFinished())
            {
                return false;
            }

            return true;
        }

        private IEnumerator RunRoutine()
        {
            while (CanExecuteStep())
            {
                runner_.ExecuteNextStep(game_);
                RefreshViewAnimated();

                yield return new WaitForSeconds(stepDelaySeconds_);
            }

            runCoroutine_ = null;
        }

        private void StopRunning()
        {
            if (runCoroutine_ == null)
            {
                return;
            }

            StopCoroutine(runCoroutine_);
            runCoroutine_ = null;
        }

        private void RefreshViewImmediate()
        {
            if (movingGameView_ == null || game_ == null)
            {
                return;
            }

            movingGameView_.RefreshImmediate(game_);
        }

        private void RefreshViewAnimated()
        {
            if (movingGameView_ == null || game_ == null)
            {
                return;
            }

            movingGameView_.RefreshAnimated(game_);
        }
    }
}