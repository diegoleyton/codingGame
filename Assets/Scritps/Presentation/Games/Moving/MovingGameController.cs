using System.Collections;
using UnityEngine;
using CodingGame.Runtime.Core;
using CodingGame.Runtime.Games.Moving;

namespace CodingGame.Presentation.Games.Moving
{
    /// <summary>
    /// Connects the moving game runtime to the Unity scene and UI.
    /// </summary>
    public sealed class MovingGameController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform character_;
        [SerializeField] private Transform food_;

        [Header("Grid")]
        [SerializeField] private float cellSize_ = 1.0f;

        [Header("Run Settings")]
        [SerializeField] private float stepDelaySeconds_ = 0.5f;

        private Runtime.Games.Moving.MovingGame game_;
        private ProgramRunner runner_;
        private ProgramDefinition program_;
        private Coroutine runCoroutine_;

        /// <summary>
        /// Initializes the moving game and demo program.
        /// </summary>
        private void Start()
        {
            CreateGame();
            CreateProgram();
            UpdateView();
        }

        /// <summary>
        /// Executes the next step in the loaded program.
        /// </summary>
        public void Step()
        {
            if (game_ == null || runner_ == null)
            {
                return;
            }

            if (game_.HasWon() || game_.HasFailed() || runner_.IsFinished())
            {
                return;
            }

            runner_.ExecuteNextStep(game_);
            UpdateView();
        }

        /// <summary>
        /// Runs the loaded program automatically with a delay between steps.
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
            UpdateView();
        }

        /// <summary>
        /// Rebuilds the demo program and resets execution.
        /// </summary>
        public void RebuildProgram()
        {
            StopRunning();

            CreateProgram();

            if (game_ != null)
            {
                game_.ResetGame();
            }

            UpdateView();
        }

        private void CreateGame()
        {
            game_ = new Runtime.Games.Moving.MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(3, 1));

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

        private IEnumerator RunRoutine()
        {
            while (game_ != null &&
                   runner_ != null &&
                   !runner_.IsFinished() &&
                   !game_.HasWon() &&
                   !game_.HasFailed())
            {
                runner_.ExecuteNextStep(game_);
                UpdateView();

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

        private void UpdateView()
        {
            if (game_ == null)
            {
                return;
            }

            UpdateCharacterView();
            UpdateFoodView();
        }

        private void UpdateCharacterView()
        {
            if (character_ == null)
            {
                return;
            }

            GridPosition characterPosition = game_.GetCharacterPosition();

            character_.position = GridToWorldPosition(characterPosition);
            character_.rotation = DirectionToWorldRotation(game_.GetCharacterDirection());
        }

        private void UpdateFoodView()
        {
            if (food_ == null)
            {
                return;
            }

            food_.position = GridToWorldPosition(game_.GetFoodPosition());
        }

        private Vector3 GridToWorldPosition(GridPosition position)
        {
            return new Vector3(
                position.GetX() * cellSize_,
                0f,
                position.GetY() * cellSize_);
        }

        private Quaternion DirectionToWorldRotation(Direction direction)
        {
            float yRotation = direction switch
            {
                Direction.Up => 0f,
                Direction.Right => 90f,
                Direction.Down => 180f,
                Direction.Left => 270f,
                _ => 0f
            };

            return Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}