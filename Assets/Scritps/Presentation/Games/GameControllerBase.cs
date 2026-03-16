using System.Collections;
using UnityEngine;
using CodingGame.Runtime.Core;
using CodingGame.Runtime.Games.Moving;
using CodingGame.Runtime.Instructions;

namespace CodingGame.Presentation.Games
{
    /// <summary>
    /// Coordinates the game runtime, scene view, and execution controls.
    /// </summary>
    public abstract class GameControllerBase<TGame> : MonoBehaviour where TGame : IGame
    {
        [Header("Execution")]
        [SerializeField] private float stepDelaySeconds_ = 0.5f;

        private TGame game_;
        private ProgramRunner runner_;
        private ProgramDefinition currentProgram_;
        private Coroutine runCoroutine_;

        /// <summary>
        /// Initializes the game, demo program, and view.
        /// </summary>
        private void Start()
        {
            runner_ = new ProgramRunner();
            game_ = CreateGame();
            CreateNewProgram();
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
        /// Deletes all instructions inside the program
        /// </summary>
        public void CleanProgram()
        {
            ResetGame();
            CreateNewProgram();
        }

        /// <summary>
        /// Add an instruction of the given instruction definition to the current program
        /// </summary>
        /// <param name="instructionDefinition"></param>
        /// <exception cref="System.Exception"></exception>
        protected void AddInstructionToCurrentProgram(GameInstructionDefinitionBase<TGame> instructionDefinition)
        {
            if (currentProgram_ == null)
            {
                throw new System.Exception("A program has to be created to add instructions.");
            }
            InstructionInstance instructionInstance = new InstructionInstance(instructionDefinition);
            currentProgram_.AddInstruction(instructionInstance);
        }

        protected abstract void InitializeView();

        protected abstract TGame CreateGame();

        protected abstract void RefreshViewImmediate();

        protected abstract void RefreshViewAnimated();

        private void CreateNewProgram()
        {
            currentProgram_ = new ProgramDefinition();
            runner_.LoadProgram(currentProgram_);
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
    }
}