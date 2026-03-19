using System;
using System.Collections;
using UnityEngine;
using Flowbit.Engine;
using Flowbit.Engine.Instructions;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Coordinates the game runtime, scene view, and execution controls.
    /// </summary>
    public abstract class GameControllerBase<TGame> : MonoBehaviour
        where TGame : class, IGame
    {
        [Header("UI")]
        [SerializeField] private ProgramViewBase programPanelView_;
        [SerializeField] private GameStatusViewBase gameStatusView_;

        [Header("Execution")]
        [SerializeField] private float stepDelaySeconds_ = 0.5f;

        private TGame game_;
        private ProgramRunner runner_;
        private ProgramDefinition currentProgram_;
        private Coroutine runCoroutine_;
        private Coroutine stepCoroutine_;
        private bool programDirty_;

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Start()
        {
            runner_ = new ProgramRunner();

            if (ShouldCreateGameOnStart())
            {
                LoadGame(CreateGame());
                return;
            }

            CreateNewProgram();
            ClearInstructionHighlight();
            RefreshResultView();
        }

        /// <summary>
        /// Executes one program step.
        /// </summary>
        public void Step()
        {
            if (stepCoroutine_ != null || runCoroutine_ != null)
            {
                return;
            }

            if (!CanExecuteStep())
            {
                return;
            }

            stepCoroutine_ = StartCoroutine(StepRoutine());
        }

        /// <summary>
        /// Runs the loaded program automatically.
        /// </summary>
        public void Run()
        {
            if (runCoroutine_ != null || stepCoroutine_ != null)
            {
                return;
            }

            EnsureRunnerReady();
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
            programDirty_ = false;

            RefreshViewImmediate();
            ClearInstructionHighlight();
            RefreshResultView();
        }

        /// <summary>
        /// Deletes all instructions inside the program.
        /// </summary>
        public void CleanProgram()
        {
            StopRunning();

            if (game_ == null)
            {
                return;
            }

            game_.ResetGame();
            CreateNewProgram();

            RefreshViewImmediate();
            ClearInstructionHighlight();
            RefreshResultView();
        }

        /// <summary>
        /// Removes the last instruction from the current program.
        /// </summary>
        public void RemoveLastInstruction()
        {
            if (currentProgram_ == null || currentProgram_.GetInstructionCount() == 0)
            {
                return;
            }

            currentProgram_.RemoveLastInstruction();
            OnProgramChanged();
        }

        /// <summary>
        /// Returns the current game instance.
        /// </summary>
        protected TGame GetGame()
        {
            return game_;
        }

        /// <summary>
        /// Returns the current program definition.
        /// </summary>
        protected ProgramDefinition GetCurrentProgram()
        {
            return currentProgram_;
        }

        /// <summary>
        /// Returns whether a game should be created automatically on start.
        /// </summary>
        protected virtual bool ShouldCreateGameOnStart()
        {
            return true;
        }

        /// <summary>
        /// Loads a new game instance into the controller.
        /// </summary>
        protected void LoadGame(TGame game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            StopRunning();

            if (runner_ == null)
            {
                runner_ = new ProgramRunner();
            }

            game_ = game;
            CreateNewProgram();
            InitializeView();
            RefreshViewImmediate();
            ClearInstructionHighlight();
            RefreshResultView();
        }

        /// <summary>
        /// Adds an instruction of the given instruction definition to the current program.
        /// </summary>
        protected void AddInstructionToCurrentProgram(
            GameInstructionDefinitionBase<TGame> instructionDefinition)
        {
            if (currentProgram_ == null)
            {
                throw new InvalidOperationException(
                    "A program must be created before adding instructions.");
            }

            InstructionInstance instructionInstance =
                new InstructionInstance(instructionDefinition);

            currentProgram_.AddInstruction(instructionInstance);
            OnProgramChanged();
        }

        /// <summary>
        /// Called when the current program changes.
        /// </summary>
        protected virtual void OnProgramChanged()
        {
            programDirty_ = true;

            if (programPanelView_ != null)
            {
                programPanelView_.Rebuild(
                    currentProgram_.GenerateReadOnlyInstructions());
            }

            ClearInstructionHighlight();
            RefreshResultView();
        }

        /// <summary>
        /// Highlights the instruction at the given index.
        /// </summary>
        protected virtual void HighlightInstruction(int instructionIndex)
        {
            if (programPanelView_ == null)
            {
                return;
            }

            if (instructionIndex < 0)
            {
                programPanelView_.ClearHighlight();
                return;
            }

            programPanelView_.HighlightIndex(instructionIndex);
        }

        /// <summary>
        /// Clears the current instruction highlight.
        /// </summary>
        protected virtual void ClearInstructionHighlight()
        {
            if (programPanelView_ == null)
            {
                return;
            }

            programPanelView_.ClearHighlight();
        }

        /// <summary>
        /// Updates the result UI based on the current game state.
        /// </summary>
        protected virtual void RefreshResultView()
        {
            if (gameStatusView_ == null || game_ == null)
            {
                return;
            }

            if (game_.HasWon())
            {
                gameStatusView_.Win();
                return;
            }

            if (game_.HasFailed())
            {
                gameStatusView_.Lose();
                return;
            }

            gameStatusView_.Idle();
        }

        /// <summary>
        /// Initializes the scene view.
        /// </summary>
        protected abstract void InitializeView();

        /// <summary>
        /// Creates the initial game instance.
        /// </summary>
        protected abstract TGame CreateGame();

        /// <summary>
        /// Refreshes the view immediately.
        /// </summary>
        protected abstract void RefreshViewImmediate();

        /// <summary>
        /// Refreshes the view with animation.
        /// </summary>
        protected abstract IEnumerator RefreshViewAnimated();

        private void CreateNewProgram()
        {
            currentProgram_ = new ProgramDefinition();
            runner_.LoadProgram(currentProgram_);
            programDirty_ = false;

            if (programPanelView_ != null)
            {
                programPanelView_.Rebuild(
                    currentProgram_.GenerateReadOnlyInstructions());
            }
        }

        private void EnsureRunnerReady()
        {
            if (!programDirty_)
            {
                return;
            }

            runner_.ResetExecution();
            programDirty_ = false;
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

        private IEnumerator StepRoutine()
        {
            EnsureRunnerReady();

            int currentInstructionIndex = runner_.GetCurrentInstructionIndex();
            HighlightInstruction(currentInstructionIndex);

            runner_.ExecuteNextStep(game_);
            yield return RefreshViewAnimated();
            RefreshResultView();

            stepCoroutine_ = null;
        }

        private IEnumerator RunRoutine()
        {
            while (CanExecuteStep())
            {
                int currentInstructionIndex = runner_.GetCurrentInstructionIndex();
                HighlightInstruction(currentInstructionIndex);

                runner_.ExecuteNextStep(game_);
                yield return RefreshViewAnimated();
                RefreshResultView();

                if (!CanExecuteStep())
                {
                    break;
                }

                yield return new WaitForSeconds(stepDelaySeconds_);
            }

            runCoroutine_ = null;
        }

        private void StopRunning()
        {
            if (runCoroutine_ != null)
            {
                StopCoroutine(runCoroutine_);
                runCoroutine_ = null;
            }

            if (stepCoroutine_ != null)
            {
                StopCoroutine(stepCoroutine_);
                stepCoroutine_ = null;
            }
        }
    }
}